using System.ComponentModel.DataAnnotations;

namespace ProjectRed.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Username { get; set; } = null!;
        [MaxLength(30)]
        public string? DisplayName { get; set; }
        public int? BirthYear { get; set; }
        public string? AvatarId { get; set; }
        public bool IsVerified { get; set; }
        [MaxLength(2)]
        public string? CountryCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserSession> Sessions { get; set; } = [];
        public ICollection<UserFavoritePlayer> FavoritePlayers { get; set; } = [];
        public ICollection<UserAuth> AuthMethods { get; set; } = [];
    }
}
