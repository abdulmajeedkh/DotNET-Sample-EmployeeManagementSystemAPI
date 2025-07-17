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
    public class DepartmentServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DepartmentDto, Department>().ReverseMap();
            });

            _mapper = config.CreateMapper();

            var logger = new Mock<ILogger<DepartmentService>>();
            _service = new DepartmentService(_context, _mapper, logger.Object);

            SeedTenant();
        }

        private void SeedTenant()
        {
            _context.Tenants.Add(new Tenant
            {
                Id = Guid.NewGuid(),
                CompanyName = "DeptCo",
                Address = "Somewhere"
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldAddDepartment()
        {
            var tenant = await _context.Tenants.FirstAsync();
            var dto = new DepartmentDto
            {
                Name = "HR",
                Description = "Human Resources",
                TenantId = tenant.Id
            };

            var result = await _service.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal("HR", result.Value!.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDepartment()
        {
            var tenant = await _context.Tenants.FirstAsync();
            var dept = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Finance",
                Description = "Finance Dept",
                TenantId = tenant.Id
            };
            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();

            var updateDto = new DepartmentDto
            {
                Id = dept.Id,
                Name = "Finance Updated",
                Description = "Updated Desc",
                TenantId = (Guid)dept.TenantId
            };

            var result = await _service.UpdateAsync(updateDto);

            Assert.True(result.IsSuccess);
            Assert.Equal("Finance Updated", result.Value!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveDepartment()
        {
            var dept = new Department
            {
                Id = Guid.NewGuid(),
                Name = "DeleteDept",
                Description = "Temp",
                TenantId = _context.Tenants.First().Id
            };

            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(dept.Id);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDepartment()
        {
            var dept = new Department
            {
                Id = Guid.NewGuid(),
                Name = "GetDept",
                Description = "Return me",
                TenantId = _context.Tenants.First().Id
            };

            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(dept.Id);

            Assert.True(result.IsSuccess);
            Assert.Equal("GetDept", result.Value!.Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnList()
        {
            var result = await _service.GetAllAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }
    }

}
