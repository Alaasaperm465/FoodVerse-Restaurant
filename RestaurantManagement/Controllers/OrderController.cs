using Application.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModel;
namespace RestaurantManagement.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly RestaurantContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(
            RestaurantContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User) // ✅ تضمين بيانات المستخدم
                .Where(o => o.OrderItems.Any())
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var viewModel = new OrderDetailsVM
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                UserName = order.User?.UserName,
                UserEmail = order.User?.Email,
                UserAddress = order.User?.Address,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                Notes = order.Notes
            };

            return View(viewModel);
        }
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // ✅ الحصول على الطلبات النشطة التي تحتوي على عناصر
            var activeOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.UserId == user.Id &&
                            o.Status != OrderStatus.Completed.ToString() &&
                            o.Status != OrderStatus.Cancelled.ToString() &&
                            o.OrderItems.Any()) // ✅ شرط إضافي: يجب أن تحتوي على عناصر
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // ✅ حذف الطلبات الفارغة تلقائياً
            var emptyOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == user.Id &&
                            o.Status != OrderStatus.Completed.ToString() &&
                            o.Status != OrderStatus.Cancelled.ToString() &&
                            !o.OrderItems.Any()) // ✅ الطلبات التي لا تحتوي على عناصر
                .ToListAsync();

            if (emptyOrders.Any())
            {
                _context.Orders.RemoveRange(emptyOrders);
                await _context.SaveChangesAsync();
            }

            if (!activeOrders.Any())
                return View("EmptyCart");

            // ✅ حساب الإجمالي لكل طلب
            foreach (var order in activeOrders)
            {
                var total = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
                var discountResult = ApplyDiscounts(total);
                order.TotalPrice = discountResult.FinalTotal;
                order.DiscountAmount = discountResult.DiscountAmount;
                order.DiscountDescription = discountResult.DiscountDescription;

                _context.Update(order);
            }

            await _context.SaveChangesAsync();
            return View(activeOrders);
        }
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            var myOrders = await _context.Orders
                .Where(o => o.UserId == user.Id && o.OrderItems.Any()) // ✅ فقط الطلبات التي تحتوي على عناصر
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(myOrders);
        }
        public async Task<IActionResult> AddToCart(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null || !menuItem.IsAvailable)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            var pendingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Status == "Pending" && o.UserId == user.Id);

            if (pendingOrder == null)
            {
                pendingOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    TotalPrice = 0,
                    UserId = user.Id,
                    OrderNumber = Guid.NewGuid().ToString().Substring(0, 8)
                };

                _context.Orders.Add(pendingOrder);
                await _context.SaveChangesAsync();
            }

            var existingItem = pendingOrder.OrderItems.FirstOrDefault(oi => oi.MenuItemId == id);

            if (existingItem != null)
            {
                // ✅ تحقق من الكمية المتاحة
                if (existingItem.Quantity + 1 > menuItem.Instoke)
                {
                    TempData["ErrorMessage"] = $"❌ Only {menuItem.Instoke} available in stock!";
                    return RedirectToAction("Cart");
                }

                existingItem.Quantity++;
            }
            else
            {
                // ✅ تحقق من المخزون قبل إضافة العنصر لأول مرة
                if (menuItem.Instoke < 1)
                {
                    TempData["ErrorMessage"] = "❌ This item is out of stock!";
                    return RedirectToAction("Cart");
                }

                var orderItem = new OrderItem
                {
                    OrderId = pendingOrder.Id,
                    MenuItemId = id,
                    Quantity = 1,
                    UnitPrice = menuItem.Price
                };
                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            await UpdateOrderTotal(pendingOrder.Id);

            return RedirectToAction("Cart");
        }
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .FirstOrDefaultAsync(oi => oi.Id == id);

            if (orderItem != null)
            {
                // ✅ تحقق من الكمية قبل الزيادة
                if (orderItem.Quantity + 1 > orderItem.MenuItem.Instoke)
                {
                    TempData["ErrorMessage"] = $"❌ Only {orderItem.MenuItem.Instoke} available in stock!";
                    return RedirectToAction("Cart");
                }

                orderItem.Quantity++;
                await _context.SaveChangesAsync();
                await UpdateOrderTotal(orderItem.OrderId);
            }
            return RedirectToAction("Cart");
        }
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.OrderItems)
                .FirstOrDefaultAsync(oi => oi.Id == id);

            if (orderItem != null)
            {
                var order = orderItem.Order;

                if (orderItem.Quantity > 1)
                {
                    orderItem.Quantity--;
                    await _context.SaveChangesAsync();
                    await UpdateOrderTotal(orderItem.OrderId);
                }
                else
                {
                    _context.OrderItems.Remove(orderItem);
                    await _context.SaveChangesAsync();

                    // ✅ تحقق إذا أصبح الطلب فارغاً
                    var updatedOrder = await _context.Orders
                        .Include(o => o.OrderItems)
                        .FirstOrDefaultAsync(o => o.Id == order.Id);

                    if (updatedOrder != null && !updatedOrder.OrderItems.Any())
                    {
                        _context.Orders.Remove(updatedOrder);
                        await _context.SaveChangesAsync();
                        TempData["InfoMessage"] = "Order removed as it became empty.";
                        return RedirectToAction("Cart");
                    }
                    else
                    {
                        await UpdateOrderTotal(orderItem.OrderId);
                    }
                }
            }
            return RedirectToAction("Cart");
        }
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .ThenInclude(o => o.OrderItems)
                .FirstOrDefaultAsync(oi => oi.Id == id);

            if (orderItem != null)
            {
                var order = orderItem.Order;
                var orderId = orderItem.OrderId;

                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();

                // ✅ إذا كانت الطلب أصبح فارغاً بعد الحذف، احذف الطلب نفسه
                var updatedOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (updatedOrder != null && !updatedOrder.OrderItems.Any())
                {
                    _context.Orders.Remove(updatedOrder);
                    await _context.SaveChangesAsync();
                    TempData["InfoMessage"] = "Order removed as it became empty.";
                }
                else
                {
                    await UpdateOrderTotal(orderId);
                }
            }

            return RedirectToAction("Cart");
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> ConfirmOrder(int id, string customerName, string orderType, string? deliveryAddress)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found!";
                return RedirectToAction("Cart");
            }

            if (order.Status != OrderStatus.Pending.ToString())
            {
                TempData["InfoMessage"] = "Order already confirmed.";
                return RedirectToAction("MyOrders");
            }

            // ✅ تحويل الـ orderType النصي إلى enum
            if (!Enum.TryParse<OrderType>(orderType, out var parsedOrderType))
            {
                TempData["ErrorMessage"] = "Invalid order type.";
                return RedirectToAction("Cart");
            }

            // ✅ تحديث بيانات الطلب مع حفظ اسم العميل
            order.Status = OrderStatus.Confirmed.ToString();
            order.OrderType = parsedOrderType;
            order.DeliveryAddress = parsedOrderType == OrderType.Delivery ? deliveryAddress : null;
            order.OrderDate = DateTime.Now;

            // ✅ حفظ اسم العميل في الـ Notes
            order.Notes = $"Customer Name: {customerName}";

            // حساب وقت التوصيل المتوقع لو الطلب Delivery
            if (parsedOrderType == OrderType.Delivery)
            {
                int maxPrepTime = order.OrderItems.Any()
                    ? order.OrderItems.Max(i => i.MenuItem.PreparationTime)
                    : 0;

                order.EstimatedDeliveryTime = DateTime.Now.AddMinutes(maxPrepTime + 30);
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Your order has been confirmed successfully! Order #: {order.OrderNumber}";
            return RedirectToAction("OrderConfirmed");
        }
        private (decimal FinalTotal, decimal DiscountAmount, string DiscountDescription) ApplyDiscounts(decimal total)
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var discountAmount = 0m;
            var description = "";

            var startHappyHour = new TimeSpan(15, 0, 0);
            var endHappyHour = new TimeSpan(17, 0, 0);

            if (currentTime >= startHappyHour && currentTime <= endHappyHour)
            {
                var discount = total * 0.20m;
                total -= discount;
                discountAmount += discount;
                description += "Happy Hour 20% off";
            }
            if (total > 100)
            {
                var discount = total * 0.10m;
                total -= discount;
                discountAmount += discount;
                description += string.IsNullOrEmpty(description)
                    ? "Bulk Discount 10% off"
                    : " + Bulk Discount 10% off";
            }

            return (total, discountAmount, description);
        }
        public IActionResult OrderConfirmed()
        {
            return View();
        }
        private async Task UpdateOrderTotal(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                // ✅ إذا لم يكن هناك عناصر، احذف الطلب
                if (!order.OrderItems.Any())
                {
                    _context.Orders.Remove(order);
                }
                else
                {
                    order.TotalPrice = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
                }
                await _context.SaveChangesAsync();
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
                return NotFound();

            // منع الإلغاء لو الحالة Ready أو Completed (Delivered)
            if (order.Status == OrderStatus.Ready.ToString() || order.Status == OrderStatus.Completed.ToString())
            {
                TempData["ErrorMessage"] = "You cannot cancel an order that is Ready or Delivered.";
                return RedirectToAction("MyOrders");
            }

            if (order.Status != OrderStatus.Pending.ToString())
            {
                TempData["ErrorMessage"] = "You can only delete orders that are still pending.";
                return RedirectToAction("MyOrders");
            }

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction("MyOrders");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            if (!Enum.IsDefined(typeof(OrderStatus), newStatus))
                return BadRequest("Invalid status");

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
