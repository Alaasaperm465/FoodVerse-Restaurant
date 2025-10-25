using Application.Context;
using Application.Contract;
using Application.Services;
using Infrastructure.Repository;
using Infrastructure.Repositry;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;

namespace RestaurantManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

             
            // 1️⃣ تسجيل DbContext
            builder.Services.AddDbContext<RestaurantContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // لو كتيت url مش موجود هيوديني علي الهوم بيج
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Home/Index";
                options.LoginPath = "/Account/Login";
            });

            // 2️⃣ تسجيل Generic Repository
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option=>
            {
                option.Password.RequireUppercase= false;
                option.Password.RequireNonAlphanumeric= false;
            }).AddEntityFrameworkStores<RestaurantContext>();    //it make injection for UserManager and RoleManager
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IServices<>), typeof(Services<>));
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ImenuItemReposatory, MenueItemRepository>();
            builder.Services.AddScoped<IMenuItemService, MenuItemService>();

            builder.Services.AddScoped<IOrderReposatory, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderItemReposatory, OrderItemRepository>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5); 
                options.LoginPath = "/Account/Login";
                options.SlidingExpiration = true;
            });

            var app = builder.Build();
            //app.UseMiddleware<BusinessHoursMiddleware>();



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
