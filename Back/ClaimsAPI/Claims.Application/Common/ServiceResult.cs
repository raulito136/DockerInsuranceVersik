namespace Claims.Application.Common;

/// <summary>
/// Resultado tipado que devuelven los servicios de aplicación.
/// En vez de lanzar excepciones para errores de negocio (ej: "policy not found"),
/// el servicio devuelve un ServiceResult con IsSuccess=false y los errores.
/// El controller lee el StatusCode para saber qué HTTP status devolver.
/// </summary>
public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public List<ApiErrorItem> Errors { get; set; } = new();
    public int StatusCode { get; set; } = 200;

    public static ServiceResult<T> Success(T data, int statusCode = 200) =>
        new() { IsSuccess = true, Data = data, StatusCode = statusCode };

    public static ServiceResult<T> Failure(string field, string message, int statusCode = 400) =>
        new()
        {
            IsSuccess = false,
            Errors = new List<ApiErrorItem> { new(field, message) },
            StatusCode = statusCode
        };

    public static ServiceResult<T> Failure(List<ApiErrorItem> errors, int statusCode = 400) =>
        new() { IsSuccess = false, Errors = errors, StatusCode = statusCode };
}

/// <summary>
/// Versión sin dato de retorno — para operaciones que solo devuelven éxito/error (ej: Delete).
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public List<ApiErrorItem> Errors { get; set; } = new();
    public int StatusCode { get; set; } = 200;

    public static ServiceResult Success(int statusCode = 200) =>
        new() { IsSuccess = true, StatusCode = statusCode };

    public static ServiceResult Failure(string field, string message, int statusCode = 400) =>
        new()
        {
            IsSuccess = false,
            Errors = new List<ApiErrorItem> { new(field, message) },
            StatusCode = statusCode
        };

    public static ServiceResult Failure(List<ApiErrorItem> errors, int statusCode = 400) =>
        new() { IsSuccess = false, Errors = errors, StatusCode = statusCode };
}
