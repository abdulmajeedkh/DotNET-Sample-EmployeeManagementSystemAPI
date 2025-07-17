using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Domain.Entities.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(100)]

        public string? FullName { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
