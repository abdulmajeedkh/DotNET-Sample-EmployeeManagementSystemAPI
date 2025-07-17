using EmployeeManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface ILeaveService
    {
        Task<ResponseDto<LeaveResponseDto>> CreateAsync(LeaveDto dto);
        Task<ResponseDto<LeaveResponseDto>> UpdateAsync(LeaveDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
        Task<ResponseDto<LeaveResponseDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<List<LeaveResponseDto>>> GetAllAsync();
    }

}
