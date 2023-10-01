using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using serverapi.Base;
using serverapi.Dtos.Payments;

namespace serverapi.Commands.Payments
{
    /// <summary>
    /// 
    /// </summary>
    public class CreatePayment : IRequest<BaseResultWithData<PaymentLinkDto>>
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? PaymentRefId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? RequiredAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? PaymentDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ExpireDate { get; set; } = DateTime.Now.AddMinutes(15);

        /// <summary>
        /// 
        /// </summary>
        public string? PaymentLanguage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MerchantId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PaymentDestinationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PaymentId { get; set; }
    }
}