using EmployeeManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<ResponseDto<DepartmentResponseDto>> CreateAsync(DepartmentDto dto);
        Task<ResponseDto<DepartmentResponseDto>> UpdateAsync(DepartmentDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
        Task<ResponseDto<DepartmentResponseDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<List<DepartmentResponseDto>>> GetAllAsync();
    }

}
