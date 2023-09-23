using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Models.Orders
{
    public class OrderRequest
    {
        public string? UserId { get; set; }
        public string? ShippingAddress { get; set; }
        public decimal ShippingCost { get; set; }
        public string? PaymentMethod { get; set; }
        public IEnumerable<OrderDetailRequest>? OrderDetails { get; set; }
    }
}