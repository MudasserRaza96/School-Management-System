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
    public class StaffSalariesController : ControllerBase
    {
        private readonly IStaffSalaryService _staffSalaryService;

        public StaffSalariesController(IStaffSalaryService staffSalaryService)
        {
            _staffSalaryService = staffSalaryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<StaffSalaryDto>>>> GetStaffSalaries()
        {
            var salaries = await _staffSalaryService.GetAllStaffSalariesAsync();
            return Ok(ApiResponse<IEnumerable<StaffSalaryDto>>.SuccessResponse(salaries, "Staff salaries retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StaffSalaryDto>>> GetStaffSalary(int id)
        {
            var salary = await _staffSalaryService.GetStaffSalaryByIdAsync(id);
            if (salary == null)
            {
                return NotFound(ApiResponse<StaffSalaryDto>.ErrorResponse($"No staff salary found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<StaffSalaryDto>.SuccessResponse(salary, "Staff salary retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutStaffSalary(int id, StaffSalaryDto dto)
        {
            if (id != dto.StaffSalaryId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload StaffSalaryId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.StaffSalaryId}'." }, 400));
            }

            var success = await _staffSalaryService.UpdateStaffSalaryAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No staff salary found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Staff salary updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StaffSalaryDto>>> PostStaffSalary(StaffSalaryDto dto)
        {
            var created = await _staffSalaryService.CreateStaffSalaryAsync(dto);
            return CreatedAtAction(nameof(GetStaffSalary), new { id = created.StaffSalaryId }, ApiResponse<StaffSalaryDto>.SuccessResponse(created, "Staff salary created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStaffSalary(int id)
        {
            var success = await _staffSalaryService.DeleteStaffSalaryAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No staff salary found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Staff salary deleted successfully."));
        }
    }
}
