using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos;
using serverapi.Dtos.Products;
using serverapi.Entity;
using serverapi.Enum;
using serverapi.Services.PagingAndFilterService;

namespace serverapi.Controllers
{
    /// <summary>
    /// All API for Product
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly PetShopDbContext _dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public ProductController(PetShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Default language is Vn : List info of all product use display home page (AllowAnonymous)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        [Route("/list-product-info-homepage")]
        [ProducesResponseType(typeof(BasePagingData<List<ProductInfoDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlls()
        {
            var pagingService = new PagingFilterDto();
            var data = await GetListProductsAsync("VN");
            var totalPage = (int)Math.Ceiling((double)data.Count / pagingService.PageSize) == 0 ? 1 : (int)Math.Ceiling((double)data.Count / pagingService.PageSize);
            return Ok(new BasePagingData<List<ProductInfoDto>>()
            {
                Success = true,
                Message = "List Product",
                Data = data,
                TotalPage = totalPage
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("/list-product-discount")]
        [ProducesResponseType(typeof(BasePagingData<List<ProductInfoDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllProductDiscount()
        {
            
            var pagingFilterDto = new PagingFilterDto();
            var data = await GetListProductDiscountAsync("VN");
            var service = new PagingFilterService<ProductInfoDto>();
            var filteredAndPagedProducts = service.FilterAndPage(data, pagingFilterDto,
                product => product.Name.Contains(pagingFilterDto.Filter!) || product.CategoryName!.Contains(pagingFilterDto.Filter!),
                product => product.CategoryId == pagingFilterDto.CategoryId,
                product => product.Name);
            var totalPage = (int)Math.Ceiling((double)filteredAndPagedProducts.Count / pagingFilterDto.PageSize) == 0 ? 1 : (int)Math.Ceiling((double)filteredAndPagedProducts.Count / pagingFilterDto.PageSize);
            return Ok(new BasePagingData<List<ProductInfoDto>>()
            {
                Success = true,
                Message = "List Product Discount",
                TotalPage = totalPage,
                Data = filteredAndPagedProducts,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("/list-product-no-discount")]
        [ProducesResponseType(typeof(BasePagingData<List<ProductInfoDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllProductNoDiscount()
        {
            var pagingFilterDto = new PagingFilterDto();
            var data = await GetListProductNoDiscountAsync("VN");
            var service = new PagingFilterService<ProductInfoDto>();
            var filteredAndPagedProducts = service.FilterAndPage(data, pagingFilterDto,
                product => product.Name.Contains(pagingFilterDto.Filter!) || product.CategoryName!.Contains(pagingFilterDto.Filter!),
                product => product.CategoryId == pagingFilterDto.CategoryId,
                product => product.Name);
            var totalPage = (int)Math.Ceiling((double)filteredAndPagedProducts.Count / pagingFilterDto.PageSize) == 0 ? 1 : (int)Math.Ceiling((double)filteredAndPagedProducts.Count / pagingFilterDto.PageSize);
            return Ok(new BasePagingData<List<ProductInfoDto>>()
            {
                Success = true,
                Message = "List Product No Discount",
                TotalPage = totalPage,
                Data = filteredAndPagedProducts,
            });
        }

        /// <summary>
        /// List info of all product use display home page with choosed language (AllowAnonymous)
        /// </summary>
        /// <param name="idLanguage">The laguage id want to display</param>
        /// <param name="pagingFilterDto"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST:
        /// {
        ///     "filter": "enter string to want filter",
        ///     "pageIndex": Index of page want to display,
        ///     "pageSize": Length of item want to display,
        ///     "categoryId": filter by Category Id
        /// }
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        [Route("/list-product-info-homepage")]
        [ProducesResponseType(typeof(BasePagingData<List<ProductInfoDto>>), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> GetAlls(string? idLanguage, [FromQuery] PagingFilterDto pagingFilterDto)
        {
            idLanguage = idLanguage ?? "VN";
            var data = await GetListProductsAsync(idLanguage);
            var service = new PagingFilterService<ProductInfoDto>();
            var filteredAndPagedProducts = service.FilterAndPage(
                data, 
                pagingFilterDto,
                product => product.Name.ToLower().Contains(pagingFilterDto.Filter!.ToLower()) 
                    || product.CategoryName!.ToLower().Contains(pagingFilterDto.Filter!.ToLower()),
                product => product.CategoryId == pagingFilterDto.CategoryId,
                product => product.Name);
            var totalPage = (int)Math.Ceiling((double)filteredAndPagedProducts.Count / pagingFilterDto.PageSize) == 0 ? 1 : (int)Math.Ceiling((double)filteredAndPagedProducts.Count / pagingFilterDto.PageSize);
            return Ok(new BasePagingData<List<ProductInfoDto>>()
            {
                Success = true,
                Message = "List Product",
                TotalPage = totalPage,
                Data = filteredAndPagedProducts,
            });
        }


        /// <summary>
        /// Get one Product by Id (AllowAnonymous)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idLanguage"></param>
        /// <returns></returns>
        /// <remarks>
        ///     GET : Api get product by id
        /// 
        /// </remarks>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductInfoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> GetProductById(int id, string idLanguage)
        {
            if (_dbContext.Products == null)
            {
                return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { "Internal server error" } });
            }
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Product with id : {id} not found" } });
            }
            if (await _dbContext.ProductTranslations.FirstOrDefaultAsync(p => p.ProductId == id) is null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"ProductTranslation with IdProduct : {id} not found" } });
            }
            return Ok(new BaseResultWithData<ProductInfoDto>()
            {
                Success = true,
                Message = "Get product by id",
                Data = (await GetListProductsAsync(idLanguage)).Find(p => p.Id == id)
            });
        }

        /// <summary>
        /// Api create Product and Product Translation (ADMIN)
        /// </summary>
        /// <param name="language"></param>
        /// <param name="createProductDto"></param>
        /// <returns></returns>
        /// <remarks>
        ///     POST : 
        /// {
        ///     "Price": 100.5,
        ///     "OriginalPrice": 120.0,
        ///     "Stock": 50,
        ///     "IsFeatured": true,
        ///     "Name": "Sản phẩm A",
        ///     "SeoDescription": "Đây là mô tả SEO cho sản phẩm A",
        ///     "SeoTitle": "Tiêu đề SEO cho sản phẩm A",
        ///     "SeoAlias": "seo-alias-san-pham-a"
        /// }
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResultWithData<CreateProductDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(string? language, [FromBody] CreateProductDto createProductDto)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // create product
                        //using auto map: CreateProductDto -> Product
                        var product = createProductDto.Adapt<Product>();
                        _dbContext.Products.Add(product);
                        await _dbContext.SaveChangesAsync();

                        // create product trans
                        var productTrans = createProductDto.Adapt<ProductTranslation>();
                        productTrans.ProductId = product.Id;
                        productTrans.LanguageId = language ?? "VN";
                        _dbContext.ProductTranslations.Add(productTrans);
                        await _dbContext.SaveChangesAsync();
                        // commit trans
                        transaction.Commit();
                        return Ok(new BaseResultWithData<CreateProductDto>()
                        {
                            Success = true,
                            Message = "Create product is success!",
                            Data = createProductDto
                        });
                    }
                    catch (Exception ex)
                    {
                        // This will rollback the transaction if any errors occur.
                        transaction.Rollback();
                        return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { ex.Message } });
                    }
                }
            }
            var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(p => p.ErrorMessage)).ToList();
            return BadRequest(new BaseBadRequestResult() { Errors = errors });
        }

        /// <summary>
        /// Api update Product and ProductTranslation (ADMIN)
        /// </summary>
        /// <param name="updateProductDto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        ///     PUT :
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResultWithData<UpdateProductDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(p => p.ErrorMessage)).ToList();
                return BadRequest(new BaseBadRequestResult() { Errors = errors });
            }

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new BaseBadRequestResult(){Errors = new List<string>(){"Db Product is null!"}});
            }

            using (var _transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    if (id != updateProductDto.Id)
                        return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"id = {id} gửi về khác với id product = {updateProductDto.Id}" } });

                    var productUpdate = updateProductDto.Adapt<Product>();
                    var productTranslationUpdate = updateProductDto.Adapt<ProductTranslation>();
                    productTranslationUpdate.ProductId = productUpdate.Id;
                    productTranslationUpdate.LanguageId = updateProductDto.LanguageId;
                    productTranslationUpdate.Id = updateProductDto.ProductTranslationId;

                    _dbContext.Entry<Product>(productUpdate).State = EntityState.Modified;
                    _dbContext.Entry<ProductTranslation>(productTranslationUpdate).State = EntityState.Modified;

                    await _dbContext.SaveChangesAsync();
                    await _transaction.CommitAsync();

                    return Ok(new BaseResultWithData<UpdateProductDto>()
                    {
                        Success = true,
                        Message = $"Product with Id: {id} was update successed!",
                        Data = updateProductDto
                    });
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { ex.Message } });
                }
            }
        }


        /// <summary>
        /// Delete Product by Id (Admin)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_dbContext.Products == null)
            {
                return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { "Internal server error" } });
            }
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Product with id : {id} not found" } });
            }
            try
            {
                _dbContext.Products.Remove(product);
                var productTransRemove = await _dbContext.ProductTranslations.FirstOrDefaultAsync(p => p.ProductId == id);
                if (productTransRemove is not null)
                {
                    _dbContext.ProductTranslations.Remove(productTransRemove);
                    using (var _transaction = await _dbContext.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            await _dbContext.SaveChangesAsync();
                            await _transaction.CommitAsync();
                            return NoContent();
                        }
                        catch (Exception ex)
                        {
                            await _transaction.RollbackAsync();
                            return StatusCode(500, new BaseBadRequestResult()
                            {
                                Errors = new List<string>()
                            {
                                "An error occurred while performing the transaction",
                                $"Không nên hiển thị lỗi này cho client - {ex.Message}"
                            }
                            });
                        }
                    }
                }
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseBadRequestResult()
                {
                    Errors = new List<string>()
                {
                    "Internal server error",
                    $"Không nên hiển thị lỗi này cho client - {ex.Message}"
                }
                });
            }
        }

        private async Task<List<ProductInfoDto>> GetListProductNoDiscountAsync(string? language)
            => await GetListProductIsDiscountAsync(language, false);

        private async Task<List<ProductInfoDto>> GetListProductDiscountAsync(string? language)
            => await GetListProductIsDiscountAsync(language, true);

        private async Task<List<ProductInfoDto>> GetListProductsAsync(string? language)
        {
            var a = await GetListProductIsDiscountAsync(language, true);
            var b = await GetListProductIsDiscountAsync(language, false);
            a.AddRange(b);
            return a;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="language">LanguageId to display</param>
        /// <param name="isDiscount">False : List product non discount; True : Listproduct discount</param>
        /// <returns></returns>
        private async Task<List<ProductInfoDto>> GetListProductIsDiscountAsync(string? language, bool isDiscount)
        {
            language = language ?? "VN";
            var currentDate = DateTime.Now;

            var discountedProducts = await _dbContext.Products
                .Include(p => p.ProductTranslations)
                .ThenInclude(pt => pt.Language)
                .Include(p => p.ProductImages)
                .Include(p => p.PromotionProducts)
                .ThenInclude(pp => pp.Promotion)
                .Include(p => p.Category)
                .ThenInclude(c => c.CategoryTranslations)
                .ThenInclude(ct => ct.Language)
                .Include(p => p.Category.PromotionCategories).ThenInclude(pr => pr.Promotion)
                .Where(p => (p.PromotionProducts.Any(pp => pp.Promotion!.FromDate <= currentDate && pp.Promotion.ToDate >= currentDate) ||
                            p.Category.PromotionCategories.Any(pc => pc.Promotion!.FromDate <= currentDate && pc.Promotion.ToDate >= currentDate)) == isDiscount)
                .Select(p => new ProductInfoDto
                {
                    Id = p.Id,
                    Name = p.ProductTranslations.FirstOrDefault(pp => pp.LanguageId == language)!.Name,
                    Price = p.Price,
                    OriginalPrice = p.OriginalPrice,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryTranslations.FirstOrDefault(ct => ct.LanguageId == language)!.Name!,
                    Description = p.ProductTranslations.FirstOrDefault(pp => pp.LanguageId == language)!.Description,
                    Details = p.ProductTranslations.FirstOrDefault(pp => pp.LanguageId == language)!.Details,
                    SeoDescription = p.ProductTranslations.FirstOrDefault(pp => pp.LanguageId == language)!.SeoDescription,
                    SeoTitle = p.ProductTranslations.FirstOrDefault(pp => pp.LanguageId == language)!.SeoTitle,
                    SeoAlias = p.ProductTranslations.FirstOrDefault(pp => pp.LanguageId == language)!.SeoAlias,
                    ListProductImage = p.ProductImages.Select(pi => new ProductImageDtos
                    {
                        Id = pi.Id,
                        ImageUrl = pi.ImagePath
                    }).ToList(),
                    // ListDiscount = !isDiscount ? null : p.PromotionProducts.Where(pp => pp.Promotion!.FromDate <= currentDate && pp.Promotion.ToDate >= currentDate)
                    //     .Select(pp => new PromotionDto
                    //     {
                    //         PromotionId = pp.Promotion!.Id,
                    //         PromotionName = pp.Promotion.Name,
                    //         FromDate = pp.Promotion.FromDate,
                    //         ToDate = pp.Promotion.ToDate,
                    //         DiscountType = pp.Promotion.DiscountType,
                    //         DiscountValue = pp.Promotion.DiscountValue,
                    //         PriceDisounted = pp.Promotion.DiscountType == DiscountType.Percent ? ((pp.Promotion.DiscountValue > 1 ? (pp.Promotion.DiscountValue / 100) : pp.Promotion.DiscountValue) * p.Price) : pp.Promotion.DiscountValue
                    //     }).Union(
                    //     p.Category.PromotionCategories.Where(pc => pc.Promotion!.FromDate <= currentDate && pc.Promotion.ToDate >= currentDate)
                    //     .Select(pc => new PromotionDto
                    //     {
                    //         PromotionId = pc.Promotion!.Id,
                    //         PromotionName = pc.Promotion.Name,
                    //         FromDate = pc.Promotion.FromDate,
                    //         ToDate = pc.Promotion.ToDate,
                    //         DiscountType = pc.Promotion.DiscountType,
                    //         DiscountValue = pc.Promotion.DiscountValue,
                    //         PriceDisounted = pc.Promotion.DiscountType == DiscountType.Percent ? ((pc.Promotion.DiscountValue > 1 ? (pc.Promotion.DiscountValue / 100) : pc.Promotion.DiscountValue) * p.Price) : pc.Promotion.DiscountValue
                    //     }))
                    //     .ToList(),
                    TotalPriceDiscount = !isDiscount ? 0 : p.PromotionProducts.Where(pp => pp.Promotion!.FromDate <= currentDate && pp.Promotion.ToDate >= currentDate)
                        .Select(pp => new
                        {
                            PriceDisounted = pp.Promotion!.DiscountType == DiscountType.Percent ? ((pp.Promotion.DiscountValue > 1 ? (pp.Promotion.DiscountValue / 100) : pp.Promotion.DiscountValue) * p.Price) : pp.Promotion.DiscountValue
                        }).Union(
                        p.Category.PromotionCategories.Where(pc => pc.Promotion!.FromDate <= currentDate && pc.Promotion.ToDate >= currentDate)
                        .Select(pc => new
                        {
                            PriceDisounted = pc.Promotion!.DiscountType == DiscountType.Percent ? ((pc.Promotion.DiscountValue > 1 ? (pc.Promotion.DiscountValue / 100) : pc.Promotion.DiscountValue) * p.Price) : pc.Promotion.DiscountValue
                        }))
                        .Sum(l => l.PriceDisounted)
                })
                .OrderBy(p => p.Id)
                .ThenBy(p => p.CategoryId)
                .ToListAsync();
            return discountedProducts;
        }
    }
}