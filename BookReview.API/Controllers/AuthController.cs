using BookReview.API.JWT;
using BookReview.DataAccess;
using BookReview.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BookReview.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BookReviewDbContext _context;
        private readonly JwtHandler _handler;

        public AuthController(
            BookReviewDbContext context,
            JwtHandler handler)
        {
            _context = context;
            _handler = handler;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            User? user = _context.Users
                .FirstOrDefault(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized();
            }

            if (!BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.Password))
            {
                return Unauthorized();
            }

            return Ok(_handler.MakeToken(user));
        }



        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            var refreshToken = _context.AuthTokens
                .FirstOrDefault(x => x.TokenId == request.RefreshToken);

            if (refreshToken == null)
            {
                return Unauthorized();
            }

            if (refreshToken.RevokedAt != null)
            {
                return Unauthorized();
            }

            if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var user = _context.Users
                .FirstOrDefault(x => x.Id == refreshToken.UserId);

            if (user == null)
            {
                return Unauthorized();
            }

            if (refreshToken.JwtToken != null)
            {
                refreshToken.JwtToken.RevokedAt = DateTime.UtcNow;
            }

            refreshToken.RevokedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return Ok(_handler.MakeToken(user));
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return Unauthorized();
            }

            var parts = authorizationHeader.Split(' ');

            if (parts.Length != 2 || parts[0] != "Bearer")
            {
                return Unauthorized();
            }

            var token = parts[1];

            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwtToken;

            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch
            {
                return Unauthorized();
            }

            var tokenIdClaim = jwtToken.Claims
                .FirstOrDefault(x => x.Type == "TokenId");

            if (tokenIdClaim == null)
            {
                return Unauthorized();
            }

            var authToken = _context.AuthTokens
                .FirstOrDefault(x => x.TokenId == tokenIdClaim.Value);

            if (authToken == null)
            {
                return Unauthorized();
            }

            if (authToken.RevokedAt != null)
            {
                return Unauthorized();
            }

            authToken.RevokedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return NoContent();
        }
    }
}
