namespace ProjectRed.Core.DTOs.Requests.Auth
{
    public class LoginRequest
    {
        public required string Identifier { get; init; } // email or username
        public required string Password { get; init; }
    }
}
