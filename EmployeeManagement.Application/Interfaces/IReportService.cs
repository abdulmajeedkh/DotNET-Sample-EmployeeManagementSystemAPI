using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.FilterDto;
using EmployeeManagement.Application.ReportDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface IReportService
    {
        Task<ResponseDto<PagedResult<EmployeeReportDto>>> GetEmployeeReportsAsync(EmployeeReportFilterDto filter, int pageIndex, int pageSize, string sortBy, string sortDirection);
       // Task<PagedResult<LeaveResponseDto>> GetLeaveReportAsync(LeaveReportFilterDto filter);
      //  Task<PagedResult<PayrollResponseDto>> GetPayrollReportAsync(PayrollReportFilterDto filter);

        Task<ResponseDto<byte[]>> ExportEmployeeReportToCsvAsync(EmployeeReportFilterDto filter);
        //Task<ResponseDto<byte[]>> ExportEmployeeReportToPdfAsync(EmployeeReportFilterDto filter);
        // Extend for Attendance, Training, etc.
    }

}
