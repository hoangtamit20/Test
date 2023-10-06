using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos.PaymentDestinations;
using serverapi.Entity;

namespace PetShop.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentDestinationController : ControllerBase
    {
        private readonly PetShopDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public PaymentDestinationController(PetShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list paymentdestination (Authorize)
        /// </summary>
        /// <returns></returns>
        // GET: api/PaymentDestination
        [HttpGet]
        // [Authorize]
        [ProducesResponseType(typeof(BaseResultWithData<List<PaymentDesInfoDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPaymentDestinations()
        {
            if (_context.PaymentDestinations == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){"DB is null!"}});
            }
            var listPaymentDestination = await _context.PaymentDestinations.ToListAsync();
            var result = new BaseResultWithData<List<PaymentDesInfoDto>>()
            {
                Result = true,
                Message = "List of Paymentdestination",
                Data = listPaymentDestination.Adapt<List<PaymentDesInfoDto>>()
            };
            return  Ok(result);
        }

        /// <summary>
        /// Get Payment Destination by Id (Authorize)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/PaymentDestination/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResultWithData<PaymentDesInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPaymentDestination(int id)
        {
            if (_context.PaymentDestinations == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){"DB is null!"}});
            }
            var paymentDestination = await _context.PaymentDestinations.FindAsync(id);

            if (paymentDestination == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){$"Payment Destination with Id : {id} not found!"}});
            }

            return Ok(paymentDestination.Adapt<PaymentDesInfoDto>());
        }

        /// <summary>
        /// Update paymentdestination (Authorize)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatePaymentDestinationDto"></param>
        /// <returns></returns>
        // PUT: api/PaymentDestination/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdatePaymentDestination(int id, UpdatePaymentDestinationDto updatePaymentDestinationDto)
        {
            if (id != updatePaymentDestinationDto.Id)
            {
                return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"Route Id : {id} is in valid with PaymentDestination Id : {updatePaymentDestinationDto.Id}"}});
            }
            _context.Entry(updatePaymentDestinationDto.Adapt<PaymentDestination>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentDestinationExists(id))
                {
                    return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){$"Update Payment Distination with Id : {id} failed"}});
                }
                else
                {
                    return StatusCode(500, new BaseBadRequestResult(){Errors = new List<string>(){"Internal Server Error!"}});
                }
            }
        }

        /// <summary>
        /// Create payment destination (Authorize)
        /// </summary>
        /// <param name="paymentDestination"></param>
        /// <returns></returns>
        // POST: api/PaymentDestination
        [HttpPost]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(BaseResultBadRequest), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResultWithData<CreatePaymentDesDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaymentDestination>> CreatePaymentDestination(CreatePaymentDesDto paymentDestination)
        {
            if (_context.PaymentDestinations == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){"Entity set 'PetShopDbContext.PaymentDestinations'  is null."}});
            }
            try
            {
                var paymentDes = paymentDestination.Adapt<PaymentDestination>();
                await _context.PaymentDestinations.AddAsync(paymentDes);
                await _context.SaveChangesAsync();
                return Ok(
                    new BaseResultWithData<CreatePaymentDesDto>()
                    {
                        Result = true,
                        Message = "Create Payment Destination",
                        Data = paymentDestination
                    }
                );
            }
            catch(Exception ex)
            {
                return StatusCode(500, new BaseBadRequestResult(){Errors = new List<string>(){$"Internal Server Error - {ex.Message}"}});
            }
        }

        // DELETE: api/PaymentDestination/5
        /// <summary>
        /// Delete payment destination with id (authorize)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.PaymentDestinations == null)
            {
                return NotFound(new BaseResultBadRequest(){Errors = new List<string>(){$"Table Payment Destination is null!"}});
            }
            var paymentDestination = await _context.PaymentDestinations.FindAsync(id);
            if (paymentDestination == null)
            {
                return NotFound(new BaseResultBadRequest(){Errors = new List<string>(){$"Payment destination with id : {id} not found!"}});
            }

            _context.PaymentDestinations.Remove(paymentDestination);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool PaymentDestinationExists(int id)
        {
            return (_context.PaymentDestinations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}