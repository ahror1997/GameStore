using GameStore.API.Data;
using GameStore.API.Database;
using GameStore.API.Endpoints;
using GameStore.API.Health;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;

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


builder.Services.AddOpenTelemetry()
    .WithMetrics(options =>
    {
        options.AddPrometheusExporter();

        options.AddMeter("Microsoft.AspNetCore.Hosting",
                         "Microsoft.AspNetCore.Server.Kestrel");

        options.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                       0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
            });
    });

builder.Services.AddMetrics();

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

app.MapPrometheusScrapingEndpoint();

// endpoints
app.MapGamesEndpoints();
app.MapGenresEndpoints();

app.Run();
