using Application.Contract;
using Microsoft.EntityFrameworkCore;
//using RestaurantManagement.Context;
using RestaurantManagement.Models;

namespace Application.Contract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        //Task<IEnumerable<Category>> GetActiveCategoriesAsync();

    }
}
