using System.Text.Json.Serialization;

namespace PoliciesService.Application.Common
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("errors")]
        public List<ApiError>? Errors { get; set; }

        public static ApiResponse<T> Success(T data) => new() { Data = data };
        public static ApiResponse<T> Error(string message, string? field = null)
            => new() { Errors = [new ApiError { Message = message, Field = field }] };
    }

    public class ApiError
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class PaginatedResponse<T> : ApiResponse<IEnumerable<T>>
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        public static PaginatedResponse<T> Create(IEnumerable<T> data, int page, int pageSize, int total)
            => new() { Data = data, Page = page, PageSize = pageSize, Total = total };
    }
}
