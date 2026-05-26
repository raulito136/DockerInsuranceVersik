using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.Common
{
    /// <summary>
    /// Standard structure for all API responses.
    /// </summary>
    /// <typeparam name="T">The type of the data returned in the response.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// The payload of the response when the operation is successful.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// A list of errors encountered during the request processing.
        /// </summary>
        public List<ApiError> Errors { get; set; } = [];

        /// <summary>
        /// Creates a successful response with the provided data.
        /// </summary>
        /// <param name="data">The data to be returned.</param>
        /// <returns>A new instance of <see cref="ApiResponse{T}"/> marked as success.</returns>
        public static ApiResponse<T> Success(T data) => new() { Data = data };

        /// <summary>
        /// Creates a failed response with a specific field and message.
        /// </summary>
        /// <param name="field">The name of the field that caused the error.</param>
        /// <param name="message">A human-readable description of the error.</param>
        /// <returns>A new instance of <see cref="ApiResponse{T}"/> containing the error details.</returns>
        public static ApiResponse<T> Fail(string field, string message) =>
            new() { Errors = [new ApiError(field, message)] };
    }

    /// <summary>
    /// Represents a specific error detail.
    /// </summary>
    /// <param name="Field">The property or field name associated with the error.</param>
    /// <param name="Message">The error description.</param>
    public record ApiError(string Field, string Message);
}