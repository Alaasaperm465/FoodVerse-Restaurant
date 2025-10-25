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
                new Category { Id = 101, Name = "Burgers", Description = "Juicy grilled burgers with fresh ingredients.", DisplayOrder = 1, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 102, Name = "Sandwiches", Description = "Tasty sandwiches made with quality bread and fillings.", DisplayOrder = 2, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 103, Name = "Steaks", Description = "Premium beef steaks cooked to perfection.", DisplayOrder = 3, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 104, Name = "Chicken Meals", Description = "Crispy and grilled chicken dishes served hot.", DisplayOrder = 4, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 105, Name = "Desserts", Description = "Sweet treats and baked delights to end your meal.", DisplayOrder = 5, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) },
                new Category { Id = 106, Name = "Drinks", Description = "Refreshing cold and hot beverages for every taste.", DisplayOrder = 6, IsActive = true, CreatedAt = new DateTime(2025, 10, 10) }
            );


            modelBuilder.Entity<MenuItem>().HasData(
                // Burgers
                new MenuItem { Id = 201, Name = "Classic Beef Burger", Description = "Grilled beef patty with cheese and lettuce.", Price = 110, CategoryId = 101, ImageUrl = "/images/img_1.jpg", IsAvailable = true, Instoke = 25, PreparationTime = 15, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 202, Name = "Cheese Burger", Description = "Beef burger with double cheese.", Price = 120, CategoryId = 101, ImageUrl = "/images/img_22", IsAvailable = true, Instoke = 20, PreparationTime = 15, CreatedAt = new DateTime(2025, 10, 10) },

                // Sandwiches
                new MenuItem { Id = 203, Name = "Club Sandwich", Description = "Triple-layer sandwich with chicken and bacon.", Price = 90, CategoryId = 102, ImageUrl = "/images/img_3.jpg", IsAvailable = true, Instoke = 30, PreparationTime = 10, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 204, Name = "Tuna Sandwich", Description = "Fresh tuna with lettuce and mayo.", Price = 85, CategoryId = 102, ImageUrl = "/images/img_4.jpg", IsAvailable = true, Instoke = 30, PreparationTime = 10, CreatedAt = new DateTime(2025, 10, 10) },

                // Steaks
                new MenuItem { Id = 205, Name = "Ribeye Steak", Description = "Tender and juicy ribeye grilled to perfection.", Price = 250, CategoryId = 103, ImageUrl = "/images/img_5.jpg", IsAvailable = true, Instoke = 10, PreparationTime = 25, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 206, Name = "T-Bone Steak", Description = "Classic T-bone steak with herbs.", Price = 270, CategoryId = 103, ImageUrl = "/images/img_6.jpg", IsAvailable = true, Instoke = 8, PreparationTime = 25, CreatedAt = new DateTime(2025, 10, 10) },

                // Chicken Meals
                new MenuItem { Id = 207, Name = "Grilled Chicken", Description = "Grilled chicken breast with seasoning.", Price = 130, CategoryId = 104, ImageUrl = "/images/img_7.jpg", IsAvailable = true, Instoke = 15, PreparationTime = 20, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 208, Name = "Fried Chicken", Description = "Crispy fried chicken pieces.", Price = 120, CategoryId = 104, ImageUrl = "/images/img_8.jpg", IsAvailable = true, Instoke = 18, PreparationTime = 20, CreatedAt = new DateTime(2025, 10, 10) },

                // Desserts
                new MenuItem { Id = 209, Name = "Chocolate Cake", Description = "Rich chocolate layered cake.", Price = 70, CategoryId = 105, ImageUrl = "/images/img_9.jpg", IsAvailable = true, Instoke = 10, PreparationTime = 5, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 210, Name = "Cheesecake", Description = "Creamy baked cheesecake with topping.", Price = 80, CategoryId = 105, ImageUrl = "/images/img_10.jpg", IsAvailable = true, Instoke = 8, PreparationTime = 5, CreatedAt = new DateTime(2025, 10, 10) },

                // Drinks
                new MenuItem { Id = 211, Name = "Coca-Cola", Description = "Cold refreshing soda.", Price = 25, CategoryId = 106, ImageUrl = "/images/img_11.jpg", IsAvailable = true, Instoke = 50, PreparationTime = 1, CreatedAt = new DateTime(2025, 10, 10) },
                new MenuItem { Id = 212, Name = "Latte", Description = "Hot milk coffee drink.", Price = 45, CategoryId = 106, ImageUrl = "/images/img_12.jpg", IsAvailable = true, Instoke = 20, PreparationTime = 5, CreatedAt = new DateTime(2025, 10, 10) }
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

