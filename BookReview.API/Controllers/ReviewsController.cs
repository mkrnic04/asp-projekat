using Microsoft.AspNetCore.Http;
using BookReview.DataAccess;
using BookReview.Domain;
using Microsoft.AspNetCore.Mvc;

using BookReview.Application.Commands.Reviews;
using BookReview.Application.DTO;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BookReview.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly BookReviewDbContext _context;
        private readonly ICreateReviewCommand _createReviewCommand;
        private readonly IUpdateReviewCommand _updateReviewCommand;

        public ReviewsController(
                 BookReviewDbContext context,
                 ICreateReviewCommand createReviewCommand,
                 IUpdateReviewCommand updateReviewCommand)
        {
            _context = context;
            _createReviewCommand = createReviewCommand;
            _updateReviewCommand = updateReviewCommand;
        }

        [HttpGet]
        public IActionResult Get(
        [FromQuery] string? search = null,
        [FromQuery] int? rating = null,
        [FromQuery] int? bookId = null,
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

            var query = _context.Reviews.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    x.Text.Contains(search));
            }

            if (rating.HasValue)
            {
                query = query.Where(x =>
                    x.Rating == rating.Value);
            }

            if (bookId.HasValue)
            {
                query = query.Where(x => x.BookId == bookId.Value);
            }

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var reviews = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.BookId,
                    x.UserId,
                    x.Title,
                    x.Text,
                    x.Rating,
                    x.CreatedAt
                })
                .ToList();

            var result = new BookReview.Application.DTO.PaginationDTO<object>
            {
                Items = reviews,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var review = _context.Reviews
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.BookId,
                    x.UserId,
                    x.Title,
                    x.Text,
                    x.Rating,
                    x.CreatedAt
                })
                .FirstOrDefault();

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        // POST: api/reviews
        /*[HttpPost]
        public IActionResult Post([FromBody] CreateReviewDTO data)
        {
            _createReviewCommand.Execute(data);

            return StatusCode(201);
        }*/
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] CreateReviewDTO data)
        {
            var userIdClaim = User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            data.UserId = int.Parse(userIdClaim.Value);

            _createReviewCommand.Execute(data);

            return StatusCode(201);
        }

        // PUT: api/reviews/1
        /*[HttpPut("{id}")]
        public IActionResult Put(
        int id,
        [FromBody] UpdateReviewDTO data)
        {
            _updateReviewCommand.Execute(id, data);

            return NoContent();
        }*/
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(
        int id,
        [FromBody] UpdateReviewDTO data)
        {
            var userIdClaim = User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var review = _context.Reviews
                .FirstOrDefault(x => x.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _updateReviewCommand.Execute(id, data);

            return NoContent();
        }

        // DELETE: api/reviews/1
        /*[HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews
                .FirstOrDefault(x => x.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
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

            var review = _context.Reviews
                .FirstOrDefault(x => x.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
