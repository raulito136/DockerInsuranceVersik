using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceData.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for insurance policy types.
    /// </summary>
    public class PolicyTypeDto
    {
        public int Id { get; set; }
        /// <summary> Unique code for the policy type (e.g., 'AUTO', 'HOME'). </summary>
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Input model for adding a new policy type.
    /// </summary>
    public class CreatePolicyTypeRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Input model for modifying a policy type.
    /// </summary>
    public class UpdatePolicyTypeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}