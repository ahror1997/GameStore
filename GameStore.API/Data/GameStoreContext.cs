using GameStore.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext (options)
{
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Game> Games => Set<Game>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Genre>().HasData(
            new Genre() { Id = 1, Name = "Fighting" },
            new Genre() { Id = 2, Name = "RP" },
            new Genre() { Id = 3, Name = "Sports" },
            new Genre() { Id = 4, Name = "Racing" },
            new Genre() { Id = 5, Name = "Kids and Family" }
        );
    }
}
