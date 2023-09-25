using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Models
{
    public class LoginRequest
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
    }
}