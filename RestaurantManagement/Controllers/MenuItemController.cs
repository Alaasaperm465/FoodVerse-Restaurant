using Application.Context;
using Application.Contract;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model;
using RestaurantManagement.Models;
using RestaurantManagement.ViewModel;

namespace RestaurantManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MenuItemController : Controller 
    {
        private readonly IMenuItemService _menuService;
        private readonly ICategoryService _categoryService;
        private readonly RestaurantContext _context;

        public MenuItemController(IMenuItemService menuService, ICategoryService categoryService, RestaurantContext context)
        {
            _menuService = menuService;
            _categoryService = categoryService;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var menuItems = await _menuService.GetAllAsync();
            var itemsInStock = menuItems.Where(mi => mi.Instoke > 0).ToList();

            //var itemDiscount = await _context.ite;
            //ViewBag["name"] = itemDiscount.Name;

            return View(menuItems);
        }
        public async Task<IActionResult> Details(int id)
        {
            var item = await _menuService.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var cats = await _categoryService.GetAllAsync();
            MenuItemVM vm = new()
            {
                Categories = new SelectList(cats, "Id", "Name")
            };

            return View(vm);
        }

  
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveCreate(MenuItemVM model)
        {
            if (!ModelState.IsValid)
            {
                var cats = await _categoryService.GetAllAsync();
                model.Categories = new SelectList(cats, "Id", "Name");
                return View("Create", model);
            }

            string imagePath = null;

            // 👇 لو الصورة اترفعت
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/menu");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // المسار النسبي للموقع
                imagePath = "/images/menu/" + fileName;
            }

            MenuItem menuItem = new()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                IsAvailable = model.IsAvailable,
                ImageUrl = imagePath,
                PreparationTime = model.PreparationTime,
                Instoke = model.Instoke,
                CategoryId = model.CategoryId
            };

            await _menuService.CreateAsync(menuItem);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var menuItem = await _menuService.GetByIdAsync(id);
            if (menuItem == null)
                return NotFound();

            var viewModel = new MenuItemVM
            {
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                ImageUrl = menuItem.ImageUrl,
                PreparationTime = menuItem.PreparationTime,
                Instoke = menuItem.Instoke,
                CategoryId = menuItem.CategoryId,
                Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", menuItem.CategoryId)
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuItemVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", model.CategoryId);
                return View(model);
            }

            var menuItem = await _menuService.GetByIdAsync(id);
            if (menuItem == null)
                return NotFound();

            // 🔹 لو المستخدم رفع صورة جديدة
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // حذف الصورة القديمة من المجلد (لو موجودة)
                if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", menuItem.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/menu");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                menuItem.ImageUrl = "/images/menu/" + fileName;
            }

            menuItem.Name = model.Name;
            menuItem.Description = model.Description;
            menuItem.Price = model.Price;
            menuItem.IsAvailable = model.IsAvailable;
            menuItem.PreparationTime = model.PreparationTime;
            menuItem.Instoke = model.Instoke;
            menuItem.CategoryId = model.CategoryId;

            await _menuService.UpdateAsync(menuItem);

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Delete(int id)
        {
            await _menuService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
