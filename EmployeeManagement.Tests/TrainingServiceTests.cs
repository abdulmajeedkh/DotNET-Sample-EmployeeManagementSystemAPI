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
    public class TrainingServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<TrainingService>> _logger;
        private readonly TrainingService _service;

        public TrainingServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TrainingDto, Training>().ReverseMap();
            });
            _mapper = mapperConfig.CreateMapper();

            _logger = new Mock<ILogger<TrainingService>>();
            _service = new TrainingService(_context, _mapper, _logger.Object);

            SeedTestEmployee();
        }

        private void SeedTestEmployee()
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Ali",
                LastName = "Khan",
                DepartmentId = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString()
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddTraining()
        {
            var employee = await _context.Employees.FirstAsync();

            var dto = new TrainingDto
            {
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Title = "C# Advanced",
                Description = "Advanced topics in C#",
                ScheduledDate = DateTime.UtcNow.AddDays(7),
                Status = "Scheduled"
            };

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal("C# Advanced", result.Value!.Title);
            Assert.Equal(employee.Id, result.Value.EmployeeId);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyTraining_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();

            var training = new Training
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Title = "Intro to SQL",
                Description = "Basics of SQL",
                ScheduledDate = DateTime.UtcNow.AddDays(2),
                Status = "Scheduled"
            };

            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();

            var updateDto = new TrainingDto
            {
                Id = training.Id,
                EmployeeId = training.EmployeeId,
                TenantId = training.TenantId,
                Title = "Intro to SQL - Updated",
                Description = "Updated content",
                ScheduledDate = training.ScheduledDate.AddDays(2),
                Status = "Completed"
            };

            var result = await _service.UpdateAsync(updateDto);

            Assert.True(result.IsSuccess);
            Assert.Equal("Completed", result.Value!.Status);
            Assert.Equal("Intro to SQL - Updated", result.Value.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTraining_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var training = new Training
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Title = "Azure DevOps",
                Description = "CI/CD",
                ScheduledDate = DateTime.UtcNow,
                Status = "Scheduled"
            };

            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(training.Id);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            var deleted = await _context.Trainings.FindAsync(training.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTraining_WhenExists()
        {
            var employee = await _context.Employees.FirstAsync();
            var training = new Training
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                TenantId = employee.TenantId,
                Title = "Angular Fundamentals",
                Description = "Learn basics of Angular",
                ScheduledDate = DateTime.UtcNow,
                Status = "Scheduled"
            };

            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(training.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal("Angular Fundamentals", result.Value!.Title);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            var result = await _service.GetAllAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<List<TrainingResponseDto>>(result.Value);
        }
    }

}
