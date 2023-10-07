using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Orders
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderDetailItemsDto
    {
        /// <summary>
        /// ProductID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Quantity of Product
        /// </summary>
        public int Quantity { get; set; }
    }
}