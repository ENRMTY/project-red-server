using System.ComponentModel.DataAnnotations;

namespace ProjectRed.Core.Entities
{
    public class Club
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(5)]
        public string ShortName { get; set; } = null!;

        public string Slug { get; set; } = null!;

        [MaxLength(2)]
        public string? CountryCode { get; set; }

        public string? City { get; set; }
        public int? FoundedYear { get; set; }
        public string? CrestImageId { get; set; }
        public string? BannerImageId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Player> Players { get; set; } = [];
        public ICollection<PlayerCareer> PlayersHistory { get; set; } = [];
    }
}
