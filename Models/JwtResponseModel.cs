using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetShop.Models
{
    public class JwtResponseModel
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public object? Data { get; set; }
        public bool Result { get; set; }
        public List<string>? Errors { get; set; }
    }
}