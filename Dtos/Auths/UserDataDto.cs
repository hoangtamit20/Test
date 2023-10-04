using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Auths
{
    /// <summary>
    /// 
    /// </summary>
    public class UserDataDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}