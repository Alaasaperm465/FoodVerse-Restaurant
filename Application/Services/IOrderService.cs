using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IOrderService : IServices<Order>
    {
        ValueTask<Order> CreateOrderAsync(
            string userId,
            List<OrderItem> orderItems,
            string notes = null,
            OrderType orderType = OrderType.DineIn,
            string deliveryAddress = null
        );

        ValueTask<List<Order>> GetUserOrdersAsync(string userId);
        ValueTask<List<Order>> GetOrdersByStatusAsync(string status);
        ValueTask<Order> GetOrderWithDetailsAsync(int orderId);
        ValueTask<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        ValueTask<bool> CancelOrderAsync(int orderId);
        ValueTask<decimal> CalculateOrderTotalAsync(List<OrderItem> orderItems);
    }
}
