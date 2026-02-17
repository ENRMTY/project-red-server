using System.ComponentModel.DataAnnotations;

namespace ProjectRed.Core.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? ClubId { get; set; }
        public string? Position { get; set; }
        public int? ShirtNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public char Gender { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserFavoritePlayer> Fans { get; set; } = [];
        public ICollection<PlayerCareer> CareerHistory { get; set; } = [];
        public Club? Club { get; set; }
    }
}
