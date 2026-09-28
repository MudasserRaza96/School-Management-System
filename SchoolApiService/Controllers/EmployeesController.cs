using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Models;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public EmployeesController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetEmployees()
        {
            var employees = await _context.dbsEmployee.Include(e => e.EmployeeType).ToListAsync();
            var dtos = employees.Select(e => new EmployeeDto
            {
                EmployeeId = e.EmployeeId,
                EmployeeName = e.EmployeeName ?? string.Empty,
                EmployeeTypeId = e.EmployeeTypeId,
                EmployeeTypeName = e.EmployeeType?.EmployeeTypeName,
                ImagePath = e.ImagePath
            });

            return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(dtos, "Employees retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(int id)
        {
            var employee = await _context.dbsEmployee.Include(e => e.EmployeeType).FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound(ApiResponse<EmployeeDto>.ErrorResponse($"No employee found with ID {id}.", statusCode: 404));
            }

            var dto = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName ?? string.Empty,
                EmployeeTypeId = employee.EmployeeTypeId,
                EmployeeTypeName = employee.EmployeeType?.EmployeeTypeName,
                ImagePath = employee.ImagePath
            };

            return Ok(ApiResponse<EmployeeDto>.SuccessResponse(dto, "Employee retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> PostEmployee(EmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeName))
            {
                return BadRequest(ApiResponse<EmployeeDto>.ErrorResponse("Employee creation failed.", new List<string> { "EmployeeName is required." }, 400));
            }

            var entity = new Employee
            {
                EmployeeName = dto.EmployeeName,
                EmployeeTypeId = dto.EmployeeTypeId,
                ImagePath = dto.ImagePath
            };

            _context.dbsEmployee.Add(entity);
            await _context.SaveChangesAsync();
            dto.EmployeeId = entity.EmployeeId;

            return CreatedAtAction(nameof(GetEmployee), new { id = dto.EmployeeId }, ApiResponse<EmployeeDto>.SuccessResponse(dto, "Employee created successfully.", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutEmployee(int id, EmployeeDto dto)
        {
            if (id != dto.EmployeeId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload EmployeeId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.EmployeeId}'." }, 400));
            }

            var entity = await _context.dbsEmployee.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No employee found with ID {id} to update.", statusCode: 404));
            }

            entity.EmployeeName = dto.EmployeeName;
            entity.EmployeeTypeId = dto.EmployeeTypeId;
            entity.ImagePath = dto.ImagePath;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Employee updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteEmployee(int id)
        {
            var entity = await _context.dbsEmployee.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No employee found with ID {id} to delete.", statusCode: 404));
            }

            _context.dbsEmployee.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Employee deleted successfully."));
        }
    }
}
