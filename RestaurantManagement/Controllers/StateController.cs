using Microsoft.AspNetCore.Mvc;

namespace RestaurantManagement.Controllers
{
    public class StateController : Controller
    {
        public IActionResult SetTemp()
        {
            TempData["Name"] = "Alaa";
            TempData["Age"] = 20;
            return View();
        }
        public IActionResult GetTemp() 
        {
            var name = TempData.Peek("Name");
            var age = TempData.Peek("Age");
            return View(name);

        }

        //public IActionResult SetSession()
        //{
        //    HttpContext.Session.SetString("Name", "alaa");
        //    HttpContext.Session.SetInt32("Age", 24);
        //    return View();
        //}
        //public IActionResult GetSession()
        //{
        //    ViewBag.Name = HttpContext.Session.GetString("Name");
        //    ViewBag.Age = HttpContext.Session.GetInt32("Age");
        //    return View();
        //}
        public IActionResult SetCookie()
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(10), 
                HttpOnly = true, 
                Secure = false 
            };

            Response.Cookies.Append("Name", "Alaa", options);
            Response.Cookies.Append("Age", "24", options);

            return View();
        }
        public IActionResult GetCookie()
        {
            var name = Request.Cookies["Name"];
            var age = Request.Cookies["Age"];

            ViewBag.Name = name;
            ViewBag.Age = age;

            return View();
        }

    }
}
