using Application.Context;
using Application.Contract;
//using Infrastructure.Migrations;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;

namespace Infrastructure.Repositry
{

    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly RestaurantContext _context;

        public CategoryRepository(RestaurantContext context) : base(context)
        {
            _context = context;
        }

        public override async ValueTask<List<Category>> GetAllAsync()
        {
            return await _context.Categories
        .Include(c => c.MenuItems.Where(m => !m.IsDeleted)) 
        .ToListAsync();
        }
    }
}
