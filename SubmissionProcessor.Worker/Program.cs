using SubmissionProcessor.Worker;
using SubmissionProcessor.Worker.Settings;
using SubmissionProcessor.Worker.Workers;
using SubmissionProcessor.Worker.Data;
using Microsoft.EntityFrameworkCore;
using SubmissionProcessor.Worker.Interfaces;
using SubmissionProcessor.Worker.Services;
using Microsoft.Extensions.Http.Resilience;
using System.Net.Http.Headers;
using DotNetEnv;
using Serilog;

DotNetEnv.Env.Load();


// using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQSettings")
);

builder.Services.Configure<TrainingDirectorySettings>(
    builder.Configuration.GetSection("TrainingDirectorySettings")
);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(logEvent =>
            logEvent.Properties.TryGetValue("SourceContext", out var value) &&
            (value.ToString().Contains("Microsoft") ||
            value.ToString().Contains("System")))
        .WriteTo.File(
            path: "logs/app-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}"))
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

var directorySettings = builder.Configuration.GetSection("TrainingDirectorySettings").Get<TrainingDirectorySettings>()
    ?? throw new InvalidOperationException("TrainingDirectory settings are missing!");

// Typed HttpClient
builder.Services.AddHttpClient<ITrainingDirectoryClient, TrainingDirectoryClient>(client =>
{
    client.BaseAddress = new Uri(directorySettings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(directorySettings.TimeoutSeconds);

    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddStandardResilienceHandler(options =>
{
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(8);

    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);

    options.Retry.MaxRetryAttempts = 2;
    options.Retry.Delay = TimeSpan.FromSeconds(1);

    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
    options.CircuitBreaker.MinimumThroughput = 2;
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);
});


builder.Services.AddHostedService<SubmissionProcessingWorker>();


var host = builder.Build();
// host.UseExceptionHandler();
host.Run();
