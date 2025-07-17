using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    /// <summary>
    /// Represents a training session assigned to an employee.
    /// </summary>
    public class Training : EntityBase<Guid>
    {

        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public Tenant? Tenant { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled
    }

}
