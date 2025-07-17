using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    /// <summary>
    /// Represents payroll information for an employee for a specific month.
    /// </summary>
    public class Payroll :  EntityBase<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public Tenant? Tenant { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }

        public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
    }

}
