
using Application.Contract;
using RestaurantManagement.Models;

namespace Application.Contract
{
    public interface IOrderReposatory: IRepository<Order>
    {
        ValueTask<List<Order>> GetOrdersByUserIdAsync(string userId);
        ValueTask<List<Order>> GetOrdersByStatusAsync(string status);
        ValueTask<Order> GetOrderWithDetailsAsync(int orderId);
        ValueTask<string> GenerateOrderNumberAsync();
    } 
}
