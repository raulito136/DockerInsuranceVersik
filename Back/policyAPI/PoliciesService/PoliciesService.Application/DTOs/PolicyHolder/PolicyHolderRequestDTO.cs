namespace PoliciesService.Application.DTOs.PolicyHolder
{
    public class PolicyHolderRequestDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string RegionCode { get; set; } = string.Empty;
    }
}
