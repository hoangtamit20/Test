using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetShop.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class NguoiDungRegisterModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string? Password { get; set; }
    }
}