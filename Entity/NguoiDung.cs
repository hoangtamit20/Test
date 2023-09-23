using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PetShop.Entity
{
    public class NguoiDung : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}