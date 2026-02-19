using System.Security.Claims;

namespace ProjectRed.Core.Interfaces.Services.Auth
{
    public interface ITokenService
    {
        string GenerateAuthToken(int userId, string email, string username);
        string GenerateProfileCompletionToken(string provider, string providerUserId);
        string GeneratePasswordResetToken(int userId);
        string GenerateEmailVerificationToken(int userId, string email);
        ClaimsPrincipal? ValidateToken(string token);
        string? GetClaim(ClaimsPrincipal principal, string claimType);
    }
}
