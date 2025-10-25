using RestaurantManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IOrderService: IServices<Order>
    {
        ValueTask<Order> CreateOrderAsync(string userId, List<OrderItem> orderItems, string notes = null);
        ValueTask<List<Order>> GetUserOrdersAsync(string userId);
        ValueTask<List<Order>> GetOrdersByStatusAsync(string status);
        ValueTask<Order> GetOrderWithDetailsAsync(int orderId);
        ValueTask<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        ValueTask<bool> CancelOrderAsync(int orderId);
        ValueTask<decimal> CalculateOrderTotalAsync(List<OrderItem> orderItems);
    }
}
