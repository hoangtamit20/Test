using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using serverapi.Entity;

namespace PetShop.Entity
{
    public class NguoiDung : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        [InverseProperty("UserIdNavigation")]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}