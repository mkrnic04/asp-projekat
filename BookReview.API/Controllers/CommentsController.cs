using BookReview.Application.Commands.Comments;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly BookReviewDbContext _context;
        private readonly ICreateCommentCommand _createCommentCommand;
        private readonly IUpdateCommentCommand _updateCommentCommand;

        public CommentsController(
            BookReviewDbContext context,
            ICreateCommentCommand createCommentCommand,
            IUpdateCommentCommand updateCommentCommand)
        {
            _context = context;
            _createCommentCommand = createCommentCommand;
            _updateCommentCommand = updateCommentCommand;
        }


        [HttpGet]
        public IActionResult Get(
            [FromQuery] string? search = null,
            [FromQuery] int? reviewId = null,
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

            var query = _context.Comments.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Text.Contains(search));
            }

            // Review filter
            if (reviewId.HasValue)
            {
                query = query.Where(x =>
                    x.ReviewId == reviewId.Value);
            }

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var comments = query
                .OrderBy(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.ReviewId,
                    x.UserId,
                    x.Text,
                    x.CreatedAt
                })
                .ToList();

            var result =
                new BookReview.Application.DTO.PaginationDTO<object>
                {
                    Items = comments,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };

            return Ok(result);
        }

        // GET: api/comments/1
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var comment = _context.Comments
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.ReviewId,
                    x.UserId,
                    x.Text,
                    x.CreatedAt
                })
                .FirstOrDefault();

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // POST: api/comments
        [Authorize]
        [HttpPost]
        public IActionResult Post(
            [FromBody] CreateCommentDTO data)
        {
            var userIdClaim =
                User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            data.UserId =
                int.Parse(userIdClaim.Value);

            _createCommentCommand.Execute(data);

            return StatusCode(201);
        }

        // PUT: api/comments/1
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(
            int id,
            [FromBody] UpdateCommentDTO data)
        {
            var userIdClaim =
                User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId =
                int.Parse(userIdClaim.Value);

            var comment =
                _context.Comments
                    .FirstOrDefault(x => x.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

       
            if (
                comment.UserId != userId &&
                !User.IsInRole("Admin")
            )
            {
                return Forbid();
            }

            _updateCommentCommand.Execute(
                id,
                data);

            return NoContent();
        }

        // DELETE: api/comments/1
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userIdClaim =
                User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId =
                int.Parse(userIdClaim.Value);

            var comment =
                _context.Comments
                    .FirstOrDefault(x => x.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

    
            // Admin moze brisati kom
            if (
                comment.UserId != userId &&
                !User.IsInRole("Admin")
            )
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);

            _context.SaveChanges();

            return NoContent();
        }
    }
}