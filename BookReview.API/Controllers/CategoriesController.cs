using BookReview.Application.Commands.Categories;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly BookReviewDbContext _context;
        private readonly ICreateCategoryCommand _createCategoryCommand;
        private readonly IUpdateCategoryCommand _updateCategoryCommand;

        /*public CategoriesController(BookReviewDbContext context)
        {
            _context = context;
        }*/
        public CategoriesController(BookReviewDbContext context, 
                                ICreateCategoryCommand createCategoryCommand, IUpdateCategoryCommand updateCategoryCommand)
        {
            _context = context;
            _createCategoryCommand = createCategoryCommand;
            _updateCategoryCommand = updateCategoryCommand;
        }

        // GET: api/categories
        [HttpGet]
        public IActionResult Get(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(search));
            }

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var categories = query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToList();

            var result = new BookReview.Application.DTO.PaginationDTO<object>
            {
                Items = categories,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return Ok(result);
        }

        // GET: api/categories/1
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var category = _context.Categories
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .FirstOrDefault();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // POST: api/categories
        /*[HttpPost]
        public IActionResult Post([FromBody] Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return StatusCode(201, category);
        }*/
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] CreateCategoryDTO data)
        {
            _createCategoryCommand.Execute(data);

            return StatusCode(201);
        }

        // PUT: api/categories/1
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateCategoryDTO data)
        {
            _updateCategoryCommand.Execute(id, data);
            return NoContent();
        }

        // DELETE: api/categories/1
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories
                .FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
