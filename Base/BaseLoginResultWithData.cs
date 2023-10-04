using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseLoginResultWithData<T> : BaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public string RefreshToken { get; set; } = null!;
        /// <summary>
        /// 
        /// </summary>
        public T? Data { get; set; }

    }
}