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
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _svc;
        private readonly ILogger<EmployeesController> _log;

        public EmployeesController(IEmployeeService svc, ILogger<EmployeesController> log)
        {
            _svc = svc; _log = log;
        }

        /// <summary>Get all employees.</summary>
        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<EmployeeResponseDto>>>> GetAll()
            => Ok(await _svc.GetAllAsync());

        /// <summary>Get employee by Id.</summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResponseDto<EmployeeResponseDto>>> GetById(Guid id)
            => Ok(await _svc.GetByIdAsync(id));

        /// <summary>Create a new employee.</summary>
        [HttpPost]
        public async Task<ActionResult<ResponseDto<EmployeeResponseDto>>> Create(EmployeeDto dto)
            => Ok(await _svc.CreateAsync(dto));

        /// <summary>Update an existing employee.</summary>
        [HttpPut]
        public async Task<ActionResult<ResponseDto<EmployeeResponseDto>>> Update(EmployeeDto dto)
            => Ok(await _svc.UpdateAsync(dto));

        /// <summary>Delete an employee.</summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ResponseDto<bool>>> Delete(Guid id)
            => Ok(await _svc.DeleteAsync(id));
    }

}
