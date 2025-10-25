//using Microsoft.AspNetCore.Mvc;
//using System.IO;
//using RestaurantManagement.Context;

//namespace RestaurantManagement.Validations
//{
//    public class Images:Controller
//    {

//        [AcceptVerbs("GET", "POST")]
//        public IActionResult PngImage(string imageUrl)
//        {
//            var extension = Path.GetExtension(imageUrl).ToLower();

//            if (extension != ".png")
//            {
//                return new JsonResult("Only .png images are allowed");
//            }

//            return new JsonResult(true);
//        }
//    }
//}
