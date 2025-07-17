using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.ReportDto;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Helpers
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<EmployeeDto, Employee>().ReverseMap();
            CreateMap<DepartmentDto, Department>().ReverseMap();
            CreateMap<AttendanceDto, Attendance>().ReverseMap();
            CreateMap<LeaveDto, Leave>().ReverseMap();
            CreateMap<PayrollDto, Payroll>().ReverseMap();
            CreateMap<TrainingDto, Training>().ReverseMap();

            CreateMap<TenantDto, Tenant>().ReverseMap();
            CreateMap<Tenant, TenantResponseDto>().ReverseMap();

            CreateMap<Employee, EmployeeResponseDto>();
            CreateMap<Employee, EmployeeReportDto>()
    .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department!.Name))
    .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant!.CompanyName));



        }
    }
}
