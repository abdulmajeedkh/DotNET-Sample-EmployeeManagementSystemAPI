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
    public class PayrollService : IPayrollService
    {
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<PayrollService> _log;

        public PayrollService(AppDbContext ctx, IMapper mapper, ILogger<PayrollService> log)
        {
            _ctx = ctx;
            _mapper = mapper;
            _log = log;
        }

        public async Task<ResponseDto<PayrollResponseDto>> CreateAsync(PayrollDto dto)
        {
            try
            {
                var entity = _mapper.Map<Payroll>(dto);
                entity.Id = Guid.NewGuid();
                entity.NetSalary = dto.BasicSalary + dto.Allowances - dto.Deductions;

                _ctx.Payrolls.Add(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<PayrollResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error generating payroll");
                return ResponseDto<PayrollResponseDto>.Failure("Error creating payroll");
            }
        }

        public async Task<ResponseDto<PayrollResponseDto>> UpdateAsync(PayrollDto dto)
        {
            try
            {
                var entity = await _ctx.Payrolls.FindAsync(dto.Id);
                if (entity == null)
                    return ResponseDto<PayrollResponseDto>.Failure("Payroll not found");

                entity.BasicSalary = dto.BasicSalary;
                entity.Allowances = dto.Allowances;
                entity.Deductions = dto.Deductions;
                entity.NetSalary = dto.BasicSalary + dto.Allowances - dto.Deductions;
                entity.Year = dto.Year;
                entity.Month = dto.Month;
                entity.EmployeeId = dto.EmployeeId;
                entity.TenantId = dto.TenantId;

                await _ctx.SaveChangesAsync();
                return ResponseDto<PayrollResponseDto>.Success(await MapToResponseAsync(entity.Id));
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error updating payroll");
                return ResponseDto<PayrollResponseDto>.Failure("Error updating payroll");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _ctx.Payrolls.FindAsync(id);
                if (entity == null)
                    return ResponseDto<bool>.Failure("Payroll not found");

                _ctx.Payrolls.Remove(entity);
                await _ctx.SaveChangesAsync();

                return ResponseDto<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error deleting payroll");
                return ResponseDto<bool>.Failure("Error deleting payroll");
            }
        }

        public async Task<ResponseDto<PayrollResponseDto>> GetByIdAsync(Guid id)
        {
            var entity = await MapToResponseAsync(id);
            return entity == null
                ? ResponseDto<PayrollResponseDto>.Failure("Payroll not found")
                : ResponseDto<PayrollResponseDto>.Success(entity);
        }

        public async Task<ResponseDto<List<PayrollResponseDto>>> GetAllAsync()
        {
            var data = await _ctx.Payrolls.Include(x => x.Employee).ToListAsync();
            var list = data.Select(x => new PayrollResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = $"{x.Employee?.FirstName} {x.Employee?.LastName}",
                BasicSalary = x.BasicSalary,
                Allowances = x.Allowances,
                Deductions = x.Deductions,
                NetSalary = x.NetSalary,
                Year = x.Year,
                Month = x.Month,
                GeneratedOn = x.GeneratedOn,
                TenantId = (Guid)x.TenantId
            }).ToList();

            return ResponseDto<List<PayrollResponseDto>>.Success(list);
        }

        private async Task<PayrollResponseDto?> MapToResponseAsync(Guid id)
        {
            var p = await _ctx.Payrolls.Include(x => x.Employee).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return null;

            return new PayrollResponseDto
            {
                Id = p.Id,
                EmployeeId = p.EmployeeId,
                EmployeeName = $"{p.Employee?.FirstName} {p.Employee?.LastName}",
                Year = p.Year,
                Month = p.Month,
                BasicSalary = p.BasicSalary,
                Allowances = p.Allowances,
                Deductions = p.Deductions,
                NetSalary = p.NetSalary,
                GeneratedOn = p.GeneratedOn,
                TenantId = (Guid)p.TenantId
            };
        }
    }

}
