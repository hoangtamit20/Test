using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos.Statisticals;
using serverapi.Enum;

namespace serverapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")]
    public class StatisticalController : ControllerBase
    {
        private readonly PetShopDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public StatisticalController(PetShopDbContext context)
        {
            _context = context;
        }

        


        // [HttpGet]

        // public async Task<IActionResult> GetSalestProductOfMonth(int month, int year)
        // {
        //     var products = await _context.OrderDetails
        //         .Include(od => od.Order)
        //         .Include(od => od.Product)
        //         .Include(od => od.Product.ProductImages)
        //         .Include(od => od.Product.ProductTranslations)
        //         .Where(od => 
        //             od.Order.OrderDate.Month == month 
        //             && od.Order.OrderDate.Year == year 
        //             && od.Order.Status == OrderStatus.Success
        //         ).Select(od => new {
        //             ProductId = od.ProductId,
        //             ProductName = od.Product.ProductTranslations.FirstOrDefault(pt => pt.ProductId == od.ProductId && )!.Name,

        //         });
        // }

        // [HttpPost]
        // public async Task<IActionResult> GetSalestProductOfMonth

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetYear()
        {
            List<ChooseYearDto> listResult = new List<ChooseYearDto>();
            listResult.Add(new ChooseYearDto() { value = -1, text = "Chọn năm thống kê" });
            var list = _context.Orders
                // .Where(od => od.Status == OrderStatus.Success)
                .ToList() // Chuyển đổi kết quả thành danh sách trước khi thực hiện GroupBy
                .GroupBy(od => od.OrderDate.Year)
                .Select(od => new ChooseYearDto()
                {
                    value = od.Key,
                    text = od.Key.ToString()
                })
                .OrderByDescending(p => p)
                .ToList();
            listResult.AddRange(list);
            return Ok(listResult);
        }

        /// <summary>
        /// Get total price 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("total-price-saled-in-day")]
        public async Task<IActionResult> TotalPriceSaledInDay()
        {
            if (_context.Orders is null)
                return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"Db Orders is null!"}});
            var currentDate = DateTime.Now;
            var totalPriceInday = await _context.Orders
                .Where(od => od.OrderDate.Date == currentDate.Date)
                .SumAsync(od => od.TotalPrice);
            return Ok(new BaseResultWithData<decimal>(){
                Success = true,
                Message = $"Total price saled in {currentDate}.",
                Data = totalPriceInday
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("total-price-saled-in-day")]
        public async Task<IActionResult> TotalPriceSaledInDay(DateTime date)
        {
            if (_context.Orders is null)
                return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"Db Orders is null!"}});
            var totalPriceInday = await _context.Orders
                .Where(od => od.OrderDate.Date == date.Date)
                .SumAsync(od => od.TotalPrice);
            return Ok(new BaseResultWithData<decimal>(){
                Success = true,
                Message = $"Total price saled in {date}.",
                Data = totalPriceInday
            });
        }




    }
}