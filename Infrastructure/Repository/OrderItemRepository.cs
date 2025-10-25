using Application.Context;
using Infrastructure.Repository;
using RestaurantManagement.Models;
using Infrastructure.Repositry;
using Application.Contract;

namespace Infrastructure.Repositry
{
    public class OrderItemRepository : Repository<OrderItem>,IOrderItemReposatory
    {
        public OrderItemRepository(RestaurantContext context) : base(context) { }
    }
}
