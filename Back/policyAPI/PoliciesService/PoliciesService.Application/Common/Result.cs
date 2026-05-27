namespace PoliciesService.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public T? Data { get; }

        private Result(bool isSuccess, string? errorMessage, T? data)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Data = data;
        }

        // Everything good
        public static Result<T> Success(T data)
            => new(true, null, data);

        // Failure, send message
        public static Result<T> Failure(string errorMessage)
            => new(false, errorMessage, default);
    }
}
