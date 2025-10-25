using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.ViewModel;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleVM role)
        {
            if (ModelState.IsValid)
            {
                // Save role in database (not implemented here)
                //IdentityRole identityRole = new IdentityRole();
                //identityRole.Name = role.RoleName;
                //await _roleManager.CreateAsync(new IdentityRole(role.RoleName));


                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
                if (result.Succeeded)
                {
                    ViewBag.success = "Role created successfully";
                    return RedirectToAction("AddRole");
                }
                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(role);
        }
    }
}
