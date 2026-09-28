using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.Models;
using SchoolApiService.DTOs;
using SchoolApiService.Models;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeTypesController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public EmployeeTypesController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeTypeDto>>>> GetEmployeeTypes()
        {
            var types = await _context.dbsEmployeeType.ToListAsync();
            var dtos = types.Select(t => new EmployeeTypeDto
            {
                EmployeeTypeId = t.EmployeeTypeId,
                EmployeeTypeName = t.EmployeeTypeName ?? string.Empty
            });

            return Ok(ApiResponse<IEnumerable<EmployeeTypeDto>>.SuccessResponse(dtos, "Employee types retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EmployeeTypeDto>>> GetEmployeeType(int id)
        {
            var type = await _context.dbsEmployeeType.FindAsync(id);
            if (type == null)
            {
                return NotFound(ApiResponse<EmployeeTypeDto>.ErrorResponse($"No employee type found with ID {id}.", statusCode: 404));
            }

            var dto = new EmployeeTypeDto
            {
                EmployeeTypeId = type.EmployeeTypeId,
                EmployeeTypeName = type.EmployeeTypeName ?? string.Empty
            };

            return Ok(ApiResponse<EmployeeTypeDto>.SuccessResponse(dto, "Employee type retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutEmployeeType(int id, EmployeeTypeDto dto)
        {
            if (id != dto.EmployeeTypeId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload EmployeeTypeId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.EmployeeTypeId}'." }, 400));
            }

            var entity = await _context.dbsEmployeeType.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No employee type found with ID {id} to update.", statusCode: 404));
            }

            entity.EmployeeTypeName = dto.EmployeeTypeName;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Employee type updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<EmployeeTypeDto>>> PostEmployeeType(EmployeeTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeTypeName))
            {
                return BadRequest(ApiResponse<EmployeeTypeDto>.ErrorResponse("Employee type creation failed.", new List<string> { "EmployeeTypeName is required." }, 400));
            }

            var entity = new EmployeeType
            {
                EmployeeTypeName = dto.EmployeeTypeName
            };

            _context.dbsEmployeeType.Add(entity);
            await _context.SaveChangesAsync();
            dto.EmployeeTypeId = entity.EmployeeTypeId;

            return CreatedAtAction(nameof(GetEmployeeType), new { id = dto.EmployeeTypeId }, ApiResponse<EmployeeTypeDto>.SuccessResponse(dto, "Employee type created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteEmployeeType(int id)
        {
            var entity = await _context.dbsEmployeeType.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No employee type found with ID {id} to delete.", statusCode: 404));
            }

            _context.dbsEmployeeType.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Employee type deleted successfully."));
        }
    }
}
