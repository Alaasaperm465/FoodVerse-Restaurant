using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ApplicationUser:IdentityUser
    {
        public string? Address { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
