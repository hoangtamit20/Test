using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos;
using serverapi.Dtos.Statisticals;
using serverapi.Enum;
using serverapi.Services.PagingAndFilterService;

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
        /// Total price saled from Start date to End date
        /// </summary>
        /// <param name="startDate">Choose start date</param>
        /// <param name="endDate">Choose end date</param>
        /// <returns></returns>
        [HttpGet]
        [Route("total-price-saled-in-period")]
        public async Task<IActionResult> TotalPriceSaledInPeriod(DateTime startDate, DateTime endDate)
        {
            if (startDate.Date > endDate.Date)
            {
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Start date cannot be after end date!" } });
            }
            var totalPriceInPeriod = await _context.Orders
                .Where(od =>
                    od.OrderDate.Date >= startDate.Date
                    && od.OrderDate.Date <= endDate.Date
                    && od.Status == OrderStatus.Success)
                .SumAsync(od => od.TotalPrice);

            return Ok(new BaseResultWithData<decimal>()
            {
                Success = true,
                Message = $"Total price saled from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}.",
                Data = totalPriceInPeriod
            });
        }


        /// <summary>
        /// Get the total sales revenue by product category within a specific period order by descending to category totalrevenue and then by category name.    
        /// </summary>
        /// <param name="language">The language to display the category name. Default is "VN".</param>
        /// <param name="startDate">The start date of the period.</param>
        /// <param name="endDate">The end date of the period.</param>
        /// <returns>A list of product categories and their total sales revenue within the specified period.</returns>
        [HttpGet]
        [Route("total-revenue-by-category")]
        [ProducesResponseType(typeof(BaseResultWithData<List<TotalRevenueToCategoryDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> TotalRevenueByCategory(string language, DateTime startDate, DateTime endDate)
        {
            language = language ?? "VN";
            var productRevenues = await _context.OrderDetails
                .Include(od => od.Product)
                .ThenInclude(p => p.Category)
                .ThenInclude(c => c.CategoryTranslations)
                .Select(od => new
                {
                    OrderDetail = od,
                    CategoryTranslation = od.Product.Category.CategoryTranslations
                        .FirstOrDefault(ct => ct.LanguageId == language)
                })
                .Where(x =>
                    x.CategoryTranslation != null
                    && x.OrderDetail.Order.OrderDate >= startDate
                    && x.OrderDetail.Order.OrderDate <= endDate
                    && x.OrderDetail.Order.Status == OrderStatus.Success
                )
                .GroupBy(x => new { x.OrderDetail.Product.Category.Id, x.CategoryTranslation!.Name })
                .Select(g => new TotalRevenueToCategoryDto
                {
                    CategoryId = g.Key.Id,
                    CategoryName = g.Key.Name!,
                    TotalRevenue = g.Sum(x => x.OrderDetail.SubTotal)
                })
                .OrderByDescending(t => t.TotalRevenue).ThenBy(t => t.CategoryName)
                .ToListAsync();
            return Ok(new BaseResultWithData<List<TotalRevenueToCategoryDto>>()
            {
                Success = true,
                Message = $"A list of product categories and their total sales revenue within the specified period from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                Data = productRevenues
            });
        }

        /// <summary>
        /// Get the total sales revenue by product within a specific period order by descending to product totalrevenue and then by product name.
        /// </summary>
        /// <param name="language">The language to display the product name. Default is "VN".</param>
        /// <param name="startDate">The start date of the period.</param>
        /// <param name="endDate">The end date of the period.</param>
        /// <returns>A list of products and their total sales revenue within the specified period.</returns>
        [HttpGet]
        [Route("total-revenue-by-product")]
        public async Task<IActionResult> TotalRevenueByProduct(string language, DateTime startDate, DateTime endDate)
        {
            language = language ?? "VN";
            var productRevenues = await _context.OrderDetails
                .Include(od => od.Product)
                .ThenInclude(p => p.ProductTranslations)
                .Select(od => new
                {
                    OrderDetail = od,
                    ProductTranslation = od.Product.ProductTranslations
                        .FirstOrDefault(pt => pt.LanguageId == language)
                })
                .Where(x =>
                    x.ProductTranslation != null
                    && x.OrderDetail.Order.OrderDate >= startDate
                    && x.OrderDetail.Order.OrderDate <= endDate
                    && x.OrderDetail.Order.Status == OrderStatus.Success
                )
                .GroupBy(x => new { x.OrderDetail.Product.Id, x.ProductTranslation!.Name })
                .Select(g => new TotalRevenueToProductDto
                {
                    ProductId = g.Key.Id,
                    ProductName = g.Key.Name,
                    TotalRevenue = g.Sum(x => x.OrderDetail.SubTotal)
                })
                .OrderByDescending(t => t.TotalRevenue).ThenBy(t => t.ProductName)
                .ToListAsync();

            return Ok(new BaseResultWithData<List<TotalRevenueToProductDto>>()
            {
                Success = true,
                Message = $"A list of products and their total sales revenue within the specified period from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
                Data = productRevenues
            });
        }


        /// <summary>
        /// This API endpoint returns the revenue for each month of a specified year.
        /// </summary>
        /// <param name="year">The year for which the monthly revenue data is required.</param>
        /// <returns>
        /// A list of <see cref="RevenueEveryMonthOfYearDto"/> objects, each representing the revenue for a specific month of the year.
        /// If there are no orders for a particular month, the revenue for that month is returned as 0.
        /// </returns>
        /// <response code="200">Returns the monthly revenue data for the specified year.</response>
        /// <response code="400">If the request is invalid or if the Orders database is null, a list of error messages is returned.</response>
        [HttpGet]
        [Route("revenue-every-month-of-year{year}")]
        [ProducesResponseType(typeof(BaseResultWithData<List<RevenueEveryMonthOfYearDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RevenueEveryMonthOfTheYear(int year)
        {
            if (ModelState.IsValid)
            {
                if (_context.Orders is null)
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Orders is null" } });
                var result = await _context.Orders
                    .Where(od => od.OrderDate.Year == year && od.Status == OrderStatus.Success)
                    .GroupBy(od => od.OrderDate.Month)
                    .Select(od => new RevenueEveryMonthOfYearDto
                    {
                        Month = od.Key,
                        TotalPrice = od.Sum(d => d.TotalPrice)
                    })
                    .ToListAsync();

                for (int i = 1; i <= 12; i++)
                {
                    if (!result.Any(t => t.Month == i))
                        result.Add(new RevenueEveryMonthOfYearDto() { Month = i, TotalPrice = 0 });
                }
                result = result.OrderBy(t => t.Month).ToList();

                return Ok(new BaseResultWithData<List<RevenueEveryMonthOfYearDto>>()
                {
                    Success = true,
                    Message = $"List revenue every month in year {year}",
                    Data = result
                });

            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList() });
        }

        /// <summary>
        /// This API endpoint is used for managing the inventory list.
        /// </summary>
        /// <param name="language">The language parameter is optional and defaults to "VN" if not provided.</param>
        /// <returns>
        /// The API returns a list of items in the inventory that are still in stock. 
        /// Each item in the list includes the product ID, product name, and stock quantity.
        /// If a product is out of stock, its stock quantity will be 0.
        /// The response is sorted in descending order by product stock then by product name.
        /// </returns>

        [HttpGet]
        [Route("inventory-management")]
        public async Task<IActionResult> InventoryManagement(string language)
        {
            language = language ?? "VN";
            var inventoryItems = await _context.Products
                .Include(p => p.ProductTranslations)
                .Select(p => new
                {
                    Product = p,
                    ProductTranslation = p.ProductTranslations
                        .FirstOrDefault(pt => pt.LanguageId == language)
                })
                .Where(x =>
                    x.ProductTranslation != null
                    && x.Product.Stock > 0
                )
                .Select(g => new InventoryItemDto
                {
                    ProductId = g.Product.Id,
                    ProductName = g.ProductTranslation!.Name,
                    Stock = g.Product.Stock
                })
                .OrderByDescending(t => t.Stock).ThenBy(t => t.ProductName)
                .ToListAsync();

            return Ok(new BaseResultWithData<List<InventoryItemDto>>()
            {
                Success = true,
                Message = $"List inventory product",
                Data = inventoryItems
            });
        }


        /// <summary>
        /// This API endpoint is used for listing products with suggestions for the admin.
        /// </summary>
        /// <param name="pagingFilterDto">The language parameter is optional and defaults to "VN" if not provided.</param>
        /// <returns>
        /// The API returns a list of products with their information and suggestions. 
        /// Each product in the list includes the product ID, product name, view count, promotion name, stock, quantity saled in current month, and suggestion.
        /// The suggestion is based on the stock, view count, and quantity saled in current month of the product. 
        /// The suggestion can be one of the following: "Must add product", "Could apply promotion for this product.", or "Normal".
        /// The response is sorted in ascending order by product name.
        /// </returns>
        [HttpGet]
        [Route("list-product-with-suggestion")]
        public async Task<IActionResult> ProductSuggestionManagement([FromQuery] PagingFilterDto pagingFilterDto)
        {
            pagingFilterDto.LanguageId = pagingFilterDto.LanguageId ?? "VN";
            var result = await GetListProductItemWithSuggestionAsync(pagingFilterDto.LanguageId);
            var _pagingFilterService = new PagingFilterService<ProductItemWithSuggestionDto>();
            int TotalPage = 0;
            result = _pagingFilterService.FilterAndPage(
                result,
                pagingFilterDto,
                p => p.ProductName.ToLower().Contains(pagingFilterDto.Filter!.ToLower()),
                p => p.QuantitySaledInCurrentMonth,
                ref TotalPage
            );

            return Ok(new BasePagingData<List<ProductItemWithSuggestionDto>>()
            {
                TotalPage = TotalPage,
                Success = true,
                Message = $"List product for admin",
                Data = result
            });
        }

        private async Task<List<ProductItemWithSuggestionDto>> GetListProductItemWithSuggestionAsync(string language)
        {
            var products = await _context.Products
                .Include(p => p.ProductTranslations)
                .Include(p => p.PromotionProducts)
                .ThenInclude(pp => pp.Promotion)
                .AsSplitQuery()
                .ToListAsync();

            var inventoryItems = products
                .Where(p => p.ProductTranslations.Any(pt => pt.LanguageId == language))
                .Select(g => new ProductItemWithSuggestionDto
                {
                    ProductId = g.Id,
                    ProductName = g.ProductTranslations.First(pt => pt.LanguageId == language).Name,
                    ViewCount = g.ViewCount,
                    PromotionName = CheckProductPromotion(g.Id),
                    Stock = g.Stock,
                    QuantitySaledInCurrentMonth = GetQuantitySaledInCurrentMonth(g.Id),
                    Suggestion = GetProductSuggestion(g.Stock, g.ViewCount, GetQuantitySaledInCurrentMonth(g.Id))
                })
                .OrderBy(t => t.ProductName)
                .ToList();
            return inventoryItems;
        }


        private string GetProductSuggestion(int stock, int viewCount, int quantitySaledInCurrentMonth)
        {
            if (stock == 0)
            {
                return "Must add product";
            }
            else if (stock > 100 || viewCount < 100 || quantitySaledInCurrentMonth < 50)
            {
                return "Could apply promotion for this product.";
            }
            else
            {
                return "Normal";
            }
        }



        private string CheckProductPromotion(int productId)
        {
            // var product = _context.Products
            //     .Include(p => p.PromotionProducts)
            //     .ThenInclude(pp => pp.Promotion)
            //     .FirstOrDefault(p => p.Id == productId);

            var b = _context.PromotionProducts
                .Include(pp => pp.Promotion)
                .FirstOrDefault(p =>
                    p.ProductId == productId
                    && p.Promotion != null
                    && p.Promotion.FromDate <= DateTime.Now
                    && p.Promotion.ToDate >= DateTime.Now);

            // if (b == null)
            // {
            //     throw new Exception("Product not found.");
            // }

            // var promotion = product.PromotionProducts.FirstOrDefault()?.Promotion;
            string discountType = (b is not null && b.Promotion is not null) ? (b.Promotion.DiscountType == DiscountType.Percent ? "Percent" : "Amount") : "";
            return (b is not null && b.Promotion != null)
                ? $"Product is on promotion: {b.Promotion.Name}, from {b.Promotion.FromDate} to {b.Promotion.ToDate}, discount type: {discountType}, discount value: {b.Promotion.DiscountValue}"
                : "No promotion for this product.";
        }


        private int GetQuantitySaledInCurrentMonth(int productId)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var quantitySaledInCurrentMonth = _context.OrderDetails
                .Where(od =>
                    od.ProductId == productId
                    && od.Order.OrderDate.Month == currentMonth
                    && od.Order.OrderDate.Year == currentYear
                    && od.Order.Status == OrderStatus.Success)
                .Sum(od => od.Quantity);

            return quantitySaledInCurrentMonth;
        }

    }
}