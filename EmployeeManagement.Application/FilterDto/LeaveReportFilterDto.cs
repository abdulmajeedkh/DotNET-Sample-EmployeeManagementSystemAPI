using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.FilterDto
{
    public class LeaveReportFilterDto
    {
        public string? Name { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? TenantId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
