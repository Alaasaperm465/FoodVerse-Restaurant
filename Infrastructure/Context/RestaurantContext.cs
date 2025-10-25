using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model;
using RestaurantManagement.Models;

namespace Application.Context
{
    public class RestaurantContext:IdentityDbContext<ApplicationUser>
    {
        public RestaurantContext(DbContextOptions<RestaurantContext> options)
            : base(options)
        { } 

        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.MenuItems)
                .WithOne(m => m.Category)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
           
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

           
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderDate);

        modelBuilder.Entity<MenuItem>().Property(m => m.Price).HasColumnType("decimal(18,2)");

            
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Pizza", Description = "All types of pizza", DisplayOrder = 1, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 2, Name = "Burgers", Description = "Juicy grilled burgers", DisplayOrder = 2, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 3, Name = "Pasta", Description = "Italian pasta dishes", DisplayOrder = 3, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 4, Name = "Drinks", Description = "Soft drinks and beverages", DisplayOrder = 4, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 5, Name = "Desserts", Description = "Sweet desserts", DisplayOrder = 5, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) }
            );


            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem { Id = 1, Name = "Margherita Pizza", Description = "Classic cheese pizza", Price = 120, CategoryId = 1, IsAvailable = true, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 2, Name = "Pepperoni Pizza", Description = "Spicy pepperoni and cheese", Price = 140, CategoryId = 1, IsAvailable = true, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 3, Name = "Beef Burger", Description = "Grilled beef patty with cheese", Price = 110, CategoryId = 2, IsAvailable = true, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 4, Name = "Chicken Burger", Description = "Grilled chicken sandwich", Price = 100, CategoryId = 2, IsAvailable = true, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 5, Name = "Fettuccine Alfredo", Description = "Creamy pasta with chicken", Price = 130, CategoryId = 3, IsAvailable = true, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 6, Name = "Coca Cola", Description = "Cold drink", Price = 25, CategoryId = 4, IsAvailable = true, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 7, Name = "Chocolate Cake", Description = "Rich chocolate dessert", Price = 70, CategoryId = 5, IsAvailable = true, CreatedAt = new DateTime(2025, 10, 10) }
            );
        }
         
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.UpdatedAt = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }


    }
}

