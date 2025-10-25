//using Microsoft.AspNetCore.Mvc;
//using RestaurantManagement.Models;
////using RestaurantManagement.Application.Contract;
//using Application.Contract;

//namespace RestaurantManagement.Controllers
//{
//    public class OrderItemController : Controller
//    {
//        private readonly IRepository<OrderItem> _orderItemRepo;

//        public OrderItemController(IRepository<OrderItem> orderItemRepo)
//        {
//            _orderItemRepo = orderItemRepo;
//        }

//        public IActionResult Index()
//        {
//            var orderItems = _orderItemRepo.GetAll();
//            return View(orderItems);
//        }

//        public IActionResult Details(int id)
//        {
//            var orderItem = _orderItemRepo.GetById(id);
//            if (orderItem == null)
//                return NotFound();
//            return View(orderItem);
//        }

//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Create(OrderItem orderItem)
//        {
//            if (ModelState.IsValid)
//            {
//                _orderItemRepo.Add(orderItem);
//                _orderItemRepo.Save();
//                return RedirectToAction("Index");
//            }
//            return View(orderItem);
//        }

//        [HttpGet]
//        public IActionResult Edit(int id)
//        {
//            var orderItem = _orderItemRepo.GetById(id);
//            if (orderItem == null)
//                return NotFound();
//            return View(orderItem);
//        }

//        [HttpPost]
//        public IActionResult Edit(OrderItem orderItem)
//        {
//            if (ModelState.IsValid)
//            {
//                _orderItemRepo.Update(orderItem);
//                _orderItemRepo.Save();
//                return RedirectToAction("Index");
//            }
//            return View(orderItem);
//        }

//        public IActionResult Delete(int id)
//        {
//            _orderItemRepo.Delete(id);
//            _orderItemRepo.Save();
//            return RedirectToAction("Index");
//        }
//    }
//}
