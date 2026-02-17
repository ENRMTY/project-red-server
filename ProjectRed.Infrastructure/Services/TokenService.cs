using Microsoft.IdentityModel.Tokens;
using ProjectRed.Core.Configuration;
using ProjectRed.Core.Enums;
using ProjectRed.Core.Interfaces.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectRed.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(JwtSettings jwtSettings)
        {
            _key = jwtSettings.Key;
            _issuer = jwtSettings.Issuer;
            _audience = jwtSettings.Audience;
        }

        public string GenerateToken(TokenType tokenType, int? userId = null, string email, string? username = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, email)
            };

            if (userId.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
            }

            if (!string.IsNullOrEmpty(username))
            {
                claims.Add(new Claim(ClaimTypes.Name, username));
            }

            claims.Add(new Claim(ClaimTypes.Role, tokenType.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(180),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string? GetClaim(string token, string claimType)
        {
            var principal = ValidateToken(token);
            var claim = principal?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

            return claim ?? null;
        }
    }
}
