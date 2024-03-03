using GameStore.API.Data;
using GameStore.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(options => {});

var connectionString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connectionString);

var app = builder.Build();

app.UseHttpLogging();

await app.MigrateDbAsync();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

app.Run();
