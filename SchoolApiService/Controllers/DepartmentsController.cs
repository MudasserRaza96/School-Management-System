using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.DTOs;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDto>>>> GetDepartments()
        {
            var depts = await _departmentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<DepartmentDto>>.SuccessResponse(depts, "Departments retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetDepartment(int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);
            if (dept == null)
            {
                return NotFound(ApiResponse<DepartmentDto>.ErrorResponse($"No department found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<DepartmentDto>.SuccessResponse(dept, "Department retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutDepartment(int id, DepartmentDto dto)
        {
            if (id != dto.DepartmentId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload DepartmentId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.DepartmentId}'." }, 400));
            }

            var success = await _departmentService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No department found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Department updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<DepartmentDto>>> PostDepartment(DepartmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DepartmentName))
            {
                return BadRequest(ApiResponse<DepartmentDto>.ErrorResponse("Department creation failed.", new List<string> { "DepartmentName is required." }, 400));
            }

            var created = await _departmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDepartment), new { id = created.DepartmentId }, ApiResponse<DepartmentDto>.SuccessResponse(created, "Department created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteDepartment(int id)
        {
            var success = await _departmentService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No department found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Department deleted successfully."));
        }
    }
}
