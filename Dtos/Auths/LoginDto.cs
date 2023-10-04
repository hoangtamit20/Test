using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Auths
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// 
        /// </summary>
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}