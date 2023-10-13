using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        public bool? Success { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public string? Message { get; set; }
    }
}