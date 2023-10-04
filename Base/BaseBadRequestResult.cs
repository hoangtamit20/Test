using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseBadRequestResult
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
    }
}