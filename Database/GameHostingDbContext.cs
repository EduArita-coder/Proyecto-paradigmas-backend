using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GAMEHOSTING_APIREST.Entities;

namespace GAMEHOSTING_APIREST.Database
{
    public class GameHostingDbContext : IdentityDbContext<UserEntity>
    {
        public GameHostingDbContext(DbContextOptions<GameHostingDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            builder.Entity<ProductEntity>().HasData(
            new ProductEntity
            {
                Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001"),
                Name = "Minecraft - Plan Básico",
                Description = "Servidor vanilla para grupos pequeños. 2 GB RAM, 10 slots.",
                Price = 4.99m,
                ImageUrl = "/images/minecraft.png",
                Cpu = "Shared vCPU",
                Ram = "2 GB",
                Slots = 10,
                CreatedAt = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductEntity
            {
                Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000002"),
                Name = "Minecraft - Plan Premium",
                Description = "Servidor con mods y plugins. 8 GB RAM, 50 slots.",
                Price = 14.99m,
                ImageUrl = "/images/minecraft.png",
                Cpu = "4 vCPU",
                Ram = "8 GB",
                Slots = 50,
                CreatedAt = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductEntity
            {
                Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000003"),
                Name = "Rust - Plan Estándar",
                Description = "Servidor dedicado para Rust. 8 GB RAM, 100 slots.",
                Price = 19.99m,
                ImageUrl = "/images/rust.png",
                Cpu = "4 vCPU",
                Ram = "8 GB",
                Slots = 100,
                CreatedAt = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)
            },
            new ProductEntity
            {
                Id = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000004"),
                Name = "CS2 - Competitivo",
                Description = "Servidor 128 tick para partidas competitivas. 4 GB RAM.",
                Price = 9.99m,
                ImageUrl = "/images/cs2.png",
                Cpu = "2 vCPU",
                Ram = "4 GB",
                Slots = 16,
                CreatedAt = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)
            }
        );
            SetIdentityTableNames(builder);

            // Nombres de tablas en la base de datos relacionados al proyecto
            builder.Entity<ProductEntity>().ToTable("products");
            builder.Entity<CartItemEntity>().ToTable("cart_items");
            builder.Entity<TransactionEntity>().ToTable("transactions");

            // Configuración de relaciones y claves foráneas
            builder.Entity<CartItemEntity>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TransactionEntity>()
                .HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void SetIdentityTableNames(ModelBuilder builder)
        {
            builder.Entity<UserEntity>().ToTable("users");
            builder.Entity<IdentityRole>().ToTable("roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("users_roles")
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            builder.Entity<IdentityUserClaim<string>>().ToTable("users_claims");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("roles_claims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("users_login");
            builder.Entity<IdentityUserToken<string>>().ToTable("users_tokens");
        }

        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
    }
}