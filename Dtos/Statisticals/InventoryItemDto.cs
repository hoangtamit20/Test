using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Statisticals
{
    /// <summary>
    /// 
    /// </summary>
    public class InventoryItemDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public int Stock { get; set; }
    }
}