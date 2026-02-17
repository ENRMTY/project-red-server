namespace ProjectRed.Core.DTOs.Data
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? DisplayName { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
