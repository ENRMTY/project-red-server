namespace ProjectRed.Core.DTOs.Requests.Auth
{
    public class GoogleAuthRequest
    {
        public required string ProviderUserId { get; init; }
        public required string Email { get; init; }
        public string? DisplayName { get; init; }
        public string? AvatarUrl { get; init; }
    }
}
