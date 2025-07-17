using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.ReportDto
{
    public class EmployeeReportDto
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string TenantName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public DateTime DateJoined { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid TenantId { get; set; }
    }
}
