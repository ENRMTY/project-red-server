namespace ProjectRed.Core.Entities
{
    public class UserFavoritePlayer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlayerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
