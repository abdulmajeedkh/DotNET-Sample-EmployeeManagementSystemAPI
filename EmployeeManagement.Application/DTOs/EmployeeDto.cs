using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs
{
    /// <summary>
    /// DTO for creating or updating an employee.
    /// </summary>
    public class EmployeeDto
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public DateTime DateJoined { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid TenantId { get; set; }
    }

    /// <summary>
    /// Employee response DTO with linked user and dept info.
    /// </summary>
    public class EmployeeResponseDto : EmployeeDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string TenantName { get; set; } = null!;
    }


}
