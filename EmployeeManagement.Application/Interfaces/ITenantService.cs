using EmployeeManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface ITenantService
    {
        Task<ResponseDto<TenantResponseDto>> CreateAsync(TenantDto dto);
        Task<ResponseDto<TenantResponseDto>> UpdateAsync(TenantDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
        Task<ResponseDto<TenantResponseDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<List<TenantResponseDto>>> GetAllAsync();
    }

}
