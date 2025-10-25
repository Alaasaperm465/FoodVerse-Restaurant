using Application.Services;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Models;
using System.Diagnostics;

namespace RestaurantManagement.Controllers;

public class HomeController : Controller
{
    //private readonly ILogger<HomeController> _logger;
    private readonly IMenuItemService _menuService;
    private readonly ICategoryService _categoryService;

    public HomeController(IMenuItemService menuService, ICategoryService categoryService)
    {
        _menuService = menuService;
        _categoryService = categoryService;
    }

    //public HomeController(ILogger<HomeController> logger)
    //{
    //    _logger = logger;
    //}

    public async Task<IActionResult> Index()
    {
        var menuItems = await _menuService.GetAllAsync();
        return View(menuItems);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
