using RabbitMQ.Client;

namespace SubmissionProcessor.Worker.Interfaces
{
    public interface IConnectionManagerService
    {
        Task<IConnection> CreateConnection( CancellationToken cancellationToken = default);
    }
}