using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseResultBadRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
    }
}