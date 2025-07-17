using EmployeeManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Interfaces
{
    public interface ITrainingService
    {
        Task<ResponseDto<TrainingResponseDto>> CreateAsync(TrainingDto dto);
        Task<ResponseDto<TrainingResponseDto>> UpdateAsync(TrainingDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
        Task<ResponseDto<TrainingResponseDto>> GetByIdAsync(Guid id);
        Task<ResponseDto<List<TrainingResponseDto>>> GetAllAsync();
    }

}
