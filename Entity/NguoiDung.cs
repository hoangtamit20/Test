using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PetShop.Entity
{
    public class NguoiDung : IdentityUser
    {
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}