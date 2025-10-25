using Application.Contract;
using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : Services<Category>, ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo) : base(repo)
        {
            _repo = repo;
        }

        public override async ValueTask<List<Category>> GetAllAsync()
        {
            var allCategories = await _repo.GetAllAsync();
            var categoriesWithItems = allCategories
                //.Where(c => c.MenuItems != null && c.MenuItems.Any())
                .ToList();
            return categoriesWithItems;
        }
    }
}
