using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serverapi.Services.PagingAndFilterService
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryFilter : PaginationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Filter { get; set; }
    }
}