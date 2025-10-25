using Application.Contract;
using RestaurantManagement.Models;


namespace Application.Services
{
    public class OrderItemService:Services<OrderItem>, IOrderItemService
    {
        public OrderItemService(IOrderItemReposatory repo) : base(repo) { }
    }
}
