using BookReview.Application.Commands.Users;
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
    public class UsersController : ControllerBase
    {
        private readonly BookReviewDbContext _context;
        private readonly IRegisterUserCommand _registerUserCommand;
        private readonly IUpdateUserCommand _updateUserCommand;

        public UsersController(
            BookReviewDbContext context,
            IRegisterUserCommand registerUserCommand,
            IUpdateUserCommand updateUserCommand)
        {
            _context = context;
            _registerUserCommand = registerUserCommand;
            _updateUserCommand = updateUserCommand;
        }

        // Samo Admin može da vidi listu svih korisnika.
        [Authorize(Roles = "Admin")]
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

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Username.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search));
            }

            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var users = query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Username,
                    x.Email,
                    x.FirstName,
                    x.LastName,
                    x.Role,
                    x.RegisteredAt
                })
                .ToList();

            var result = new BookReview.Application.DTO.PaginationDTO<object>
            {
                Items = users,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return Ok(result);
        }



        // Admin može da vidi bilo kog korisnika.
        // User može da vidi samo sebe.
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var userIdClaim = User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var currentUserId = int.Parse(userIdClaim.Value);

            if (id != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var user = _context.Users
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Username,
                    x.Email,
                    x.FirstName,
                    x.LastName,
                    x.Role,
                    x.RegisteredAt
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        // POST: api/users
        [HttpPost]
        public IActionResult Post([FromBody] RegisterUserDTO data)
        {
            _registerUserCommand.Execute(data);

            return StatusCode(201);
        }


        // Admin može da menja bilo kog korisnika.
        // User može da menja samo sebe.
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(
            int id,
            [FromBody] UpdateUserDTO data)
        {
            var userIdClaim = User.FindFirst("Id");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var currentUserId = int.Parse(userIdClaim.Value);

            if (id != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _updateUserCommand.Execute(id, data);

            return NoContent();
        }


        // Samo Admin može da briše korisnike.
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }
    }
}