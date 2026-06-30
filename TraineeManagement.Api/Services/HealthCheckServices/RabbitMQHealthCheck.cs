using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.Api.Settings;

namespace TraineeManagement.Api.Services.HealthCheckServices
{
    public class RabbitMQHealthCheck : IHealthCheck
    {
        private readonly RabbitMQSettings _settings;
        public RabbitMQHealthCheck(
            IOptions<RabbitMQSettings> settings
        )
        {
            _settings = settings.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
            };

            await using var connection = await factory.CreateConnectionAsync(cancellationToken);
            if (connection.IsOpen)
            {
                return HealthCheckResult.Healthy("RabbitMQ is reachable");
            }

            return HealthCheckResult.Unhealthy("RabbitMQ is unreachable");
        }
    }
}