using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs
{
    public class PayrollDto
    {
        public Guid? Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid TenantId { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
    }

    public class PayrollResponseDto : PayrollDto
    {
        public decimal NetSalary { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
    }

}
