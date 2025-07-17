using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    public class Leave : EntityBase<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public Tenant? Tenant { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = null!;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string? ApproverComment { get; set; }
    }

}
