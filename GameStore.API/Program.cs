using GameStore.API.Data;
using GameStore.API.Database;
using GameStore.API.Endpoints;
using GameStore.API.Health;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// logging
builder.Services.AddHttpLogging(options => {});

// db connection string
var connectionString = builder.Configuration.GetConnectionString("GameStore:PostgreSQL");

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(builder.Configuration.GetConnectionString("GameStore:SQLite")!));

// health checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("sqlite")
    .AddNpgSql(connectionString!);

//builder.Services.AddSqlite<GameStoreContext>(connectionString);

builder.Services.AddNpgsql<GameStoreContext>(connectionString);

var app = builder.Build();

// logging
app.UseHttpLogging();


// health checks
app.UseHealthChecks("/_health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// database migration
await app.MigrateDbAsync();

// endpoints
app.MapGamesEndpoints();
app.MapGenresEndpoints();

app.Run();
