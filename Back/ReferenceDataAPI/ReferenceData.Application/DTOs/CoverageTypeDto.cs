using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for insurance coverage types.
    /// </summary>
    public class CoverageTypeDto
    {
        public int Id { get; set; }
        /// <summary> Alphanumeric code representing the coverage (e.g., 'TPL', 'COMP'). </summary>
        public string Code { get; set; } = string.Empty;
        /// <summary> Name of the coverage type. </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary> Description of what is included in this coverage. </summary>
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Data required to create a new type of coverage.
    /// </summary>
    public class CreateCoverageTypeRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data required to update an existing coverage type.
    /// </summary>
    public class UpdateCoverageTypeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}