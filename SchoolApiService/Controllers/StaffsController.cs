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
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<StaffResponseDto>>>> GetStaffs()
        {
            var staffs = await _staffService.GetAllStaffAsync();
            return Ok(ApiResponse<IEnumerable<StaffResponseDto>>.SuccessResponse(staffs, "Staff members retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StaffResponseDto>>> GetStaff(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound(ApiResponse<StaffResponseDto>.ErrorResponse($"No staff member was found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<StaffResponseDto>.SuccessResponse(staff, "Staff member retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<StaffResponseDto>>> PutStaff(int id, StaffUpdateDto dto)
        {
            if (id != dto.StaffId)
            {
                return BadRequest(ApiResponse<StaffResponseDto>.ErrorResponse("Staff update failed.", new List<string> { $"Provided URL ID '{id}' does not match payload StaffId '{dto.StaffId}'." }, 400));
            }

            var (succeeded, errorMessage, updatedStaff) = await _staffService.UpdateStaffAsync(id, dto);
            if (!succeeded)
            {
                return BadRequest(ApiResponse<StaffResponseDto>.ErrorResponse("Staff update failed.", new List<string> { errorMessage ?? "Failed to update staff." }, 400));
            }

            return Ok(ApiResponse<StaffResponseDto>.SuccessResponse(updatedStaff!, "Staff member updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StaffResponseDto>>> PostStaff(StaffCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StaffName))
            {
                return BadRequest(ApiResponse<StaffResponseDto>.ErrorResponse("Staff creation failed.", new List<string> { "Staff name is required." }, 400));
            }

            var (succeeded, errorMessage, createdStaff) = await _staffService.CreateStaffAsync(dto);
            if (!succeeded || createdStaff == null)
            {
                return BadRequest(ApiResponse<StaffResponseDto>.ErrorResponse("Staff creation failed.", new List<string> { errorMessage ?? "Failed to create staff." }, 400));
            }

            return CreatedAtAction(nameof(GetStaff), new { id = createdStaff.StaffId }, ApiResponse<StaffResponseDto>.SuccessResponse(createdStaff, "Staff member created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStaff(int id)
        {
            var success = await _staffService.DeleteStaffAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No staff member was found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Staff member deleted successfully."));
        }
    }
}
