using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    public class Payslip : EntityBase<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay => BasicSalary + Allowances - Deductions;
    }

}
