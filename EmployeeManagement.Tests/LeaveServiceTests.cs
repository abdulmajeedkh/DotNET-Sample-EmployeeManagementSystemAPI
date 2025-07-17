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


    public class LeaveServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<LeaveService>> _logger;
        private readonly LeaveService _service;

        public LeaveServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Isolated DB
                .Options;

            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LeaveDto, Leave>().ReverseMap();
            });

            _mapper = config.CreateMapper();
            _logger = new Mock<ILogger<LeaveService>>();
            _service = new LeaveService(_context, _mapper, _logger.Object);

            SeedTestEmployee();
        }

        private void SeedTestEmployee()
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                TenantId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString()
            };
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateLeaveRequest()
        {
            var employee = await _context.Employees.FirstAsync();

            var dto = new LeaveDto
            {
                EmployeeId = employee.Id,
                TenantId = (Guid)employee.TenantId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                Reason = "Vacation"
            };

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal("Vacation", result.Value?.Reason);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateLeaveRequest()
        {
            // Arrange
            var employee = await _context.Employees.FirstAsync();
            var leave = new Leave
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Reason = "Sick",
                Status = "Pending"
            };
            _context.LeaveRequests.Add(leave);
            await _context.SaveChangesAsync();

            var updateDto = new LeaveDto
            {
                Id = leave.Id,
                EmployeeId = employee.Id,
                TenantId = (Guid)employee.TenantId,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate.AddDays(1),
                Reason = "Updated Reason",
                Status = "Approved",
                ApproverComment = "Get well soon"
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Approved", result.Value?.Status);
            Assert.Equal("Updated Reason", result.Value?.Reason);
            Assert.Equal("Get well soon", result.Value?.ApproverComment);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteLeave_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var leave = new Leave
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Reason = "Delete Test"
            };

            _context.LeaveRequests.Add(leave);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(leave.Id);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            var deleted = await _context.LeaveRequests.FindAsync(leave.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnLeave_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var leave = new Leave
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                Reason = "Get Test"
            };

            _context.LeaveRequests.Add(leave);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(leave.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal("Get Test", result.Value?.Reason);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            var result = await _service.GetAllAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<List<LeaveResponseDto>>(result.Value);
        }
    }


}
