using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    /// <summary>
    /// Represents daily attendance record of an employee.
    /// </summary>
    public class Attendance : EntityBase<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string Status { get; set; } = "Present"; // e.g., Present, Absent, Leave
        public Tenant? Tenant { get; set; }
    }

}
