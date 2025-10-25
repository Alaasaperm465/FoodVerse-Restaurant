using RestaurantManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICategoryService : IServices<Category>
    {
        //Task<string?> GetAllAsync();
    }
}
