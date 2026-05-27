using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a claim status record.
    /// </summary>
    public class ClaimStatusDto
    {
        /// <summary> Unique identifier for the claim status. </summary>
        public int Id { get; set; }
        /// <summary> Unique alphanumeric business code. </summary>
        public string Code { get; set; } = string.Empty;
        /// <summary> Display name of the status. </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary> Detailed information about the status usage. </summary>
        public string? Description { get; set; }
        /// <summary> Indicates if the status is currently active in the system. </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Request object for creating a new claim status.
    /// </summary>
    public class CreateClaimStatusRequest
    {
        /// <summary> Unique code for the status (e.g., 'OPEN', 'CLOSED'). </summary>
        public string Code { get; set; } = string.Empty;
        /// <summary> Descriptive name for the status. </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary> Optional description of what this status represents. </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request object for updating an existing claim status.
    /// </summary>
    public class UpdateClaimStatusRequest
    {
        /// <summary> Updated name for the status. </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary> Updated description. </summary>
        public string? Description { get; set; }
        /// <summary> Updated activity status. </summary>
        public bool IsActive { get; set; }
    }
}