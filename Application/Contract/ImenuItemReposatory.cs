using Application.Contract;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;

namespace Application.Contract
{
    public interface ImenuItemReposatory: IRepository<MenuItem>
    {
         
    }
}
