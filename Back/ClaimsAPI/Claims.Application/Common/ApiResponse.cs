namespace Claims.Application.Common;

/// <summary>
/// Envelope estándar para TODAS las respuestas de la API.
/// Éxito: { "data": ... }
/// Error: { "errors": [{ "field": "...", "message": "..." }] }
/// </summary>
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public List<ApiErrorItem>? Errors { get; set; }

    public static ApiResponse<T> Success(T data) => new() { Data = data };

    public static ApiResponse<T> Error(List<ApiErrorItem> errors) => new() { Errors = errors };

    public static ApiResponse<T> Error(string field, string message) =>
        new() { Errors = new List<ApiErrorItem> { new(field, message) } };
}

/// <summary>
/// Un error individual dentro de la respuesta.
/// Ejemplo: { "field": "PolicyNumber", "message": "Policy not found" }
/// </summary>
public class ApiErrorItem
{
    public string Field { get; set; }
    public string Message { get; set; }

    public ApiErrorItem(string field, string message)
    {
        Field = field;
        Message = message;
    }
}

/// <summary>
/// Respuesta paginada — extiende el envelope estándar con metadatos de paginación.
/// { "data": [...], "page": 1, "pageSize": 20, "total": 100 }
/// </summary>
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public List<ApiErrorItem>? Errors { get; set; }

    public static PaginatedResponse<T> Success(List<T> data, int page, int pageSize, int total) =>
        new() { Data = data, Page = page, PageSize = pageSize, Total = total };

    public static PaginatedResponse<T> Error(List<ApiErrorItem> errors) =>
        new() { Errors = errors };
}
