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

        /// <summary>
        /// Service layer for managing employee operations.
        /// </summary>
        public class EmployeeService : IEmployeeService
        {
            private readonly AppDbContext _ctx;
            private readonly ILogger<EmployeeService> _log;

            public EmployeeService(AppDbContext ctx, ILogger<EmployeeService> log)
            {
                _ctx = ctx;
                _log = log;
            }

            /// <summary>
            /// Creates a new employee.
            /// </summary>
            public async Task<ResponseDto<EmployeeResponseDto>> CreateAsync(EmployeeDto dto)
            {
                try
                {
                    var entity = new Employee
                    {
                        Id = Guid.NewGuid(),
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        DateOfBirth = dto.DateOfBirth,
                        DateJoined = dto.DateJoined,
                        DepartmentId = dto.DepartmentId,
                        TenantId = dto.TenantId,
                        //UserId = "user-id-placeholder" // Ideally from IdentityUser
                    };

                    _ctx.Employees.Add(entity);
                    await _ctx.SaveChangesAsync();

                    _log.LogInformation("Employee {EmployeeId} created successfully", entity.Id);
                    return ResponseDto<EmployeeResponseDto>.Success(await MapToResponseAsync(entity.Id));
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error creating employee");
                    return ResponseDto<EmployeeResponseDto>.Failure("Error creating employee");
                }
            }

            /// <summary>
            /// Updates an existing employee.
            /// </summary>
            public async Task<ResponseDto<EmployeeResponseDto>> UpdateAsync(EmployeeDto dto)
            {
                try
                {
                    var entity = await _ctx.Employees.FirstOrDefaultAsync(e => e.Id == dto.Id);
                    if (entity == null)
                    {
                        _log.LogWarning("Employee {EmployeeId} not found for update", dto.Id);
                        return ResponseDto<EmployeeResponseDto>.Failure("Employee not found");
                    }

                    entity.FirstName = dto.FirstName;
                    entity.LastName = dto.LastName;
                    entity.DateOfBirth = dto.DateOfBirth;
                    entity.DateJoined = dto.DateJoined;
                    entity.DepartmentId = dto.DepartmentId;
                    entity.TenantId = dto.TenantId;

                    await _ctx.SaveChangesAsync();
                    _log.LogInformation("Employee {EmployeeId} updated successfully", entity.Id);
                    return ResponseDto<EmployeeResponseDto>.Success(await MapToResponseAsync(entity.Id));
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error updating employee");
                    return ResponseDto<EmployeeResponseDto>.Failure("Error updating employee");
                }
            }

            /// <summary>
            /// Deletes an employee by Id.
            /// </summary>
            public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
            {
                try
                {
                    var entity = await _ctx.Employees.FindAsync(id);
                    if (entity == null)
                    {
                        _log.LogWarning("Employee {EmployeeId} not found for deletion", id);
                        return ResponseDto<bool>.Failure("Employee not found");
                    }

                    _ctx.Employees.Remove(entity);
                    await _ctx.SaveChangesAsync();
                    _log.LogInformation("Employee {EmployeeId} deleted successfully", id);
                    return ResponseDto<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error deleting employee");
                    return ResponseDto<bool>.Failure("Error deleting employee");
                }
            }

            /// <summary>
            /// Retrieves a single employee by Id.
            /// </summary>
            public async Task<ResponseDto<EmployeeResponseDto>> GetByIdAsync(Guid id)
            {
                try
                {
                    var dto = await MapToResponseAsync(id);
                    if (dto == null)
                    {
                        _log.LogWarning("Employee {EmployeeId} not found", id);
                        return ResponseDto<EmployeeResponseDto>.Failure("Employee not found");
                    }

                    return ResponseDto<EmployeeResponseDto>.Success(dto);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error fetching employee by ID");
                    return ResponseDto<EmployeeResponseDto>.Failure("Error fetching employee");
                }
            }

            /// <summary>
            /// Retrieves all employees.
            /// </summary>
            public async Task<ResponseDto<List<EmployeeResponseDto>>> GetAllAsync()
            {
                try
                {
                    var employees = await _ctx.Employees
                        .Include(e => e.Department)
                        .Include(e => e.Tenant)
                        .ToListAsync();

                    var responseList = employees.Select(e => new EmployeeResponseDto
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        DateOfBirth = e.DateOfBirth,
                        DateJoined = e.DateJoined,
                        DepartmentId = e.DepartmentId,
                        TenantId = (Guid)e.TenantId,
                        DepartmentName = e.Department?.Name ?? "N/A",
                        TenantName = e.Tenant?.CompanyName ?? "N/A",
                        UserId = e.UserId
                    }).ToList();

                    return ResponseDto<List<EmployeeResponseDto>>.Success(responseList);
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error retrieving employees");
                    return ResponseDto<List<EmployeeResponseDto>>.Failure("Error retrieving employees");
                }
            }

            /// <summary>
            /// Maps an employee entity to a response DTO by ID.
            /// </summary>
            private async Task<EmployeeResponseDto?> MapToResponseAsync(Guid id)
            {
                var e = await _ctx.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Tenant)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (e == null) return null;

                return new EmployeeResponseDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DateOfBirth = e.DateOfBirth,
                    DateJoined = e.DateJoined,
                    DepartmentId = e.DepartmentId,
                    TenantId = (Guid)e.TenantId,
                    DepartmentName = e.Department?.Name ?? "N/A",
                    TenantName = e.Tenant?.CompanyName ?? "N/A",
                    UserId = e.UserId
                };
            }
        }
    

}
