using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using PetShop.Data;
using serverapi.Base;
using serverapi.Commands.Merchants;
using serverapi.Contants;
using serverapi.Dtos.Merchants;
using serverapi.Entity;

namespace serverapi.Handlers.MerchantHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateMerchantHandler : IRequestHandler<CreateMerchant, BaseResultWithData<MerchantDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly PetShopDbContext _dbContext;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="dbContext"></param>
        public CreateMerchantHandler(UserManager<AppUser> userManager, PetShopDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<BaseResultWithData<MerchantDto>> Handle(CreateMerchant request, CancellationToken cancellationToken)
        {

            var baseResultWithData = new BaseResultWithData<MerchantDto>();
            try{
                // map merchant dto -> merchant entity
                var merchantEntity = request.Adapt<Merchant>();
                _dbContext.Merchants.Add(merchantEntity);
                await _dbContext.SaveChangesAsync();
                // response dataMerchantDto to client
                var dataMerchantDto = request.Adapt<MerchantDto>();
                dataMerchantDto.Id = merchantEntity.Id;
                baseResultWithData.Set(true, MessageContants.OK, dataMerchantDto);
            }catch(Exception ex)
            {
                baseResultWithData.Set(false, MessageContants.Error);
                baseResultWithData.Errors.Add(new BaseError()
                {
                    Code = MessageContants.Exception,
                    Message = ex.Message
                });
            }
            return baseResultWithData;
        }
    }
}