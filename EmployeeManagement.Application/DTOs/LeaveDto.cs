using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.DTOs
{
    public class LeaveDto
    {
        public Guid? Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = null!;
        public string Status { get; set; } = "Pending";
        public string? ApproverComment { get; set; }
    }

    public class LeaveResponseDto : LeaveDto
    {
        public string EmployeeName { get; set; } = string.Empty;
    }

}
