namespace PoliciesService.Application.DTOs.Policy
{
    public class PolicyRequestDTO
    {
        public int PolicyHolderId { get; set; }
        public string PolicyTypeCode { get; set; } = string.Empty;
        public string CoverageTypeCode { get; set; } = string.Empty;
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
