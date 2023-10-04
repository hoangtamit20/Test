using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using serverapi.Models;

namespace serverapi.Services.Iservice
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGoogleService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<GoogleResponseModel> GetUserInfoAsync(string token);
    }
}