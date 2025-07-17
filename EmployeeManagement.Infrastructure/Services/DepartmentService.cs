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
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentService> _log;

        public DepartmentService(AppDbContext ctx, IMapper mapper, ILogger<DepartmentService> log)
        {
            _ctx = ctx;
            _mapper = mapper;
            _log = log;
        }

        public async Task<ResponseDto<DepartmentResponseDto>> CreateAsync(DepartmentDto dto)
        {
            try
            {
                var entity = _mapper.Map<Department>(dto);
                entity.Id = Guid.NewGuid();

                _ctx.Departments.Add(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<DepartmentResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error creating department");
                return ResponseDto<DepartmentResponseDto>.Failure("Error creating department");
            }
        }

        public async Task<ResponseDto<DepartmentResponseDto>> UpdateAsync(DepartmentDto dto)
        {
            try
            {
                var entity = await _ctx.Departments.FindAsync(dto.Id);
                if (entity == null)
                    return ResponseDto<DepartmentResponseDto>.Failure("Department not found");

                entity.Name = dto.Name;

                await _ctx.SaveChangesAsync();
                return ResponseDto<DepartmentResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating department");
                return ResponseDto<DepartmentResponseDto>.Failure("Error updating department");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _ctx.Departments.FindAsync(id);
                if (entity == null)
                    return ResponseDto<bool>.Failure("Department not found");

                _ctx.Departments.Remove(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error deleting department");
                return ResponseDto<bool>.Failure("Error deleting department");
            }
        }

        public async Task<ResponseDto<DepartmentResponseDto>> GetByIdAsync(Guid id)
        {
            var response = await MapToResponseAsync(id);
            if (response == null)
                return ResponseDto<DepartmentResponseDto>.Failure("Department not found");

            return ResponseDto<DepartmentResponseDto>.Success(response);
        }

        public async Task<ResponseDto<List<DepartmentResponseDto>>> GetAllAsync()
        {
            var list = await _ctx.Departments.Include(x => x.Tenant).ToListAsync();
            var result = list.Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name,
              //  TenantId = d.TenantId,
                TenantName = d.Tenant?.CompanyName ?? "N/A"
            }).ToList();

            return ResponseDto<List<DepartmentResponseDto>>.Success(result);
        }

        private async Task<DepartmentResponseDto?> MapToResponseAsync(Guid id)
        {
            var dept = await _ctx.Departments.Include(x => x.Tenant).FirstOrDefaultAsync(x => x.Id == id);
            if (dept == null) return null;

            return new DepartmentResponseDto
            {
                Id = dept.Id,
                Name = dept.Name,
               // TenantId = dept.TenantId,
                TenantName = dept.Tenant?.CompanyName ?? "N/A"
            };
        }
    }

}
