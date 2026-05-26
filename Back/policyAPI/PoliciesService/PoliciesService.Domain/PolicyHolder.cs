namespace PoliciesService.Domain
{
    public class PolicyHolder
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string RegionCode {  get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } // This can be null

        public ICollection<Policy> Policies { get; set; } = [];
    }
}
