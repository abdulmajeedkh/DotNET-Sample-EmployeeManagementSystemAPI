using EmployeeManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task<ResponseDto<AttendanceResponseDto>> CreateAsync(AttendanceDto dto);
        Task<ResponseDto<AttendanceResponseDto>> UpdateAsync(AttendanceDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
        Task<ResponseDto<AttendanceResponseDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<List<AttendanceResponseDto>>> GetAllAsync();
    }

}
