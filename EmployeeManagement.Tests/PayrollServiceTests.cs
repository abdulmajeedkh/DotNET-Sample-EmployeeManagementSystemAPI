using AutoMapper;
using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests
{
    public class PayrollServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<PayrollService>> _mockLogger;
        private readonly PayrollService _service;

        public PayrollServiceTests()
        {
            // Setup InMemory EF context
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Setup AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PayrollDto, Payroll>().ReverseMap();
            });
            _mapper = mapperConfig.CreateMapper();

            _mockLogger = new Mock<ILogger<PayrollService>>();
            _service = new PayrollService(_context, _mapper, _mockLogger.Object);

            SeedTestEmployee();
        }

        private void SeedTestEmployee()
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DepartmentId = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString()
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreatePayroll()
        {
            var employee = await _context.Employees.FirstAsync();

            var dto = new PayrollDto
            {
                EmployeeId = employee.Id,
                TenantId = (Guid)employee.TenantId,
                Year = 2025,
                Month = 7,
                BasicSalary = 100000,
                Allowances = 20000,
                Deductions = 5000
            };

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(115000, result.Value!.NetSalary);
            Assert.Equal(employee.Id, result.Value.EmployeeId);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePayroll_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var payroll = new Payroll
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Year = 2025,
                Month = 7,
                BasicSalary = 100000,
                Allowances = 20000,
                Deductions = 5000,
                NetSalary = 115000
            };

            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();

            var updateDto = new PayrollDto
            {
                Id = payroll.Id,
                EmployeeId = employee.Id,
                TenantId = (Guid)employee.TenantId,
                Year = 2025,
                Month = 8,
                BasicSalary = 120000,
                Allowances = 15000,
                Deductions = 8000
            };

            var result = await _service.UpdateAsync(updateDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(127000, result.Value!.NetSalary);
            Assert.Equal(8, result.Value.Month);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemovePayroll_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var payroll = new Payroll
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Year = 2025,
                Month = 7,
                BasicSalary = 90000,
                Allowances = 10000,
                Deductions = 4000,
                NetSalary = 96000
            };

            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(payroll.Id);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            var deleted = await _context.Payrolls.FindAsync(payroll.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPayroll_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var payroll = new Payroll
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Year = 2025,
                Month = 6,
                BasicSalary = 95000,
                Allowances = 5000,
                Deductions = 3000,
                NetSalary = 97000
            };

            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(payroll.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal(97000, result.Value!.NetSalary);
            Assert.Equal(6, result.Value.Month);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            var result = await _service.GetAllAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<List<PayrollResponseDto>>(result.Value);
        }
    }
}
