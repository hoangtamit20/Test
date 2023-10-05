using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetShop.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage = "{0} không được để trống!")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "{0} phải có ít nhất {2} ký tự và nhiều nhất {1} ký tự.")]
        public string? UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Password { get; set; }
    }
}