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
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<AttendanceService> _log;

        public AttendanceService(AppDbContext ctx, IMapper mapper, ILogger<AttendanceService> log)
        {
            _ctx = ctx;
            _mapper = mapper;
            _log = log;
        }

        public async Task<ResponseDto<AttendanceResponseDto>> CreateAsync(AttendanceDto dto)
        {
            try
            {
                var entity = _mapper.Map<Attendance>(dto);
                entity.Id = Guid.NewGuid();

                _ctx.AttendanceRecords.Add(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<AttendanceResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error creating attendance record");
                return ResponseDto<AttendanceResponseDto>.Failure("Error creating attendance");
            }
        }

        public async Task<ResponseDto<AttendanceResponseDto>> UpdateAsync(AttendanceDto dto)
        {
            try
            {
                var entity = await _ctx.AttendanceRecords.FindAsync(dto.Id);
                if (entity == null)
                    return ResponseDto<AttendanceResponseDto>.Failure("Attendance not found");

                entity.CheckInTime = dto.CheckInTime;
                entity.CheckOutTime = dto.CheckOutTime;
                entity.Date = dto.Date;
                entity.Status = dto.Status;
                entity.EmployeeId = dto.EmployeeId;
                entity.TenantId = dto.TenantId;

                await _ctx.SaveChangesAsync();
                return ResponseDto<AttendanceResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating attendance record");
                return ResponseDto<AttendanceResponseDto>.Failure("Error updating attendance");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _ctx.AttendanceRecords.FindAsync(id);
                if (entity == null)
                    return ResponseDto<bool>.Failure("Attendance not found");

                _ctx.AttendanceRecords.Remove(entity);
                await _ctx.SaveChangesAsync();
                return ResponseDto<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error deleting attendance");
                return ResponseDto<bool>.Failure("Error deleting attendance");
            }
        }

        public async Task<ResponseDto<AttendanceResponseDto>> GetByIdAsync(Guid id)
        {
            var data = await MapToResponseAsync(id);
            if (data == null)
                return ResponseDto<AttendanceResponseDto>.Failure("Attendance not found");

            return ResponseDto<AttendanceResponseDto>.Success(data);
        }

        public async Task<ResponseDto<List<AttendanceResponseDto>>> GetAllAsync()
        {
            var list = await _ctx.AttendanceRecords
                .Include(a => a.Employee)
                .ToListAsync();

            var result = list.Select(a => new AttendanceResponseDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = $"{a.Employee?.FirstName} {a.Employee?.LastName}",
                Date = a.Date,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Status = a.Status,
                TenantId = (Guid)a.TenantId
            }).ToList();

            return ResponseDto<List<AttendanceResponseDto>>.Success(result);
        }

        private async Task<AttendanceResponseDto?> MapToResponseAsync(Guid id)
        {
            var a = await _ctx.AttendanceRecords.Include(x => x.Employee).FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return null;

            return new AttendanceResponseDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = $"{a.Employee?.FirstName} {a.Employee?.LastName}",
                Date = a.Date,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Status = a.Status,
                TenantId = (Guid)a.TenantId
            };
        }
    }

}
