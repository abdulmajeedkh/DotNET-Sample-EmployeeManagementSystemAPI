using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs
{
    /// <summary>DTO for creating/updating departments.</summary>
    public class DepartmentDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid TenantId { get; set; }
    }

    /// <summary>Response DTO for departments.</summary>
    public class DepartmentResponseDto : DepartmentDto
    {
        public string TenantName { get; set; } = null!;
    }

}
