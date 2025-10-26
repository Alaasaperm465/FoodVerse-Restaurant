using Application.Contract;
using RestaurantManagement.Models;
using System.Linq;

namespace Application.Services
{
    public class OrderService : Services<Order>, IOrderService
    {
        private readonly IOrderReposatory _orderRepo;
        private readonly IMenuItemService _menuItemService;

        public OrderService(IOrderReposatory orderRepo, IMenuItemService menuItemService)
            : base(orderRepo)
        {
            _orderRepo = orderRepo;
            _menuItemService = menuItemService;
        }

        public async ValueTask<Order> CreateOrderAsync(string userId, List<OrderItem> orderItems, string notes = null, OrderType orderType = OrderType.DineIn, string deliveryAddress = null)
        {
            if (string.IsNullOrEmpty(userId) || orderItems == null || !orderItems.Any())
                return null;

            // إذا النوع Delivery لازم عنوان
            if (orderType == OrderType.Delivery && string.IsNullOrWhiteSpace(deliveryAddress))
                return null;

            foreach (var item in orderItems)
            {
                var menuItem = await _menuItemService.GetByIdAsync(item.MenuItemId);
                if (menuItem == null || !menuItem.IsAvailable)
                    return null;

                if (menuItem.Instoke < item.Quantity)
                    return null;

                item.UnitPrice = menuItem.Price;
            }

            var totalPrice = await CalculateOrderTotalAsync(orderItems);
            var discountResult = ApplyDiscounts(totalPrice);
            totalPrice = discountResult.FinalTotal;
            var discountAmount = discountResult.DiscountAmount;
            var discountDescription = discountResult.DiscountDescription;

            // حساب أقصى وقت تحضير من المينيو آيتمز
            int maxPrepMinutes = 0;
            foreach (var it in orderItems)
            {
                var menuItem = await _menuItemService.GetByIdAsync(it.MenuItemId);
                // افتراض: حقل PreparationTimeMinutes موجود في MenuItem (غير الاسم لو عندك اسم مختلف)
                var prep = 0;
                if (menuItem != null)
                {
                    // حاول قراءة حقل PreparationTimeMinutes أو أي اسم عندك
                    // إذا حقل غير موجود اختر 0
                    prep = menuItem.PreparationTime; 
                }

                if (prep > maxPrepMinutes) maxPrepMinutes = prep;
            }

            DateTime? estimatedDeliveryAt = null;
            if (orderType == OrderType.Delivery)
            {
                // estimated = now + maxPrep + 30 minutes
                estimatedDeliveryAt = DateTime.UtcNow.AddMinutes(maxPrepMinutes + 30);
            }

            var order = new Order
            {
                UserId = userId,
                OrderNumber = await _orderRepo.GenerateOrderNumberAsync(),
                OrderDate = DateTime.UtcNow,
                TotalPrice = totalPrice,
                DiscountAmount = discountAmount,
                DiscountDescription = discountDescription,
                Status = OrderStatus.Pending.ToString(),
                Notes = notes,
                OrderItems = orderItems,
                OrderType = orderType,
                DeliveryAddress = orderType == OrderType.Delivery ? deliveryAddress : null,
            };
            var createdOrder = await _orderRepo.AddAsync(order);

            if (createdOrder != null)
            {
                foreach (var item in orderItems)
                {
                    var menuItem = await _menuItemService.GetByIdAsync(item.MenuItemId);
                    menuItem.Instoke -= item.Quantity;
                    await _menuItemService.UpdateAsync(menuItem);
                }
            }

            return createdOrder;
        }

        private (decimal FinalTotal, decimal DiscountAmount, string DiscountDescription) ApplyDiscounts(decimal total)
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var discountAmount = 0m;
            var description = "";

            // Happy Hour
            var startHappyHour = new TimeSpan(15, 0, 0);
            var endHappyHour = new TimeSpan(17, 0, 0);

            if (currentTime >= startHappyHour && currentTime <= endHappyHour)
            {
                var discount = total * 0.20m;
                total -= discount;
                discountAmount += discount;
                description += "Happy Hour 20% off";
            }

            //  Bulk Discount
            if (total > 100)
            {
                var discount = total * 0.10m;
                total -= discount;
                discountAmount += discount;
                description += string.IsNullOrEmpty(description) ? "Bulk Discount 10% off" : " + Bulk Discount 10% off";
            }

            return (total, discountAmount, description);
        }
        public async ValueTask<List<Order>> GetUserOrdersAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Order>();

            return await _orderRepo.GetOrdersByUserIdAsync(userId);
        }

        public async ValueTask<List<Order>> GetOrdersByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
                return await GetAllAsync();

            return await _orderRepo.GetOrdersByStatusAsync(status);
        }

        public async ValueTask<Order> GetOrderWithDetailsAsync(int orderId)
        {
            return await _orderRepo.GetOrderWithDetailsAsync(orderId);
        }
        public async ValueTask<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
                return false;

            var validStatuses = new[]
            {
                OrderStatus.Pending.ToString(),
                OrderStatus.Confirmed.ToString(),
                OrderStatus.Preparing.ToString(),
                OrderStatus.Ready.ToString(),
                OrderStatus.Completed.ToString(),
                OrderStatus.Cancelled.ToString()
            };

            if (!validStatuses.Contains(newStatus))
                return false;

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.UpdateAsync(order);

            return true;
        }
        public async ValueTask<bool> CancelOrderAsync(int orderId)
        {
            var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return false;

            // لا يمكن الإلغاء لو جاهز أو مكتمل (Delivered ~= Completed)
            if (order.Status == OrderStatus.Ready.ToString() || order.Status == OrderStatus.Completed.ToString())
                return false;

            if (order.OrderItems != null && order.OrderItems.Any())
            {
                foreach (var item in order.OrderItems)
                {
                    var menuItem = await _menuItemService.GetByIdAsync(item.MenuItemId);
                    if (menuItem != null)
                    {
                        menuItem.Instoke += item.Quantity;
                        await _menuItemService.UpdateAsync(menuItem);
                    }
                }
            }

            order.Status = OrderStatus.Cancelled.ToString();
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.UpdateAsync(order);

            return true;
        }

        public async ValueTask<decimal> CalculateOrderTotalAsync(List<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
                return 0;

            decimal total = 0;
            foreach (var item in orderItems)
            {
                total += item.UnitPrice * item.Quantity;
            }

            return total;
        }
    }
}