using System.Net;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos;
using serverapi.Dtos.Categories;
using serverapi.Entity;
using serverapi.Services.PagingAndFilterService;

namespace PetShop.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly PetShopDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public CategoryController(PetShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list category include paging, filter and sort by categories name
        /// </summary>
        /// <returns></returns>
        // GET: api/Category
        [HttpGet]
        [Route("/list-categories")]
        [ProducesResponseType(typeof(BaseResultWithData<List<CategoryInfoDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetCategories(string? language)
        {
            int TotalPage = 0;
            PagingFilterDto pagingFilterDto = new PagingFilterDto();
            if (_context.Categories == null || _context.CategoryTranslations == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Categories is null" } });
            }
            var listCategories = await _context.CategoryTranslations.Include(c => c.Category)
                .Where(cl => cl.LanguageId == (language ?? "VN"))
                .Select(cl => new CategoryInfoDto()
                {
                    Id = cl.CategoryId,
                    IsShowHome = cl.Category.IsShowHome,
                    Name = cl.Name!,
                    SeoAlias = cl.SeoAlias,
                    SeoDescription = cl.SeoDescription,
                    SeoTitle = cl.SeoTitle
                })
                .ToListAsync();

            listCategories = new PagingFilterService<CategoryInfoDto>()
                .FilterAndPage(
                    listCategories,
                    pagingFilterDto,
                    category => (category.Name != null && category.Name.Contains(pagingFilterDto.Filter!))
                        || category.Id.ToString().Contains(pagingFilterDto.Filter!),
                    category => category.Name,
                    ref TotalPage
                )
                .ToList();
            return Ok(new BaseResultWithData<List<CategoryInfoDto>>()
            {
                Success = true,
                Message = "List categorie",
                Data = listCategories
            });
        }

        /// <summary>
        /// Get list category include paging, filter and sort by categories name
        /// </summary>
        /// <param name="language"></param>
        /// <param name="pagingFilterDto"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("/list-categories")]
        [ProducesResponseType(typeof(BaseResultWithData<List<CategoryInfoDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetCategories(string? language, [FromQuery] PagingFilterDto pagingFilterDto)
        {
            if (ModelState.IsValid)
            {
                int TotalPage = 0;
                if (_context.Categories == null || _context.CategoryTranslations == null)
                {
                    return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Db Categories is null" } });
                }
                var listCategories = await _context.CategoryTranslations.Include(c => c.Category)
                    .Where(cl => cl.LanguageId == (language ?? "VN"))
                    .Select(cl => new CategoryInfoDto()
                    {
                        Id = cl.CategoryId,
                        IsShowHome = cl.Category.IsShowHome,
                        Name = cl.Name!,
                        SeoAlias = cl.SeoAlias,
                        SeoDescription = cl.SeoDescription,
                        SeoTitle = cl.SeoTitle
                    })
                    .ToListAsync();

                listCategories = new PagingFilterService<CategoryInfoDto>()
                    .FilterAndPage(
                        listCategories,
                        pagingFilterDto,
                        category => category.Name!.Contains(pagingFilterDto.Filter!)
                            || category.Id.ToString().Contains(pagingFilterDto.Filter!),
                        category => category.Name,
                        ref TotalPage
                    )
                    .ToList();
                return Ok(new BaseResultWithData<List<CategoryInfoDto>>()
                {
                    Success = true,
                    Message = "List categorie",
                    Data = listCategories
                });
            }
            // get error from modelstate
            var Errors = ModelState.Values
                .SelectMany(error => error.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new BaseBadRequestResult() { Errors = Errors });
        }

        /// <summary>
        /// Get categories by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Category/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResultWithData<CategoryInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.NotFound)]

        public async Task<ActionResult> GetCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { "Db Categories is null!" } });
            }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new BaseBadRequestResult() { Errors = new List<string>() { $"Category with id : {id} not found!" } });
            }
            return Ok(new BaseResultWithData<CategoryInfoDto>()
            {
                Success = true,
                Message = $"Cagegory with id : {id}",
                Data = category.Adapt<CategoryInfoDto>()
            });
        }

        /// <summary>
        /// Update category and categorytranslation by id (Admin)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateCategoyDto"></param>
        /// <returns></returns>
        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutCategory(int id, UpdateCategoyDto updateCategoyDto)
        {
            if (id != updateCategoyDto.Id)
            {
                return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"The Id : {id} is not valid with category id : {updateCategoyDto.Id}" } });
            }
            var category = updateCategoyDto.Adapt<Category>();
            var categoryTranslation = updateCategoyDto.Adapt<CategoryTranslation>();
            using (var _transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Entry<Category>(category).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    categoryTranslation.CategoryId = category.Id;
                    _context.Entry<CategoryTranslation>(categoryTranslation).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    _transaction.Commit();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"Failed : {ex.Message}"}});
                }
            }
        }


        /// <summary>
        /// Create category and categorytranslation
        /// </summary>
        /// <param name="createCategoryDto"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        // POST: api/Category
        [HttpPost]
        [ProducesResponseType(typeof(BaseResultWithData<CategoryInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseBadRequestResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> PostCategory(string? language, [FromBody] CreateCategoryDto createCategoryDto)
        {
            if (_context.Categories == null || _context.CategoryTranslations == null)
            {
                return StatusCode(500, new BaseBadRequestResult() { Errors = new List<string>() { $"Internal Server Error : Db Category is null!" } });
            }
            var category = createCategoryDto.Adapt<Category>();
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if ((await _context.Languages.ToListAsync()).Count < 1)
                    {
                        _context.Languages.Add(new Language()
                        {
                            Id = "VN",
                            IsDefault = true,
                            Name = "Việt Nam"
                        });
                    }
                    // add category
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                    // add category translation
                    var categoryTranslation = createCategoryDto.Adapt<CategoryTranslation>();
                    categoryTranslation.CategoryId = category.Id;
                    categoryTranslation.LanguageId = language ?? "VN";
                    _context.CategoryTranslations.Add(categoryTranslation);
                    await _context.SaveChangesAsync();
                    await _transaction.CommitAsync();
                    var data = categoryTranslation.Adapt<CategoryInfoDto>();
                    data.Id = category.Id;
                    return Ok(new BaseResultWithData<CategoryInfoDto>()
                    {
                        Success = true,
                        Message = "Create category success",
                        Data = data
                    });
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    return BadRequest(new BaseBadRequestResult() { Errors = new List<string>() { $"{ex.Message}" } });
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
