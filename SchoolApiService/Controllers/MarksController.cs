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
    public class MarksController : ControllerBase
    {
        private readonly IMarkService _markService;

        public MarksController(IMarkService markService)
        {
            _markService = markService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<MarkDto>>>> GetMarks()
        {
            var marks = await _markService.GetAllMarksAsync();
            return Ok(ApiResponse<IEnumerable<MarkDto>>.SuccessResponse(marks, "Marks retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MarkDto>>> GetMark(int id)
        {
            var mark = await _markService.GetMarkByIdAsync(id);
            if (mark == null)
            {
                return NotFound(ApiResponse<MarkDto>.ErrorResponse($"No mark record found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<MarkDto>.SuccessResponse(mark, "Mark record retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutMark(int id, MarkDto dto)
        {
            if (id != dto.MarkId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload MarkId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.MarkId}'." }, 400));
            }

            var (succeeded, errorMessage, concurrencyError) = await _markService.UpdateMarkAsync(id, dto);
            if (!succeeded)
            {
                if (concurrencyError)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse($"No mark record found with ID {id} to update.", statusCode: 404));
                }
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage ?? "Update failed.", statusCode: 400));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Mark record updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MarkDto>>> PostMark(MarkDto dto)
        {
            var (succeeded, errorMessage, createdMark) = await _markService.CreateMarkAsync(dto);
            if (!succeeded || createdMark == null)
            {
                return BadRequest(ApiResponse<MarkDto>.ErrorResponse("Mark creation failed.", new List<string> { errorMessage ?? "Failed to create mark record." }, 400));
            }

            return CreatedAtAction(nameof(GetMark), new { id = createdMark.MarkId }, ApiResponse<MarkDto>.SuccessResponse(createdMark, "Mark record created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMark(int id)
        {
            var deleted = await _markService.DeleteMarkAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No mark record found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Mark record deleted successfully."));
        }
    }
}
