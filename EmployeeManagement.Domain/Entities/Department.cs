using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    /// <summary>
    /// Represents a department under a tenant (company).
    /// </summary>
    public class Department : EntityBase<Guid>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public Tenant? Tenant { get; set; }
        public ICollection<Employee>? Employees { get; set; }
    }

}
