using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Carts
{
    /// <summary>
    /// 
    /// </summary>
    public class CartResponseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int CartId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Quantity { get; set; }

    }
}