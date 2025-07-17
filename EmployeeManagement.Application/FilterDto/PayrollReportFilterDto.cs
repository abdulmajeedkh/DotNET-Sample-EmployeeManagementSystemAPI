using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.FilterDto
{
    public class PayrollReportFilterDto
    {
        public string? Name { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
