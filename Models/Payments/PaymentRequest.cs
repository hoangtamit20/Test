using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Models.Payments
{
    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }
}