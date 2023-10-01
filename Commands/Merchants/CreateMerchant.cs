using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using serverapi.Base;
using serverapi.Dtos.Merchants;

namespace serverapi.Commands.Merchants
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateMerchant : IRequest<BaseResultWithData<MerchantDto>>
    {
        /// <summary>
        /// 
        /// </summary>
        public string? MerchantName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? MerchantWebLink { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? MerchantIpnUrl { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? MerchantReturnUrl { get; set; } = string.Empty;
    }
}