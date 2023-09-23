using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetShop.Models
{
    public class LoginResponse
    {
        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "{0} phải có ít nhất {2} ký tự và nhiều nhất {1} ký tự.")]
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}