using ProjectRed.Core.Enums;

namespace ProjectRed.Core.Entities
{
    public class PlayerCareer
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }
        public int ClubId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TransferType TransferType { get; set; }
        public decimal? TransferFee { get; set; }
        public string? FromClubName { get; set; }

        public Player Player { get; set; } = null!;
        public Club Club { get; set; } = null!;
        public ICollection<Contract> Contracts { get; set; } = [];

    }
}
