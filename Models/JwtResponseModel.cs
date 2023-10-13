using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetShop.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtResponseModel<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public string RefreshToken { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public T? Data { get; set; }
    }
}