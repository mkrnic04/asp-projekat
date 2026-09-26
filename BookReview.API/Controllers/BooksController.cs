using BookReview.Application.Commands.Books;
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
    public class BooksController : ControllerBase
    {
        private readonly BookReviewDbContext _context;
        private readonly ICreateBookCommand _createBookCommand;
        private readonly IUpdateBookCommand _updateBookCommand;
        private readonly IUploadBookCoverCommand _uploadBookCoverCommand;

        public BooksController(
            BookReviewDbContext context,
            ICreateBookCommand createBookCommand,
            IUpdateBookCommand updateBookCommand,
            IUploadBookCoverCommand uploadBookCoverCommand)
        {
            _context = context;
            _createBookCommand = createBookCommand;
            _updateBookCommand = updateBookCommand;
            _uploadBookCoverCommand = uploadBookCoverCommand;
        }


       
        // GET: api/Books
        [HttpGet]
        public IActionResult Get(
            [FromQuery] string? search = null,
            [FromQuery] string? categoryIds = null,
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

            var query = _context.Books.AsQueryable();


            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    x.ISBN.Contains(search) ||
                    x.BookAuthors.Any(ba =>
                        ba.Author.FirstName.Contains(search) ||
                        ba.Author.LastName.Contains(search)));
            }


            // Filter kategorija
            if (!string.IsNullOrWhiteSpace(categoryIds))
            {
                var ids = categoryIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x =>
                    {
                        int.TryParse(x, out int id);
                        return id;
                    })
                    .Where(x => x > 0)
                    .ToList();

                if (ids.Count > 0)
                {
                    query = query.Where(x =>
                        x.BookCategories.Any(bc =>
                            ids.Contains(bc.CategoryId)));
                }
            }


            // Paginacija
          
            var totalCount = query.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);


            var books = query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Description,
                    x.ISBN,
                    x.PublishedYear,
                    x.CoverImagePath,

                    Authors = x.BookAuthors
                        .Select(ba => new
                        {
                            ba.Author.Id,
                            ba.Author.FirstName,
                            ba.Author.LastName
                        })
                        .ToList(),

                    Categories = x.BookCategories
                        .Select(bc => new
                        {
                            bc.Category.Id,
                            bc.Category.Name
                        })
                        .ToList()
                })
                .ToList();


            var result =
                new BookReview.Application.DTO.PaginationDTO<object>
                {
                    Items = books,
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
            var book = _context.Books
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Description,
                    x.ISBN,
                    x.PublishedYear,
                    x.CoverImagePath,

                    Authors = x.BookAuthors
                        .Select(ba => new
                        {
                            ba.Author.Id,
                            ba.Author.FirstName,
                            ba.Author.LastName,
                            ba.Author.Biography
                        })
                        .ToList(),

                    Categories = x.BookCategories
                        .Select(bc => new
                        {
                            bc.Category.Id,
                            bc.Category.Name
                        })
                        .ToList()
                })
                .FirstOrDefault();


            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] CreateBookDTO data)
        {
            var bookId = _createBookCommand.Execute(data);

            return StatusCode(201, new
            {
                id = bookId
            });
        }



        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(
            int id,
            [FromBody] UpdateBookDTO data)
        {
            _updateBookCommand.Execute(id, data);

            return NoContent();
        }


        
        // POST: api/Books/1/cover
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/cover")]
        public async Task<IActionResult> UploadCover(
            int id,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            var data = new UploadBookCoverDTO
            {
                File = memoryStream.ToArray(),
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            _uploadBookCoverCommand.Execute(id, data);

            return NoContent();
        }


        
        // DELETE: api/Books/1
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var book = _context.Books
                .FirstOrDefault(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            _context.SaveChanges();

            return NoContent();
        }
    }
}