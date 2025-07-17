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
    public class TenantService : ITenantService
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<TenantService> _log;

        public TenantService(AppDbContext ctx, IMapper mapper, ILogger<TenantService> log)
        {
            _ctx = ctx;
            _mapper = mapper;
            _log = log;
        }

        public async Task<ResponseDto<TenantResponseDto>> CreateAsync(TenantDto dto)
        {
            try
            {
                var entity = _mapper.Map<Tenant>(dto);
                entity.Id = Guid.NewGuid();
                entity.CreatedAt = DateTime.UtcNow;

                _ctx.Tenants.Add(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<TenantResponseDto>.Success(_mapper.Map<TenantResponseDto>(entity));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error creating tenant");
                return ResponseDto<TenantResponseDto>.Failure("Error creating tenant");
            }
        }

        public async Task<ResponseDto<TenantResponseDto>> UpdateAsync(TenantDto dto)
        {
            try
            {
                var entity = await _ctx.Tenants.FindAsync(dto.Id);
                if (entity == null)
                    return ResponseDto<TenantResponseDto>.Failure("Tenant not found");

                _mapper.Map(dto, entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<TenantResponseDto>.Success(_mapper.Map<TenantResponseDto>(entity));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating tenant");
                return ResponseDto<TenantResponseDto>.Failure("Error updating tenant");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _ctx.Tenants.FindAsync(id);
                if (entity == null)
                    return ResponseDto<bool>.Failure("Tenant not found");

                _ctx.Tenants.Remove(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error deleting tenant");
                return ResponseDto<bool>.Failure("Error deleting tenant");
            }
        }

        public async Task<ResponseDto<TenantResponseDto>> GetByIdAsync(Guid id)
        {
            var tenant = await _ctx.Tenants.FindAsync(id);
            if (tenant == null)
                return ResponseDto<TenantResponseDto>.Failure("Tenant not found");

            return ResponseDto<TenantResponseDto>.Success(_mapper.Map<TenantResponseDto>(tenant));
        }

        public async Task<ResponseDto<List<TenantResponseDto>>> GetAllAsync()
        {
            var data = await _ctx.Tenants.ToListAsync();
            return ResponseDto<List<TenantResponseDto>>.Success(
                _mapper.Map<List<TenantResponseDto>>(data));
        }
    }

}
