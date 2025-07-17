using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _svc;

        public AttendanceController(IAttendanceService svc) => _svc = svc;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id) => Ok(await _svc.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(AttendanceDto dto) => Ok(await _svc.CreateAsync(dto));

        [HttpPut]
        public async Task<IActionResult> Update(AttendanceDto dto) => Ok(await _svc.UpdateAsync(dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id) => Ok(await _svc.DeleteAsync(id));
    }

}
