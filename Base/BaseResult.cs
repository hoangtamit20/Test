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
        public bool? Result { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<BaseError> Errors { get; set; } = new List<BaseError>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        public void Set(bool result, string message)
        {
            this.Result = result;
            this.Message = message; 
        }
    }
}