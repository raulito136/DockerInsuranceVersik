// PoliciesService\PoliciesService.Tests\TestHelpers\MockDataGenerator.cs
using Bogus;
using PoliciesService.Application.DTOs.Policy;
using PoliciesService.Application.DTOs.PolicyHolder;
using PoliciesService.Domain;

namespace PoliciesService.Tests.TestHelpers
{
    public static class MockDataGenerator
    {
        public static List<PolicyHolder> GeneratePolicyHolders(int count)
        {
            var faker = new Faker<PolicyHolder>()
                //.RuleFor(h => h.Id, f => f.IndexGlobal)
                .RuleFor(h => h.FirstName, f => f.Name.FirstName())
                .RuleFor(h => h.LastName, f => f.Name.LastName())
                .RuleFor(h => h.Email, f => f.Internet.Email())
                .RuleFor(h => h.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(h => h.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-18))))
                .RuleFor(h => h.RegionCode, "US")
                .RuleFor(h => h.CreatedAt, f => f.Date.Past());
            return faker.Generate(count);
        }

        public static List<Policy> GeneratePolicies(int count, int policyHolderId)
        {
            var faker = new Faker<Policy>()
                //.RuleFor(p => p.Id, f => f.IndexGlobal)
                .RuleFor(p => p.PolicyNumber, f => f.Random.AlphaNumeric(10).ToUpper())
                .RuleFor(p => p.PolicyHolderId, policyHolderId)
                .RuleFor(p => p.PolicyTypeCode, "AUTO")
                .RuleFor(p => p.CoverageTypeCode, "COMPREHENSIVE")
                .RuleFor(p => p.CoverageAmount, f => Math.Round(f.Random.Decimal(1000, 50000), 2))
                .RuleFor(p => p.StartDate, f => DateOnly.FromDateTime(f.Date.Recent()))
                .RuleFor(p => p.EndDate, (f, p) => p.StartDate.AddYears(1))
                .RuleFor(p => p.PremiumAmount, f => Math.Round(f.Random.Decimal(100, 2000), 2))
                .RuleFor(p => p.Status, "ACTIVE")
                .RuleFor(p => p.CreatedAt, f => f.Date.Past());
            return faker.Generate(count);
        }

        public static PolicyRequestDTO GeneratePolicyRequest(int policyHolderId)
        {
            var faker = new Faker<PolicyRequestDTO>()
                .RuleFor(dto => dto.PolicyHolderId, policyHolderId)
                .RuleFor(dto => dto.PolicyTypeCode, "AUTO")
                .RuleFor(dto => dto.CoverageTypeCode, "COMPREHENSIVE")
                .RuleFor(dto => dto.CoverageAmount, f => Math.Round(f.Random.Decimal(1000, 50000), 2))
                .RuleFor(dto => dto.StartDate, f => DateOnly.FromDateTime(f.Date.Recent()))
                .RuleFor(dto => dto.EndDate, (f, p) => p.StartDate.AddYears(1))
                .RuleFor(dto => dto.PremiumAmount, f => Math.Round(f.Random.Decimal(100, 2000), 2))
                .RuleFor(dto => dto.Status, "ACTIVE");
            return faker.Generate();
        }

        public static PolicyHolderRequestDTO GeneratePolicyHolderRequest()
        {
            var faker = new Faker<PolicyHolderRequestDTO>()
                .RuleFor(dto => dto.FirstName, f => f.Name.FirstName())
                .RuleFor(dto => dto.LastName, f => f.Name.LastName())
                .RuleFor(dto => dto.Email, f => f.Internet.Email())
                .RuleFor(dto => dto.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(dto => dto.DateOfBirth, f => DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-18))))
                .RuleFor(dto => dto.RegionCode, "US");
            return faker.Generate();
        }
    }
}
