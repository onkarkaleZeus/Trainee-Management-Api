using RabbitMQ.Client;

namespace TraineeManagement.Api.Interfaces
{
    public interface IConnectionManagerService
    {
        Task<IConnection> CreateConnection( CancellationToken cancellationToken = default);
    }
}