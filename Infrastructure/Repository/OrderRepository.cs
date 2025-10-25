using Application.Context;
using Application.Contract;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;

namespace Infrastructure.Repositry
{
    public class OrderRepository : Repository<Order>, IOrderReposatory
    {
        private readonly RestaurantContext _context;

        public OrderRepository(RestaurantContext context) : base(context)
        {
            _context = context;
        }

        public override async ValueTask<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public override async ValueTask<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Category)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        public async ValueTask<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async ValueTask<List<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Status == status && !o.IsDeleted)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async ValueTask<Order> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Category)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);
        }

        public async ValueTask<string> GenerateOrderNumberAsync()
        {
            var lastOrder = await _context.Orders
                .OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
                return $"ORD-{DateTime.UtcNow:yyyyMMdd}-0001";

            var lastNumber = lastOrder.OrderNumber.Split('-').Last();
            var newNumber = int.Parse(lastNumber) + 1;

            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{newNumber:D4}";
        }
    }
}
