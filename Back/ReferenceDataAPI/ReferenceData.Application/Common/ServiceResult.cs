using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.Common
{
    /// <summary>
    /// Internal wrapper used by services to communicate the outcome of a business operation.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    public class ServiceResult<T>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Gets the resulting value of the operation. Only populated if <see cref="IsSuccess"/> is true.
        /// </summary>
        public T? Value { get; private set; }

        /// <summary>
        /// Gets the name of the field or property that caused a validation or business logic failure.
        /// </summary>
        public string? ErrorField { get; private set; }

        /// <summary>
        /// Gets the detailed error message describing the failure.
        /// </summary>
        public string? ErrorMessage { get; private set; }

        /// <summary>
        /// Returns a successful result containing the specified value.
        /// </summary>
        /// <param name="value">The result data.</param>
        public static ServiceResult<T> Ok(T value) =>
            new() { IsSuccess = true, Value = value };

        /// <summary>
        /// Returns a failed result with details about the error.
        /// </summary>
        /// <param name="field">The field name associated with the failure.</param>
        /// <param name="message">The reason for the failure.</param>
        public static ServiceResult<T> Fail(string field, string message) =>
            new() { IsSuccess = false, ErrorField = field, ErrorMessage = message };
    }
}