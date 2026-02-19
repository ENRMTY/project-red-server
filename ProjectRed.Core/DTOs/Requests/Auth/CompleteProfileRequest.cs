namespace ProjectRed.Core.DTOs.Requests.Auth
{
    public class CompleteProfileRequest
    {
        public required string Username { get; set; }
        public required string DisplayName { get; set; }
        public int? BirthYear { get; set; } 
    }
}
