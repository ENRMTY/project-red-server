namespace ProjectRed.Core.DTOs.Requests.Auth
{
    public class RegisterRequest
    {
        public string? DisplayName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int BirthYear { get; set; }
    }
}
