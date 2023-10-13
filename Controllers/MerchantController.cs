using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos.Merchants;
using serverapi.Entity;

namespace PetShop.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        private readonly PetShopDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public MerchantController(PetShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list merchant (Authorize)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // [Authorize]
        [ProducesResponseType(typeof(BaseResultWithData<List<MerchantInfoDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetMerchants()
        {
            if (_context.Merchants == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){"DB is null!"}});
            }
            var listMerchant = await _context.Merchants.ToListAsync();
            var result = new BaseResultWithData<List<MerchantInfoDto>>()
            {
                Success = true,
                Message = "List of Merchant",
                Data = listMerchant.Adapt<List<MerchantInfoDto>>()
            };
            return  Ok(result);
        }

        /// <summary>
        /// Get Merchant by Id (Authorize)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Merchant/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResultWithData<MerchantInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetMerchantById(int id)
        {
            if (_context.Merchants == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){"DB is null!"}});
            }
            var merchant = await _context.Merchants.FindAsync(id);

            if (merchant == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){$"Merchant with Id : {id} not found!"}});
            }

            return Ok(merchant.Adapt<MerchantInfoDto>());
        }

        /// <summary>
        /// Update merchant (Authorize)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateMerchantDto"></param>
        /// <returns></returns>
        // PUT: api/Merchant/5
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdatePaymentDestination(int id, UpdateMerchantDto updateMerchantDto)
        {
            if (id != updateMerchantDto.Id)
            {
                return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"Route Id : {id} is in valid with PaymentDestination Id : {updateMerchantDto.Id}"}});
            }
            _context.Entry(updateMerchantDto.Adapt<Merchant>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchantExists(id))
                {
                    return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){$"Update Merchant with Id : {id} failed"}});
                }
                else
                {
                    return StatusCode(500, new BaseBadRequestResult(){Errors = new List<string>(){"Internal Server Error!"}});
                }
            }
        }
        
        /// <summary>
        /// Set active merchant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="activeMerchantDto"></param>
        /// <returns></returns>
        /// <remarks>
        ///     PUT : set-active-merchant
        /// {
        /// }
        /// </remarks>
        [HttpPut]
        [Route("{id}/set-active-merchant")]
        [Authorize]
        public async Task<IActionResult> SetActiveMerchant(int id, ActiveMerchantDto activeMerchantDto)
        {
            if (id != activeMerchantDto.Id)
            {
                return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"Route Id : {id} is in valid with PaymentDestination Id : {activeMerchantDto.Id}"}});
            }
            _context.Entry(activeMerchantDto.Adapt<Merchant>()).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchantExists(id))
                {
                    return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){$"Update Merchant with Id : {id} failed"}});
                }
                else
                {
                    return StatusCode(500, new BaseBadRequestResult(){Errors = new List<string>(){"Internal Server Error!"}});
                }
            }
        }

        /// <summary>
        /// Create Merchant (Authorize)
        /// </summary>
        /// <param name="createMerchantDto"></param>
        /// <returns></returns>
        // POST: api/Merchant
        [HttpPost]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResultWithData<CreateMerchantDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaymentDestination>> CreatePaymentDestination(CreateMerchantDto createMerchantDto)
        {
            if (_context.Merchants == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){"Entity set 'PetShopDbContext.Merchants'  is null."}});
            }
            try
            {
                var merchant = createMerchantDto.Adapt<Merchant>();
                await _context.Merchants.AddAsync(merchant);
                await _context.SaveChangesAsync();
                return Ok(
                    new BaseResultWithData<CreateMerchantDto>()
                    {
                        Success = true,
                        Message = "Create Merchant",
                        Data = createMerchantDto
                    }
                );
            }
            catch(Exception ex)
            {
                return StatusCode(500, new BaseBadRequestResult(){Errors = new List<string>(){$"Internal Server Error - {ex.Message}"}});
            }
        }

        // DELETE: api/Merchant/5
        /// <summary>
        /// Delete Merchant with id (authorize)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Merchants == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){$"Table Merchant is null!"}});
            }
            var merchant = await _context.Merchants.FindAsync(id);
            if (merchant == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){$"Merchant with id : {id} not found!"}});
            }

            _context.Merchants.Remove(merchant);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool MerchantExists(int id)
        {
            return (_context.Merchants?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}