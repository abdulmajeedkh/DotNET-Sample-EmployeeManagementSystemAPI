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
    public class LeaveService : ILeaveService
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<LeaveService> _log;

        public LeaveService(AppDbContext ctx, IMapper mapper, ILogger<LeaveService> log)
        {
            _ctx = ctx;
            _mapper = mapper;
            _log = log;
        }

        public async Task<ResponseDto<LeaveResponseDto>> CreateAsync(LeaveDto dto)
        {
            try
            {
                var entity = _mapper.Map<Leave>(dto);
                entity.Id = Guid.NewGuid();

                _ctx.LeaveRequests.Add(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<LeaveResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error creating leave request");
                return ResponseDto<LeaveResponseDto>.Failure("Error creating leave request");
            }
        }

        public async Task<ResponseDto<LeaveResponseDto>> UpdateAsync(LeaveDto dto)
        {
            try
            {
                var entity = await _ctx.LeaveRequests.FindAsync(dto.Id);
                if (entity == null)
                    return ResponseDto<LeaveResponseDto>.Failure("Leave request not found");

                entity.StartDate = dto.StartDate;
                entity.EndDate = dto.EndDate;
                entity.Reason = dto.Reason;
                entity.Status = dto.Status;
                entity.ApproverComment = dto.ApproverComment;
                entity.EmployeeId = dto.EmployeeId;
                entity.TenantId = dto.TenantId;

                await _ctx.SaveChangesAsync();
                return ResponseDto<LeaveResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating leave request");
                return ResponseDto<LeaveResponseDto>.Failure("Error updating leave request");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _ctx.LeaveRequests.FindAsync(id);
                if (entity == null)
                    return ResponseDto<bool>.Failure("Leave request not found");

                _ctx.LeaveRequests.Remove(entity);
                await _ctx.SaveChangesAsync();
                return ResponseDto<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error deleting leave request");
                return ResponseDto<bool>.Failure("Error deleting leave request");
            }
        }

        public async Task<ResponseDto<LeaveResponseDto>> GetByIdAsync(Guid id)
        {
            var response = await MapToResponseAsync(id);
            return response == null
                ? ResponseDto<LeaveResponseDto>.Failure("Leave request not found")
                : ResponseDto<LeaveResponseDto>.Success(response);
        }

        public async Task<ResponseDto<List<LeaveResponseDto>>> GetAllAsync()
        {
            var data = await _ctx.LeaveRequests.Include(x => x.Employee).ToListAsync();
            var result = data.Select(x => new LeaveResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = $"{x.Employee?.FirstName} {x.Employee?.LastName}",
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Reason = x.Reason,
                Status = x.Status,
                ApproverComment = x.ApproverComment,
                TenantId = (Guid)x.TenantId
            }).ToList();

            return ResponseDto<List<LeaveResponseDto>>.Success(result);
        }

        private async Task<LeaveResponseDto?> MapToResponseAsync(Guid id)
        {
            var entity = await _ctx.LeaveRequests.Include(x => x.Employee).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;

            return new LeaveResponseDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                EmployeeName = $"{entity.Employee?.FirstName} {entity.Employee?.LastName}",
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Reason = entity.Reason,
                Status = entity.Status,
                ApproverComment = entity.ApproverComment,
                TenantId = (Guid)entity.TenantId
            };
        }
    }

}
