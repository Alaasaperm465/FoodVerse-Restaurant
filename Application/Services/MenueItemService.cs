using Application.Contract;
using RestaurantManagement.Models;


namespace Application.Services
{
    public class MenuItemService :Services<MenuItem>, IMenuItemService
    {
        public MenuItemService(ImenuItemReposatory repo) : base(repo) { }

    }
}
