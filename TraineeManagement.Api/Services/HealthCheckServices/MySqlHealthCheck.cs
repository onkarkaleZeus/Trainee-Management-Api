using Microsoft.Extensions.Diagnostics.HealthChecks;
using TraineeManagement.Api.Data;

namespace TraineeManagement.Api.Services.HealthCheckServices
{
    public class MySqlHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _context;
        public MySqlHealthCheck(
            AppDbContext context
        )
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var connect = await _context.Database.CanConnectAsync(cancellationToken);
            if( connect )
            {
                return HealthCheckResult.Healthy("MySql is reachable");
            }

            return HealthCheckResult.Unhealthy("MySql is unreachable");
        }
    }
}