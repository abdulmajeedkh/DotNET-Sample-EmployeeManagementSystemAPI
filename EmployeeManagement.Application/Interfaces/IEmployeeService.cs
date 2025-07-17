using EmployeeManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    /// <summary>
    /// Exposes CRUD operations for employees.
    /// </summary>
    public interface IEmployeeService
    {
        Task<ResponseDto<EmployeeResponseDto>> CreateAsync(EmployeeDto dto);
        Task<ResponseDto<EmployeeResponseDto>> UpdateAsync(EmployeeDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
        Task<ResponseDto<EmployeeResponseDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<List<EmployeeResponseDto>>> GetAllAsync();
    }


}
