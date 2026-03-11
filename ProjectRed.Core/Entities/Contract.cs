namespace ProjectRed.Core.Entities
{
    public class Contract
    {
        public int Id { get; set; }
        public int PlayerCareerId { get; set; }

        public decimal WeeklySalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }

        public PlayerCareer PlayerCareer { get; set; } = null!;
    }
}
