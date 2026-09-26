using BookReview.Application.Commands.Favorites;
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
    public class FavoritesController : ControllerBase
    {
        private readonly BookReviewDbContext _context;
        private readonly ICreateFavoriteCommand _createFavoriteCommand;

        public FavoritesController(BookReviewDbContext context, ICreateFavoriteCommand createFavoriteCommand)
        {
            _context = context;
            _createFavoriteCommand = createFavoriteCommand;
        }


        [Authorize]
        [HttpGet]
        public IActionResult Get(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

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

            var query = _context.Favorites
                .Where(x => x.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Book.Title.Contains(search));
            }

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var favorites = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.UserId,
                    x.BookId,
                    BookTitle = x.Book.Title,
                    Username = x.User.Username,
                    x.CreatedAt
                })
                .ToList();

            var result = new BookReview.Application.DTO.PaginationDTO<object>
            {
                Items = favorites,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return Ok(result);
        }


        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var userIdClaim = User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var favorite = _context.Favorites
                .Where(x => x.Id == id && x.UserId == userId)
                .Select(x => new
                {
                    x.Id,
                    x.UserId,
                    x.BookId,
                    x.CreatedAt
                })
                .FirstOrDefault();

            if (favorite == null)
            {
                return NotFound();
            }

            return Ok(favorite);
        }

        /*[HttpPost]
        public IActionResult Post([FromBody] CreateFavoriteDTO data)
        {
            _createFavoriteCommand.Execute(data);

            return StatusCode(201);
        }*/
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] CreateFavoriteDTO data)
        {
            var userIdClaim = User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            data.UserId = int.Parse(userIdClaim.Value);

            _createFavoriteCommand.Execute(data);

            return StatusCode(201);
        }


        /*[HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var favorite = _context.Favorites
                .FirstOrDefault(x => x.Id == id);

            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorites.Remove(favorite);
            _context.SaveChanges();

            return NoContent();
        }*/
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userIdClaim = User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var favorite = _context.Favorites
                .FirstOrDefault(x => x.Id == id);

            if (favorite == null)
            {
                return NotFound();
            }

            if (favorite.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Favorites.Remove(favorite);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
