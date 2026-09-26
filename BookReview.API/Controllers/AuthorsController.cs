using BookReview.Application.Commands.Authors;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly BookReviewDbContext _context;
        private readonly ICreateAuthorCommand _createAuthorCommand;
        private readonly IUpdateAuthorCommand _updateAuthorCommand;

        public AuthorsController(BookReviewDbContext context, 
               ICreateAuthorCommand createAuthorCommand,
               IUpdateAuthorCommand updateAuthorCommand)
        {
            _context = context;
            _createAuthorCommand = createAuthorCommand;
            _updateAuthorCommand = updateAuthorCommand;
        }

        // GET: api/authors
        /*[HttpGet]
        public IActionResult Get()
        {
            var authors = _context.Authors
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Biography
                })
                .ToList();

            return Ok(authors);
        }*/
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

            var query = _context.Authors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search));
            }

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var authors = query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Biography
                })
                .ToList();

            var result = new BookReview.Application.DTO.PaginationDTO<object>
            {
                Items = authors,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return Ok(result);
        }

        // GET: api/authors/1
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var author = _context.Authors
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.Biography
                })
                .FirstOrDefault();

            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        // POST: api/authors
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] CreateAuthorDTO data)
        {
            _createAuthorCommand.Execute(data);

            return StatusCode(201);
        }

        // PUT: api/authors/1
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UpdateAuthorDTO data)
        {
            _updateAuthorCommand.Execute(id, data);

            return NoContent();
        }

        // DELETE: api/authors/1
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var author = _context.Authors
                .FirstOrDefault(x => x.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            _context.SaveChanges();

            return NoContent();
        }
    }
}