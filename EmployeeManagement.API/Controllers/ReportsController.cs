using EmployeeManagement.Application.FilterDto;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("employee")]
        public async Task<IActionResult> GetEmployeeReport([FromBody] EmployeeReportFilterDto filter, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "FirstName", [FromQuery] string sortDirection = "asc")
        {
            var result = await _reportService.GetEmployeeReportsAsync(filter, pageIndex, pageSize, sortBy, sortDirection);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("employee/export/csv")]
        public async Task<IActionResult> ExportEmployeeReportCsv([FromBody] EmployeeReportFilterDto filter)
        {
            var result = await _reportService.ExportEmployeeReportToCsvAsync(filter);
            return result.IsSuccess ? File(result.Value!, "text/csv", "employee_report.csv") : BadRequest(result);
        }

        //[HttpPost("employee/export/pdf")]
        //public async Task<IActionResult> ExportEmployeeReportPdf([FromBody] EmployeeReportFilterDto filter)
        //{
        //    var result = await _reportService.ExportEmployeeReportToPdfAsync(filter);
        //    return result.IsSuccess ? File(result.Value!, "application/pdf", "employee_report.pdf") : BadRequest(result);
        //}

        // Same for Department, Attendance, etc.
    }


}
