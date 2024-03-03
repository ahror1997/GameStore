using GameStore.API.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameStore.API.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = await _dbConnectionFactory.CreateConnectionAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                command.ExecuteScalar();

                return HealthCheckResult.Healthy();
            }
            catch (Exception exception)
            {

                return HealthCheckResult.Unhealthy(exception: exception);
            }
        }
    }
}
