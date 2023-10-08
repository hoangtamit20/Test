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
        ///     GET : Api được public cho bất kỳ ai
        /// {
        ///     "Id" : "Mã sản phẩm",
        ///     "Name" : "Tên sản phẩm"
        /// }
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        [Route("/list-product-info-homepage")]
        [ProducesResponseType(typeof(List<ProductInfoDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlls() => Ok(await getMa("VN"));

        /// <summary>
        /// List info of all product use display home page with choosed language (AllowAnonymous)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     POST : Api được public cho bất kỳ ai
        /// {
        ///     "Id" : "Mã sản phẩm",
        ///     "Name" : "Tên sản phẩm"
        /// }
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        [Route("/list-product-info-homepage")]
        [ProducesResponseType(typeof(List<ProductInfoDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlls(string idLanguage, [FromQuery] PagingFilterDto pagingFilterDto)
        {
            var list = await GetListProductInfoAsync(idLanguage);
            list = list.Skip((pagingFilterDto.PageSize - 1)*pagingFilterDto.PageIndex)
                .Take(list.Count < pagingFilterDto.PageSize*pagingFilterDto.PageIndex ? 
                    list.Count : pagingFilterDto.PageIndex * pagingFilterDto.PageSize)
                .ToList();
            if (pagingFilterDto.Filter is not null)
            {
                list = list.Where(l => l.Name.Contains(pagingFilterDto.Filter) || l.CategoryName.Contains(pagingFilterDto.Filter)).ToList();
            }
            if (pagingFilterDto.CategoryId is not null)
            {
                list = list.Where(l => l.CategoryId == pagingFilterDto.CategoryId.Value).ToList();
            }
            return Ok(list);
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
        // GET: api/Category/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductInfoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> GetCategory(int id, string idLanguage)
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
            if (_dbContext.ProductTranslations.FirstOrDefaultAsync(p => p.ProductId == id) is null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"ProductTranslation with IdProduct : {id} not found" } });
            }
            return Ok((await GetListProductInfoAsync(idLanguage)).Find(p => p.Id == id));
        }

        /// <summary>
        /// Api create Product and Product Translation (ADMIN)
        /// </summary>
        /// <param name="createProductDto"></param>
        /// <returns></returns>
        /// 
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
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
                        await _dbContext.Products.AddAsync(product);
                        await _dbContext.SaveChangesAsync();

                        // create product trans
                        var productTrans = createProductDto.Adapt<ProductTranslation>();
                        productTrans.ProductId = product.Id;
                        await _dbContext.ProductTranslations.AddAsync(productTrans);
                        await _dbContext.SaveChangesAsync();
                        // commit trans
                        transaction.Commit();
                        return Ok(createProductDto);
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
        [ProducesResponseType(typeof(UpdateProductDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
                return NotFound();
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

                    return Ok(updateProductDto);
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

        private async Task<List<ProductInfoDto>> GetListProductInfoAsync(string idLanguage)
        {
            var query = _dbContext.Products
                        .Join(_dbContext.ProductTranslations, p => p.Id, pl => pl.ProductId, (p, pl) => new { Products = p, ProductTrans = pl })
                        .AsQueryable();
            if (idLanguage != null)
            {
                query = query.Where(group => group.ProductTrans.LanguageId == idLanguage);
            }

            var listProductInfo = await query
                .Select(group => new ProductInfoDto
                {
                    Id = group.Products.Id,
                    Name = group.ProductTrans.Name,
                    Price = group.Products.Price,
                    OriginalPrice = group.Products.OriginalPrice,
                    Stock = group.Products.Stock,
                    CategoryId = group.Products.CategoryId,
                    CategoryName = group.Products.Category.CategoryTranslations.Where(c => c.Id == group.Products.CategoryId).FirstOrDefault()!.Name!,
                    Description = group.ProductTrans.Description,
                    Details = group.ProductTrans.Details,
                    SeoDescription = group.ProductTrans.SeoDescription,
                    SeoTitle = group.ProductTrans.SeoTitle,
                    SeoAlias = group.ProductTrans.SeoAlias,
                    ListProductImage = group.Products.ProductImages.Where(img => img.ProductId == group.Products.Id).Select(p => new ProductImageDtos
                    {
                        Id = p.Id,
                        ImageUrl = p.ImagePath
                    }).ToList(),
                }).ToListAsync();

            return listProductInfo;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idLanguage"></param>
        /// <returns></returns>
        private async Task<List<ProductInfoDto>> getMa(string idLanguage)
        {
            var listProductInfo = await _dbContext.Products
            .Include(p => p.ProductTranslations)
            .Include(p => p.Category)
            .ThenInclude(c => c.CategoryTranslations)
            .Include(p => p.ProductImages)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(p => p.ProductTranslations.Any(pt => pt.LanguageId == idLanguage))
            .Select(p => new ProductInfoDto
            {
                Id = p.Id,
                Name = p.ProductTranslations.FirstOrDefault(pt => pt.LanguageId == idLanguage)!.Name,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryTranslations.FirstOrDefault(ct => ct.LanguageId == idLanguage)!.Name!,
                Description = p.ProductTranslations.FirstOrDefault(pt => pt.LanguageId == idLanguage)!.Description,
                Details = p.ProductTranslations.FirstOrDefault(pt => pt.LanguageId == idLanguage)!.Details,
                SeoDescription = p.ProductTranslations.FirstOrDefault(pt => pt.LanguageId == idLanguage)!.SeoDescription,
                SeoTitle = p.ProductTranslations.FirstOrDefault(pt => pt.LanguageId == idLanguage)!.SeoTitle,
                SeoAlias = p.ProductTranslations.FirstOrDefault(pt => pt.LanguageId == idLanguage)!.SeoAlias,
                ListProductImage = p.ProductImages.Select(pi => new ProductImageDtos { Id = pi.Id, ImageUrl = pi.ImagePath }).ToList()
            })
            .ToListAsync();
            return listProductInfo;
        }
    }
}