using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Data;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        await dbContext.Database.MigrateAsync();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DB Initializer");

        logger.LogInformation("The database is ready!");
    }
}
