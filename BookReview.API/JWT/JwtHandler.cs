using BookReview.DataAccess;
using BookReview.Domain;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookReview.API.JWT
{
    public class JwtHandler
    {
        private readonly BookReviewDbContext _context;
        private readonly AppSettings _appSettings;

        public JwtHandler(
            AppSettings appSettings,
            BookReviewDbContext context)
        {
            _appSettings = appSettings;
            _context = context;
        }

        public JwtTokenResponse MakeToken(User user)
        {
            Guid tokenGuid = Guid.NewGuid();

            string tokenId = tokenGuid.ToString();

            var claims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Iss,
                    _appSettings.JwtSettings.Issuer,
                    ClaimValueTypes.String),

                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),

                new Claim("Username", user.Username),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Email", user.Email),
                new Claim("Id", user.Id.ToString()),
                new Claim("TokenId", tokenId),
                new Claim("Role", user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _appSettings.JwtSettings.SecretKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: _appSettings.JwtSettings.Issuer,
                audience: "Any",
                claims: claims,
                notBefore: now,
                expires: now.AddSeconds(
                    _appSettings.JwtSettings.DurationSeconds),
                signingCredentials: credentials);

            var refreshToken = Guid.NewGuid().ToString();

            var jwtToken = new AuthToken
            {
                CreatedAt = now,
                ExpiresAt = now.AddSeconds(
                    _appSettings.JwtSettings.DurationSeconds),
                TokenId = tokenId,
                UserId = user.Id
            };

            var refreshTokenEntity = new AuthToken
            {
                TokenId = refreshToken,
                CreatedAt = now,
                ExpiresAt = now.AddHours(
                    _appSettings.JwtSettings.RefreshTokenHours),
                UserId = user.Id,
                JwtToken = jwtToken
            };

            _context.AuthTokens.Add(jwtToken);
            _context.AuthTokens.Add(refreshTokenEntity);

            _context.SaveChanges();

            return new JwtTokenResponse
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                RefreshToken = refreshToken
            };
        }
    }
}