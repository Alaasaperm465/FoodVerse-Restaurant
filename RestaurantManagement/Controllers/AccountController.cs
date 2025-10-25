using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model;
using RestaurantManagement.ViewModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
    
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser();
                appUser.UserName = model.UserName;
                appUser.Email = model.Email;
                appUser.Address = model.Address;

                var result = await _userManager.CreateAsync(appUser, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(appUser, isPersistent: false);
                    return RedirectToAction("Index", "Category");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                //Check Found User
                ApplicationUser appUser = await _userManager.FindByNameAsync(model.UserName);
                if(appUser != null)
                {
                    bool found = await _userManager.CheckPasswordAsync(appUser, model.Password);
                    if (found)
                    {
                        List<Claim> claims = new List<Claim>
                        {
                            new Claim("Address", appUser.Address ?? "")
                        };
                        await _signInManager.SignInWithClaimsAsync(appUser, isPersistent: model.RememberMe, claims);
                        //await _signInManager.SignInAsync(appUser, isPersistent: model.RememberMe);
                        return RedirectToAction("Index", "Category");
                    }
                }
                ModelState.AddModelError("", "UserName Or Password is Wrong"); 
                
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult addAdmin()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> addAdmin(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser();
                appUser.UserName = model.UserName;
                appUser.Email = model.Email;
                appUser.Address = model.Address;

                var result = await _userManager.CreateAsync(appUser, model.Password);

                if (result.Succeeded)
                {
                    //assign to role
                    await _userManager.AddToRoleAsync(appUser, "Admin");
                    await _signInManager.SignInAsync(appUser, isPersistent: false);
                    return RedirectToAction("Index", "Category");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
    }
}
