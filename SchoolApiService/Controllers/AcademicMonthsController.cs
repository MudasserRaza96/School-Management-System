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
    public class AcademicMonthsController : ControllerBase
    {
        private readonly IAcademicMonthService _academicMonthService;

        public AcademicMonthsController(IAcademicMonthService academicMonthService)
        {
            _academicMonthService = academicMonthService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AcademicMonthDto>>>> GetAcademicMonths()
        {
            var months = await _academicMonthService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<AcademicMonthDto>>.SuccessResponse(months, "Academic months retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AcademicMonthDto>>> GetAcademicMonth(int id)
        {
            var academicMonth = await _academicMonthService.GetByIdAsync(id);
            if (academicMonth == null)
            {
                return NotFound(ApiResponse<AcademicMonthDto>.ErrorResponse($"No academic month found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<AcademicMonthDto>.SuccessResponse(academicMonth, "Academic month retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutAcademicMonth(int id, AcademicMonthDto dto)
        {
            if (id != dto.MonthId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload MonthId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.MonthId}'." }, 400));
            }

            var success = await _academicMonthService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No academic month found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Academic month updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AcademicMonthDto>>> PostAcademicMonth(AcademicMonthDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MonthName))
            {
                return BadRequest(ApiResponse<AcademicMonthDto>.ErrorResponse("Academic month creation failed.", new List<string> { "MonthName is required." }, 400));
            }

            var created = await _academicMonthService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAcademicMonth), new { id = created.MonthId }, ApiResponse<AcademicMonthDto>.SuccessResponse(created, "Academic month created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAcademicMonth(int id)
        {
            var success = await _academicMonthService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No academic month found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Academic month deleted successfully."));
        }
    }
}
