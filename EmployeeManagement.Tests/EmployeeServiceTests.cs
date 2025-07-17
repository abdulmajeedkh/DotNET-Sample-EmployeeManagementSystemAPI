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
    public class EmployeeServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly EmployeeService _service;
        private readonly Mock<ILogger<EmployeeService>> _logger;

        public EmployeeServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EmployeeDto, Employee>().ReverseMap();
                cfg.CreateMap<Employee, EmployeeResponseDto>()
                    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            });
            _mapper = config.CreateMapper();
            _logger = new Mock<ILogger<EmployeeService>>();

            _service = new EmployeeService(_context,  _logger.Object);
            SeedTestTenantAndDepartment();
        }

        private void SeedTestTenantAndDepartment()
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                CompanyName = "TestCo",
                Address = "Test Address"
            };
            var dept = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Engineering",
                TenantId = tenant.Id
            };
            _context.Tenants.Add(tenant);
            _context.Departments.Add(dept);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddEmployee()
        {
            var dept = await _context.Departments.FirstAsync();
            var dto = new EmployeeDto
            {
                FirstName = "Ali",
                LastName = "Khan",
                //Email = "ali@example.com",
                //Phone = "03123456789",
                DepartmentId = dept.Id,
                TenantId = (Guid)dept.TenantId,
            };

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal("Ali Khan", result.Value!.FullName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEmployee()
        {
            var dept = await _context.Departments.FirstAsync();
            var emp = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Zain",
                LastName = "Ahmed",
                //Email = "zain@abc.com",
                //Phone = "03001234567",
                DepartmentId = dept.Id,
                TenantId = dept.TenantId,
                UserId = Guid.NewGuid().ToString()
            };
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            var dto = new EmployeeDto
            {
                Id = emp.Id,
                FirstName = "Zain Updated",
                LastName = "Ahmed",
                //Email = "updated@abc.com",
                //Phone = "03001234568",
                //DepartmentId = dept.Id,
                //TenantId = dept.TenantId,
                //UserId = emp.UserId
            };

            var result = await _service.UpdateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal("Zain Updated", result.Value!.FirstName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEmployee()
        {
            var emp = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Delete",
                LastName = "Me",
                //Email = "del@me.com",
                //Phone = "03001111111",
                DepartmentId = _context.Departments.First().Id,
                TenantId = _context.Departments.First().TenantId,
                UserId = Guid.NewGuid().ToString()
            };
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(emp.Id);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEmployee()
        {
            var emp = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Get",
                LastName = "Me",
                //Email = "get@me.com",
                //Phone = "03002222222",
                DepartmentId = _context.Departments.First().Id,
                TenantId = _context.Departments.First().TenantId,
                UserId = Guid.NewGuid().ToString()
            };
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(emp.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal("Get Me", result.Value!.FullName);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            var result = await _service.GetAllAsync();

            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Value!);
        }
    }
}
