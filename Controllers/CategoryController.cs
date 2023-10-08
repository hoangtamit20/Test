using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using serverapi.Base;
using serverapi.Dtos.Categories;
using serverapi.Entity;

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
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            return await _context.Categories.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
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

            return category;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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


        /// <summary>
        /// Create category and categorytranslation
        /// </summary>
        /// <param name="createCategoryDto"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        // POST: api/Category
        [HttpPost]
        [ProducesResponseType(typeof(BaseResultWithData<CategoryInfoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResultBadRequest), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResultBadRequest), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> PostCategory(string? language, [FromBody]CreateCategoryDto createCategoryDto)
        {
            if (_context.Categories == null || _context.CategoryTranslations == null)
            {
                return StatusCode(500, new BaseBadRequestResult(){Errors = new List<string>(){$"Internal Server Error : Db Category is null!"}});
            }
            var category = createCategoryDto.Adapt<Category>();
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if ((await _context.Languages.ToListAsync()).Count < 1)
                    {
                        _context.Languages.Add(new Language(){
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
                        Result = true,
                        Message = "Create category success",
                        Data = data
                    });
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    return BadRequest(new BaseBadRequestResult(){Errors = new List<string>(){$"{ex.Message}"}});
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

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
