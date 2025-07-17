using EmployeeManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface IPayrollService
    {
        Task<ResponseDto<PayrollResponseDto>> CreateAsync(PayrollDto dto);
        Task<ResponseDto<PayrollResponseDto>> UpdateAsync(PayrollDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
        Task<ResponseDto<PayrollResponseDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<List<PayrollResponseDto>>> GetAllAsync();
    }

}
