using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Settings;
using System.Collections.Generic;
using TraineeManagement.Api.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace TraineeManagement.Api.Services
{
    public class SubmissionProcessingPublisher : ISubmissionProcessingPublisherService
    {
        private readonly IConnectionManagerService _service;
        private readonly ILogger<SubmissionProcessingPublisher> _logger;

        private readonly RabbitMQSettings _settings;

        public SubmissionProcessingPublisher(
            IConnectionManagerService service,
            ILogger<SubmissionProcessingPublisher> logger,
            IOptions<RabbitMQSettings> settings
        )
        {
            _service = service;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task PublishAsync(SubmissionProcessingRequested message)
        {
            try
            {
                var connection = await _service.CreateConnection();
                using (var channel = await connection.CreateChannelAsync())
                {
                    var mainQueueArguments = new Dictionary<string, object?>
                    {
                        { "x-dead-letter-exchange", _settings.DlxName },
                        { "x-dead-letter-routing-key", "dead-letter" }
                    };

                    await channel.QueueDeclareAsync(
                        queue: _settings.SubmissionProcessingQueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: mainQueueArguments
                    );

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                    var properties = new BasicProperties
                    {
                        Persistent = true,
                        MessageId = message.MessageId.ToString(),
                        CorrelationId = message.CorrelationId ?? Guid.NewGuid().ToString(),
                        ContentType = "application/json",
                        Type = nameof(SubmissionProcessingRequested)
                    };

                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: _settings.SubmissionProcessingQueueName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body
                    );
                }

                _logger.LogInformation("Published Message Id {MessageId} Correlation Id {CorrelationId} Submission Id {SubmissionId}", message.MessageId, message.CorrelationId, message.SubmissionId);
            }
            catch (Exception ex)
            {
                _logger.LogError("No queuing of submission request due to broker failure or timeout : {ex}", ex.Message);
                throw new Exception("Connection attempt failed to message broker");
            }
        }
    }
}