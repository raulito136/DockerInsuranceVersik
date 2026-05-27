namespace PoliciesService.Domain
{
    public class Policy
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public int PolicyHolderId { get; set; } // This is a FK
        public string PolicyTypeCode { get; set; } = string.Empty;
        public string CoverageTypeCode { get; set; } = string.Empty;
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public PolicyHolder? PolicyHolder { get; set; }
    }
}
