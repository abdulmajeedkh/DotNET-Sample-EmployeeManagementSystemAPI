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
    public class AttendanceServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<AttendanceService>> _mockLogger;
        private readonly AttendanceService _service;

        public AttendanceServiceTests()
        {
            // Setup InMemory DB
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            // Seed test data
            SeedDatabase();

            // Setup AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AttendanceDto, Attendance>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _mockLogger = new Mock<ILogger<AttendanceService>>();
            _service = new AttendanceService(_context, _mapper, _mockLogger.Object);
        }

        private void SeedDatabase()
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DepartmentId = Guid.NewGuid(),
               // TenantId = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString()
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateAttendance_WhenValid()
        {
            var employee = await _context.Employees.FirstAsync();

            var dto = new AttendanceDto
            {
                EmployeeId = employee.Id,
               // TenantId = employee.TenantId,
                Date = DateTime.Today,
                CheckInTime = new TimeSpan(9, 0, 0),
                CheckOutTime = new TimeSpan(17, 0, 0),
                Status = "Present"
            };

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(dto.EmployeeId, result.Value.EmployeeId);
            Assert.Equal("Present", result.Value.Status);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAttendance_WhenExists()
        {
            // Arrange - create first
            var employee = await _context.Employees.FirstAsync();
            var attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Date = DateTime.Today,
                CheckInTime = new TimeSpan(9, 0, 0),
                CheckOutTime = new TimeSpan(17, 0, 0),
                Status = "Present"
            };
            _context.AttendanceRecords.Add(attendance);
            await _context.SaveChangesAsync();

            var dto = new AttendanceDto
            {
                Id = attendance.Id,
                EmployeeId = employee.Id,
              //  TenantId = employee.TenantId,
                Date = attendance.Date,
                CheckInTime = new TimeSpan(8, 30, 0),
                CheckOutTime = new TimeSpan(16, 30, 0),
                Status = "Late"
            };

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Late", result.Value?.Status);
            Assert.Equal(new TimeSpan(8, 30, 0), result.Value?.CheckInTime);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAttendance_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Date = DateTime.Today,
                CheckInTime = new TimeSpan(9, 0, 0),
                Status = "Present"
            };
            _context.AttendanceRecords.Add(attendance);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(attendance.Id);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            var deleted = await _context.AttendanceRecords.FindAsync(attendance.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAttendance_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Date = DateTime.Today,
                CheckInTime = new TimeSpan(9, 0, 0),
                Status = "Present"
            };
            _context.AttendanceRecords.Add(attendance);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(attendance.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal(attendance.Id, result.Value?.Id);
            Assert.Equal("Present", result.Value?.Status);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            var result = await _service.GetAllAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.Count >= 0);
        }
    }
    }
