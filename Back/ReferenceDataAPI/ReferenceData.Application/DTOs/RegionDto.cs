using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a geographic region.
    /// </summary>
    public class RegionDto
    {
        public int Id { get; set; }
        /// <summary> Region business code (e.g., 'NORTH', 'EMEA'). </summary>
        public string Code { get; set; } = string.Empty;
        /// <summary> Full name of the region. </summary>
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Command to register a new region.
    /// </summary>
    public class CreateRegionRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Command to update region details.
    /// </summary>
    public class UpdateRegionRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}