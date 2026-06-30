using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Settings;

namespace TraineeManagement.Api.Services
{
    public class ConnectionManagerService : IConnectionManagerService
    {

        private readonly RabbitMQSettings _settings;

        public ConnectionManagerService(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<IConnection> CreateConnection(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
            };

            return await factory.CreateConnectionAsync(cancellationToken);
        }
    }
}