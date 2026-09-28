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
    public class StaffExperiencesController : ControllerBase
    {
        private readonly IStaffExperienceService _staffExperienceService;

        public StaffExperiencesController(IStaffExperienceService staffExperienceService)
        {
            _staffExperienceService = staffExperienceService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<StaffExperienceDto>>>> GetStaffExperiences()
        {
            var exps = await _staffExperienceService.GetAllStaffExperiencesAsync();
            return Ok(ApiResponse<IEnumerable<StaffExperienceDto>>.SuccessResponse(exps, "Staff experiences retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StaffExperienceDto>>> GetStaffExperience(int id)
        {
            var exp = await _staffExperienceService.GetStaffExperienceByIdAsync(id);
            if (exp == null)
            {
                return NotFound(ApiResponse<StaffExperienceDto>.ErrorResponse($"No staff experience found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<StaffExperienceDto>.SuccessResponse(exp, "Staff experience retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutStaffExperience(int id, StaffExperienceDto dto)
        {
            if (id != dto.StaffExperienceId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload StaffExperienceId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.StaffExperienceId}'." }, 400));
            }

            var success = await _staffExperienceService.UpdateStaffExperienceAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No staff experience found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Staff experience updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StaffExperienceDto>>> PostStaffExperience(StaffExperienceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CompanyName))
            {
                return BadRequest(ApiResponse<StaffExperienceDto>.ErrorResponse("Staff experience creation failed.", new List<string> { "CompanyName is required." }, 400));
            }

            var created = await _staffExperienceService.CreateStaffExperienceAsync(dto);
            return CreatedAtAction(nameof(GetStaffExperience), new { id = created.StaffExperienceId }, ApiResponse<StaffExperienceDto>.SuccessResponse(created, "Staff experience created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStaffExperience(int id)
        {
            var success = await _staffExperienceService.DeleteStaffExperienceAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No staff experience found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Staff experience deleted successfully."));
        }
    }
}
