using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using serverapi.Models;

namespace serverapi.Services.Iservice
{
    public interface IGoogleService
    {
        Task<GoogleResponseModel> GetUserInfoAsync(string token);
    }
}