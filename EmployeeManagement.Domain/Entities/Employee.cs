using EmployeeManagement.Domain.Entities.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    /// <summary>
    /// Represents an employee within a tenant (company).
    /// </summary>
    public class Employee : EntityBase<Guid>
    {
        public ApplicationUser? User { get; set; }
        public Guid DepartmentId { get; set; }
        public Department? Department { get; set; }
        public Tenant? Tenant { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public DateTime DateJoined { get; set; }
    }


}
