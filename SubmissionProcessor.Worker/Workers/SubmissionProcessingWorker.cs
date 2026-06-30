using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SubmissionProcessor.Worker.Settings;
using SubmissionProcessor.Worker.Models;
using SubmissionProcessor.Worker.Data;
using Microsoft.EntityFrameworkCore;
using SubmissionProcessor.Worker.Interfaces;
using SubmissionProcessor.Worker.DTOs.TraineeProfileDto;

namespace SubmissionProcessor.Worker.Workers;

public class SubmissionProcessingWorker : BackgroundService
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<SubmissionProcessingWorker> _logger;

    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public SubmissionProcessingWorker(
        IOptions<RabbitMQSettings> options,
        ILogger<SubmissionProcessingWorker> logger,
        IServiceScopeFactory scopeFactory
     )
    {
        _settings = options.Value;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                VirtualHost = _settings.VirtualHost,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            if (!_connection.IsOpen)
            {
                throw new Exception("Connection to RabbitMQ could not be made");
            }
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            //DEAD LETTER QUEUE
            string dlxName = _settings.DlxName;
            string dlqName = _settings.DlqName;

            await _channel.ExchangeDeclareAsync(
                exchange: dlxName,
                type: ExchangeType.Direct,
                durable: true,
                cancellationToken: cancellationToken
            );

            await _channel.QueueDeclareAsync(
                queue: dlqName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            await _channel.QueueBindAsync(
                queue: dlqName,
                exchange: dlxName,
                routingKey: "dead-letter",
                cancellationToken: cancellationToken
            );
            var mainQueueArguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", dlxName },
            { "x-dead-letter-routing-key", "dead-letter" }
        };

            await _channel.QueueDeclareAsync(
                queue: _settings.SubmissionProcessingQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: mainQueueArguments,
                cancellationToken: cancellationToken
            );

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken
            );

            await base.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "RabbitMQ connection failed on the worker side: {message}", ex.Message);

            await Task.Delay(TimeSpan.FromSeconds(15),cancellationToken);

            await StartAsync(cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized");
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<SubmissionProcessingRequested>(json);

                if (message == null)
                {
                    _logger.LogWarning("Received Invalid or Empty Message Payload");
                    await _channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken
                    );
                    return;
                }

                using (var scope = _scopeFactory.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var existingJob = await _context.ProcessingJobs.FirstOrDefaultAsync(t => t.CorrelationId == message.CorrelationId);

                    if (existingJob == null)
                    {
                        existingJob = new ProcessingJob
                        {
                            MessageId = message.MessageId,
                            CorrelationId = message.CorrelationId,
                            SubmissionId = message.SubmissionId,
                            FileId = message.FileId,
                            StartedAt = DateTime.Now
                        };

                        await _context.ProcessingJobs.AddAsync(existingJob);
                        await _context.SaveChangesAsync();

                    }
                    else if (existingJob.Status == ProcessingJobStatus.Queued)
                    {
                        existingJob.Status = ProcessingJobStatus.Processing;
                        existingJob.StartedAt = DateTime.Now;
                        _logger.LogInformation("Processing Job {existingJobId} for message {MessageId} has started.", existingJob.Id, message.MessageId);
                    }

                    // Idempotency
                    if (existingJob != null && existingJob.Status == ProcessingJobStatus.Completed)
                    {
                        _logger.LogCritical("Duplicate message ignored by RabbitMQ");
                        await _channel.BasicAckAsync(
                            deliveryTag: ea.DeliveryTag,
                            multiple: false,
                            cancellationToken: stoppingToken
                        );
                        return;
                    }

                    try
                    {
                        var metadata = await _context.SubmissionFiles.FindAsync(message.FileId);

                        if (metadata == null)
                        {
                            _logger.LogCritical("Metadata not found for file with Id {id}", message.FileId);
                            throw new FileNotFoundException($"Metadata file not found for file with id: {message.FileId}");
                        }
                        else
                        {
                            // throw new InvalidOperationException("Simulated external API or processing failure.");

                            _logger.LogInformation("Metadata of the file: ID: {FileId}, Name: {FileName}, Size: {FileSize} bytes, ContentType: {ContentType}, Checksum: {Checksum}, CreatedDate: {CreatedDate}", metadata.Id, metadata.OriginalFileName, metadata.FileSize, metadata.ContentType, metadata.CheckSum, metadata.CreatedAt);

                            _logger.LogInformation("Fetching trainee profile from directory");
                            var directoryClient = scope.ServiceProvider.GetRequiredService<ITrainingDirectoryClient>();

                            TraineeProfileResponse? traineeProfile = null;

                            try
                            {
                                traineeProfile = await directoryClient.GetProfiles(
                                    123,
                                    message.CorrelationId,
                                    stoppingToken
                                );
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Training Directory service is unavailable after resilience retries. Executing fallback policy.");
                                traineeProfile = new TraineeProfileResponse
                                {
                                    Id = 123,
                                    Name = "Fallback Profile (Service Offline)",
                                    Email = "offline@system.com",
                                    Designation = "Unknown"
                                };
                            }

                            _logger.LogInformation("Successfully retrieved profile for {Id} {Name} {Email} ({Designation})", traineeProfile.Id, traineeProfile.Name, traineeProfile.Email, traineeProfile.Designation);

                            existingJob.Status = ProcessingJobStatus.Completed;
                            existingJob.Attempt++;
                            existingJob.CompletedAt = DateTime.Now;

                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Processing of message with Id {MessageId} completed. Status: {MessafeStatus}", message.MessageId, existingJob.Status);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Processing logic has failed for message with Id {MessageId}", message.MessageId);
                        existingJob.ErrorSummary = ex.Message;
                        bool isPermanentFailure = ex is FileNotFoundException;
                        bool isRetryExhausted = existingJob.Attempt >= _settings.MaxRetryAttempts;

                        if (isPermanentFailure || isRetryExhausted)
                        {
                            existingJob.Status = ProcessingJobStatus.Failed;

                            await _context.SaveChangesAsync();
                            _logger.LogCritical("Job Status: {JobStatus}, Adding to dead letter queue", existingJob.Status);

                            await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                requeue: false,
                                cancellationToken: stoppingToken
                            );
                        }
                        else
                        {
                            existingJob.Status = ProcessingJobStatus.Queued;
                            existingJob.Attempt++;

                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Attempt: {Attempts}. Changing Status to {JobStatus}. Retrying...", existingJob.Attempt, existingJob.Status);

                            await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                requeue: true,
                                cancellationToken: stoppingToken
                            );
                        }
                        return;
                    }
                }

                _logger.LogInformation("Received message. MessageId:{MessageId}, CorrelationId:{CorrelationId}, SubmissionId:{SubmissionId}",
                    message.MessageId, message.CorrelationId, message.SubmissionId);
                await Task.Delay(500, stoppingToken);

                await _channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken
                );

                _logger.LogInformation("Acknowledged Message {MessageId}", message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing RabbitMQ message: {Message}", ex.Message);
                if (_channel != null)
                {
                    await _channel.BasicNackAsync(
                                deliveryTag: ea.DeliveryTag,
                                multiple: false,
                                requeue: true,
                                cancellationToken: stoppingToken
                            );
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _settings.SubmissionProcessingQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken: cancellationToken);
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken: cancellationToken);
            await _connection.DisposeAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}