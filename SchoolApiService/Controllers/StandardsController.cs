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
    public class StandardsController : ControllerBase
    {
        private readonly IStandardService _standardService;

        public StandardsController(IStandardService standardService)
        {
            _standardService = standardService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<StandardDto>>>> GetStandards()
        {
            var stds = await _standardService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<StandardDto>>.SuccessResponse(stds, "Standards retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StandardDto>>> GetStandard(int id)
        {
            var std = await _standardService.GetByIdAsync(id);
            if (std == null)
            {
                return NotFound(ApiResponse<StandardDto>.ErrorResponse($"No standard found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<StandardDto>.SuccessResponse(std, "Standard retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutStandard(int id, StandardDto dto)
        {
            if (id != dto.StandardId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload StandardId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.StandardId}'." }, 400));
            }

            var success = await _standardService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No standard found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Standard updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StandardDto>>> PostStandard(StandardDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StandardName))
            {
                return BadRequest(ApiResponse<StandardDto>.ErrorResponse("Standard creation failed.", new List<string> { "StandardName is required." }, 400));
            }

            var created = await _standardService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetStandard), new { id = created.StandardId }, ApiResponse<StandardDto>.SuccessResponse(created, "Standard created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStandard(int id)
        {
            var success = await _standardService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No standard found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Standard deleted successfully."));
        }
    }
}
