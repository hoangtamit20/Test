using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using serverapi.Entity;

namespace serverapi.Repository.MerchantRepository
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMerchantRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Merchant>> GetMerchantsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Merchant> GetMerchantById(int id);


    }
}