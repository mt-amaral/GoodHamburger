using GoodHamburger.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Context;

public sealed class GoodHamburgerDbContext(DbContextOptions<GoodHamburgerDbContext> options) : DbContext(options)
{
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GoodHamburgerDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
