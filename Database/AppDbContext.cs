using Microsoft.EntityFrameworkCore;
using GAMEHOSTING_APIREST.Entities;

namespace GAMEHOSTING_APIREST.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001"),
                Name = "Minecraft - Plan Básico",
                Description = "Servidor vanilla para grupos pequeños. 2 GB RAM, 10 slots.",
                Price = 4.99m,
                ImageUrl = "/images/Estandar.png",
                Cpu = "Shared vCPU",
                Ram = "2 GB",
                Slots = 10
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000002"),
                Name = "Minecraft - Plan Premium",
                Description = "Servidor con mods y plugins. 8 GB RAM, 50 slots.",
                Price = 14.99m,
                ImageUrl = "/images/Premium.png",
                Cpu = "4 vCPU",
                Ram = "8 GB",
                Slots = 50
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000003"),
                Name = "Rust - Plan Estándar",
                Description = "Servidor dedicado para Rust. 8 GB RAM, 100 slots.",
                Price = 19.99m,
                ImageUrl = "/images/rust.png",
                Cpu = "4 vCPU",
                Ram = "8 GB",
                Slots = 100
            },
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000004"),
                Name = "CS2 - Competitivo",
                Description = "Servidor 128 tick para partidas competitivas. 4 GB RAM.",
                Price = 9.99m,
                ImageUrl = "/images/cs2.png",
                Cpu = "2 vCPU",
                Ram = "4 GB",
                Slots = 16
            }
        );
    }
}
