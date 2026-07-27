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