using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Dtos.Orders
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime OrderDate { get; set; } = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        public string ShipName { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string ShipAddress { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string? ShipEmail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShipPhoneNumber { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public List<OrderDetailItemsDto> ListProductOrder { get; set; } = new List<OrderDetailItemsDto>();
    }
}