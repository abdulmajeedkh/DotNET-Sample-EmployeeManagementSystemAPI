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
    public class TenantServiceTests
    {
        private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TenantService>> _logger;
    private readonly TenantService _service;

    public TenantServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TenantDto, Tenant>().ReverseMap();
            cfg.CreateMap<Tenant, TenantResponseDto>().ReverseMap();
        });

        _mapper = mapperConfig.CreateMapper();
        _logger = new Mock<ILogger<TenantService>>();
        _service = new TenantService(_context, _mapper, _logger.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddTenant()
    {
        var dto = new TenantDto
        {
            CompanyName = "Piytech Solutions",
            Address = "Karachi, Pakistan",
            ContactEmail = "info@piytech.com",
            PhoneNumber = "+92-3000000000"
        };

        var result = await _service.CreateAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Piytech Solutions", result.Value!.CompanyName);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTenant_WhenExists()
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            CompanyName = "Tech Co",
            Address = "Islamabad",
            ContactEmail = "support@techco.com",
            PhoneNumber = "+92-3111111111",
            CreatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        var updateDto = new TenantDto
        {
            Id = tenant.Id,
            CompanyName = "Tech Co Updated",
            Address = "Lahore",
            ContactEmail = "new@techco.com",
            PhoneNumber = "+92-3222222222"
        };

        var result = await _service.UpdateAsync(updateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Tech Co Updated", result.Value!.CompanyName);
        Assert.Equal("Lahore", result.Value.Address);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTenant_WhenExists()
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            CompanyName = "Delete Me Inc",
            Address = "Rawalpindi",
            ContactEmail = "delete@me.com",
            PhoneNumber = "+92-3333333333",
            CreatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteAsync(tenant.Id);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);

        var deleted = await _context.Tenants.FindAsync(tenant.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTenant_WhenExists()
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            CompanyName = "Tenant Co",
            Address = "Faisalabad",
            ContactEmail = "tenant@co.com",
            PhoneNumber = "+92-3444444444",
            CreatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(tenant.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Tenant Co", result.Value!.CompanyName);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnListOfTenants()
    {
        _context.Tenants.AddRange(
            new Tenant
            {
                Id = Guid.NewGuid(),
                CompanyName = "A",
                Address = "A Address",
                ContactEmail = "a@a.com",
                PhoneNumber = "+92-3000000001",
                CreatedAt = DateTime.UtcNow
            },
            new Tenant
            {
                Id = Guid.NewGuid(),
                CompanyName = "B",
                Address = "B Address",
                ContactEmail = "b@b.com",
                PhoneNumber = "+92-3000000002",
                CreatedAt = DateTime.UtcNow
            }
        );

        await _context.SaveChangesAsync();

        var result = await _service.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
    }
}
}
