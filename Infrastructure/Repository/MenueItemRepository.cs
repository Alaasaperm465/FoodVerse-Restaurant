using Application.Context;
using Application.Contract;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;

namespace Infrastructure.Repositry
{
    public class MenueItemRepository: Repository<MenuItem>,ImenuItemReposatory
    {
        //public MenueItemRepository(RestaurantContext context) : base(context) { }
        private readonly RestaurantContext _context;

        public MenueItemRepository(RestaurantContext context) : base(context)
        {
            _context = context;
        }

        public override async ValueTask<List<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Category) 
                .Where(m => !m.IsDeleted)
                .ToListAsync();
        }

        public override async ValueTask<MenuItem> GetByIdAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
