namespace ProjectRed.Core.DTOs.Data
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? DisplayName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
