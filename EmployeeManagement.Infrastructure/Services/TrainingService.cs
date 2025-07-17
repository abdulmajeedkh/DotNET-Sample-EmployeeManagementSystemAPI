using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Infrastructure.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<TrainingService> _log;

        public TrainingService(AppDbContext ctx, IMapper mapper, ILogger<TrainingService> log)
        {
            _ctx = ctx;
            _mapper = mapper;
            _log = log;
        }

        public async Task<ResponseDto<TrainingResponseDto>> CreateAsync(TrainingDto dto)
        {
            try
            {
                var entity = _mapper.Map<Training>(dto);
                entity.Id = Guid.NewGuid();

                _ctx.Trainings.Add(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<TrainingResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error creating training");
                return ResponseDto<TrainingResponseDto>.Failure("Error creating training");
            }
        }

        public async Task<ResponseDto<TrainingResponseDto>> UpdateAsync(TrainingDto dto)
        {
            try
            {
                var entity = await _ctx.Trainings.FindAsync(dto.Id);
                if (entity == null)
                    return ResponseDto<TrainingResponseDto>.Failure("Training not found");

                entity.Title = dto.Title;
                entity.Description = dto.Description;
                entity.ScheduledDate = dto.ScheduledDate;
                entity.Status = dto.Status;
                entity.EmployeeId = dto.EmployeeId;
                entity.TenantId = dto.TenantId;

                await _ctx.SaveChangesAsync();
                return ResponseDto<TrainingResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating training");
                return ResponseDto<TrainingResponseDto>.Failure("Error updating training");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _ctx.Trainings.FindAsync(id);
                if (entity == null)
                    return ResponseDto<bool>.Failure("Training not found");

                _ctx.Trainings.Remove(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error deleting training");
                return ResponseDto<bool>.Failure("Error deleting training");
            }
        }

        public async Task<ResponseDto<TrainingResponseDto>> GetByIdAsync(Guid id)
        {
            var entity = await MapToResponseAsync(id);
            return entity == null
                ? ResponseDto<TrainingResponseDto>.Failure("Training not found")
                : ResponseDto<TrainingResponseDto>.Success(entity);
        }

        public async Task<ResponseDto<List<TrainingResponseDto>>> GetAllAsync()
        {
            var data = await _ctx.Trainings.Include(x => x.Employee).ToListAsync();
            var list = data.Select(x => new TrainingResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ScheduledDate = x.ScheduledDate,
                Status = x.Status,
                EmployeeId = x.EmployeeId,
                EmployeeName = $"{x.Employee?.FirstName} {x.Employee?.LastName}",
                TenantId = x.TenantId
            }).ToList();

            return ResponseDto<List<TrainingResponseDto>>.Success(list);
        }

        private async Task<TrainingResponseDto?> MapToResponseAsync(Guid id)
        {
            var x = await _ctx.Trainings.Include(t => t.Employee).FirstOrDefaultAsync(t => t.Id == id);
            if (x == null) return null;

            return new TrainingResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ScheduledDate = x.ScheduledDate,
                Status = x.Status,
                EmployeeId = x.EmployeeId,
                EmployeeName = $"{x.Employee?.FirstName} {x.Employee?.LastName}",
                TenantId = x.TenantId
            };
        }
    }

}
