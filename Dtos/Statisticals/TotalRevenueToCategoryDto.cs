using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Statisticals
{
    /// <summary>
    /// 
    /// </summary>
    public class TotalRevenueToCategoryDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public decimal TotalRevenue { get; set; }
    }
}