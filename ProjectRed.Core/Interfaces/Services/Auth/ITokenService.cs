using ProjectRed.Core.Enums;
using System.Security.Claims;

namespace ProjectRed.Core.Interfaces.Services.Auth
{
    public interface ITokenService
    {
        string GenerateToken(TokenType tokenType, int? userId = null, string email, string? username = null)
        ClaimsPrincipal? ValidateToken(string token);
        string? GetClaim(string token, string claimType);

    }
}
