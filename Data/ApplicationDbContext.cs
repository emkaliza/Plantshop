using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlantShop.Models;

namespace Plantshop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Plant>()
               .HasOne(p => p.Discount)
               .WithMany(d => d.Plants)
               .HasForeignKey(p => p.DiscountId);

            // Налаштування точності для decimal полів
            modelBuilder.Entity<Plant>()
                .Property(p => p.BasePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Plant>()
                .Property(p => p.DiscountPrice)
                .HasColumnType("decimal(18,2)");

            // Plant to Category relationship
            modelBuilder.Entity<Plant>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Plants)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cart relationships
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Plant)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite relationships
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Plant)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.Property(d => d.DiscountPercentage)
                    .HasColumnType("decimal(5,2)");

                entity.Property(d => d.FixedDiscountAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(d => d.MinimumPurchaseAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(d => d.MaximumDiscountAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(d => d.Code)
                    .HasMaxLength(50);

                entity.Property(d => d.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });
            // Indexes
            modelBuilder.Entity<Plant>()
            .HasIndex(p => p.Name);

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate);

            modelBuilder.Entity<Discount>()
                .HasIndex(d => d.Code)
                .IsUnique();

            // Decimal precision
            modelBuilder.Entity<Plant>()
                .Property(p => p.BasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.PriceAtTime)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Discount>()
                .Property(d => d.MinimumPurchaseAmount)
                .HasPrecision(18, 2);
        }
    }
}
