using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos;
using serverapi.Dtos.Promotions;
using serverapi.Entity;
using serverapi.Enum;
using serverapi.Services.PagingAndFilterService;

namespace serverapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class PromotionController : ControllerBase
    {
        private readonly PetShopDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public PromotionController(PetShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list promotions
        /// </summary>
        /// <param name="pagingFilterDto">CategoryId is PromotionType (Amount : 1; Percent : 0)</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BasePagingData<List<PromotionInfoDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPromotions([FromQuery] PagingFilterDto pagingFilterDto)
        {
            if (_context.Promotions is null
            || _context.PromotionProducts is null
            || _context.PromotionCategories is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db is null!" } });
            var promotions = await GetListPromotionsAsync();
            var _pagingFilterService = new PagingFilterService<PromotionInfoDto>();
            int TotalPage = 0;
            promotions = _pagingFilterService.FilterAndPage(
                promotions,
                pagingFilterDto,
                pr => pr.PromotionName.Contains(pagingFilterDto.Filter!, StringComparison.OrdinalIgnoreCase),
                pr => pr.DiscountType ==
                    ((DiscountType)pagingFilterDto.CategoryId!
                    == DiscountType.Amount ? "Amount"
                    : (DiscountType)pagingFilterDto.CategoryId
                    == DiscountType.Percent ? "Percent" : "Undefine"),
                pr => pr.ToDate,
                ref TotalPage
            );

            return Ok(new BasePagingData<List<PromotionInfoDto>>()
            {
                TotalPage = TotalPage,
                Data = promotions,
                Message = "List promotions",
                Success = true
            });
        }


        /// <summary>
        /// Get promotion by id (Admin)
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        [HttpGet("{promotionId}")]
        [ProducesResponseType(typeof(BaseResultWithData<PromotionInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPromotionById(int promotionId)
        {
            if (_context.Promotions is null
            || _context.PromotionProducts is null
            || _context.PromotionCategories is null)
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Db is null!" } });
            return Ok(new BaseResultWithData<PromotionInfoDto>()
            {
                Success = true,
                Message = $"Get promotion by id {promotionId}",
                Data = (await GetListPromotionsAsync()).Find(d => d.PromotionId == promotionId)
            });
        }

        /// <summary>
        /// Create promotion
        /// </summary>
        /// <param name="createPromotionDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotionDto createPromotionDto)
        {
            if (ModelState.IsValid)
            {
                var promotion = createPromotionDto.Adapt<Promotion>();
                _context.Promotions.Add(promotion);
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new BaseResultWithData<CreatePromotionDto>()
                    {
                        Success = true,
                        Message = "Create promotion success!",
                        Data = createPromotionDto
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }

            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList() });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createPromotionProductDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-promotion-product")]
        public async Task<IActionResult> AddPromotionProducts(CreatePromotionProductDto createPromotionProductDto)
        {
            if (ModelState.IsValid)
            {
                if ((await _context.Promotions.FindAsync(createPromotionProductDto.PromotionId)) is null)
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Promotion with Id {createPromotionProductDto.PromotionId} not found!" } });
                var promotionProducts = new List<PromotionProduct>();
                createPromotionProductDto.PromotionProductIds
                    .ForEach(p => promotionProducts.Add(new PromotionProduct() { ProductId = p.ProductId, PromotionId = createPromotionProductDto.PromotionId }));
                _context.PromotionProducts.AddRange(promotionProducts);
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new BaseResultWithData<CreatePromotionProductDto>()
                    {
                        Success = true,
                        Message = $"Create promotion products with promotion Id {createPromotionProductDto.PromotionId} success!",
                        Data = createPromotionProductDto
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList() });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="createPromotionCategoryDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-promotion-category")]
        public async Task<IActionResult> AddPromotionCategories(CreatePromotionCategoryDto createPromotionCategoryDto)
        {
            if (ModelState.IsValid)
            {
                if ((await _context.Promotions.FindAsync(createPromotionCategoryDto.PromotionId)) is null)
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"Promotion with Id {createPromotionCategoryDto.PromotionId} not found!" } });
                var promotionCategories = new List<PromotionCategory>();
                createPromotionCategoryDto.PromotionCategoryIds
                    .ForEach(p => promotionCategories.Add(new PromotionCategory() { CategoryId = p.CategoryId, PromotionId = createPromotionCategoryDto.PromotionId }));
                _context.PromotionCategories.AddRange(promotionCategories);
                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(new BaseResultWithData<CreatePromotionCategoryDto>()
                    {
                        Success = true,
                        Message = $"Create promotion categories with promotion Id {createPromotionCategoryDto.PromotionId} success!",
                        Data = createPromotionCategoryDto
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
            return BadRequest(new BaseBadRequestResult() { Errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList() });
        }


        /// <summary>
        /// delete promotion by id (include remove: promotionproduct and promotioncategory)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            if (promotion.FromDate > DateTime.Now || promotion.ToDate > DateTime.Now)
            {
                var promotionProducts = _context.PromotionProducts.Where(pp => pp.PromotionId == id);
                var promotionCategories = _context.PromotionCategories.Where(pc => pc.PromotionId == id);

                _context.PromotionProducts.RemoveRange(promotionProducts);
                _context.PromotionCategories.RemoveRange(promotionCategories);

                await _context.SaveChangesAsync();

                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            return BadRequest($"Cannot delete promotions with FromDate {promotion.FromDate} and ToDate {promotion.ToDate} greater than the current date.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatePromotionDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, UpdatePromotionDto updatePromotionDto)
        {
            if (id != updatePromotionDto.Id)
            {
                return BadRequest();
            }

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            promotion.Name = updatePromotionDto.PromotionName;
            promotion.FromDate = updatePromotionDto.FromDate;
            promotion.ToDate = updatePromotionDto.ToDate;
            promotion.DiscountType = updatePromotionDto.DiscountType;
            promotion.DiscountValue = updatePromotionDto.DiscountValue;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PromotionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.Id == id);
        }

        private async Task<List<PromotionInfoDto>> GetListPromotionsAsync()
        {
            var promotions = await _context.Promotions
                .Include(pr => pr.PromotionCategories)
                .Include(pr => pr.PromotionCategories)
                .Select(pr => new PromotionInfoDto()
                {
                    PromotionId = pr.Id,
                    PromotionName = pr.Name,
                    DiscountType = pr.DiscountType == DiscountType.Amount
                        ? "Amount" : pr.DiscountType == DiscountType.Percent ? "Percent" : "Undefine",
                    DiscountValue = pr.DiscountValue,
                    FromDate = pr.FromDate,
                    ToDate = pr.ToDate,
                    ListPromotionCategory = pr.PromotionCategories
                        .Where(pc => pc.PromotionId == pr.Id)
                        .Select(pc => new PromotionCategoryDto
                        {
                            CategoryId = pc.CategoryId
                        }).ToList(),
                    ListPromotionProduct = pr.PromotionProducts
                        .Where(pp => pp.PromotionId == pr.Id)
                        .Select(pp => new PromotionProductDto
                        {
                            ProductId = pp.ProductId
                        }).ToList()
                })
                .ToListAsync();
            return promotions;
        }
    }
}