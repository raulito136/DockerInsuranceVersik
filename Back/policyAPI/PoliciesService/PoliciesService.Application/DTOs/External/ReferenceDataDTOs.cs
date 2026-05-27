namespace PoliciesService.Application.DTOs.External
{
    public class PolicyTypeDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CoverageTypeDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class RegionDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class ReferenceDataResponse<T>
    {
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }

    public class ReferenceDataPagedResponse<T>
    {
        public IEnumerable<T>? Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
