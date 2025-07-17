using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs
{
    /// <summary>DTO for creating or updating attendance records.</summary>
    public class AttendanceDto
    {
        public Guid? Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string Status { get; set; } = "Present";
        public Guid TenantId { get; set; }
    }

    /// <summary>Attendance response DTO with employee info.</summary>
    public class AttendanceResponseDto : AttendanceDto
    {
        public string EmployeeName { get; set; } = string.Empty;
    }

}
