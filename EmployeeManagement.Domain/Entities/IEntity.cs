using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Entities
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
    public abstract class EntityBase<T> : IEntity<T>
    {
        [Key] // Marks the property as the primary key
        public T Id { get; set; }
        public string? UserId { get; set; } // FK to Identity User
        public Guid? TenantId { get; set; } // FK to Identity User
    }
}
