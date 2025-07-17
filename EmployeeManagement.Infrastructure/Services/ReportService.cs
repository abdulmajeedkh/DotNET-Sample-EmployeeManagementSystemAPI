using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.FilterDto;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.ReportDto;
using EmployeeManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EmployeeManagement.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportService> _log;

        public ReportService(AppDbContext ctx, IMapper mapper, ILogger<ReportService> log)
        {
            _ctx = ctx;
            _mapper = mapper;
            _log = log;
        }

        public async Task<ResponseDto<PagedResult<EmployeeReportDto>>> GetEmployeeReportsAsync(EmployeeReportFilterDto filter, int pageIndex, int pageSize, string sortBy, string sortDirection)
        {
            try
            {
                var query = _ctx.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Tenant)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(filter.Name))
                    query = query.Where(e => e.FirstName.Contains(filter.Name) || e.LastName.Contains(filter.Name));

                if (filter.DepartmentId.HasValue)
                    query = query.Where(e => e.DepartmentId == filter.DepartmentId);

                if (filter.TenantId.HasValue)
                    query = query.Where(e => e.TenantId == filter.TenantId);

                // Sorting
                query = sortDirection.ToLower() == "desc"
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));

                var count = await query.CountAsync();
                var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                                       .ProjectTo<EmployeeReportDto>(_mapper.ConfigurationProvider)
                                       .ToListAsync();

                var PagedResult = new PagedResult<EmployeeReportDto>(items, count, pageIndex, pageSize);
                return ResponseDto<PagedResult<EmployeeReportDto>>.Success(PagedResult);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error generating employee report");
                return ResponseDto<PagedResult<EmployeeReportDto>>.Failure("Failed to generate report.");
            }
        }
        public async Task<ResponseDto<byte[]>> ExportEmployeeReportToCsvAsync(EmployeeReportFilterDto filter)
        {
            var employees = await GetEmployeeReportsAsync(filter, 1, int.MaxValue, "FirstName", "asc");

            if (!employees.IsSuccess) return ResponseDto<byte[]>.Failure(employees.Errors!);

            var csv = new StringBuilder();
            csv.AppendLine("FirstName,LastName,Department,Tenant");

            foreach (var emp in employees.Value!.Items)
            {
                csv.AppendLine($"{emp.FirstName},{emp.LastName},{emp.DepartmentName},{emp.TenantName}");
            }

            return ResponseDto<byte[]>.Success(Encoding.UTF8.GetBytes(csv.ToString()));
        }

        // Similar logic for Leave, Payroll, Attendance reports
    }

}
