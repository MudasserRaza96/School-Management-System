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
    public class FeesController : ControllerBase
    {
        private readonly IFeeService _feeService;

        public FeesController(IFeeService feeService)
        {
            _feeService = feeService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeeDto>>>> GetFees()
        {
            var fees = await _feeService.GetAllFeesAsync();
            return Ok(ApiResponse<IEnumerable<FeeDto>>.SuccessResponse(fees, "Fees retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FeeDto>>> GetFee(int id)
        {
            var fee = await _feeService.GetFeeByIdAsync(id);
            if (fee == null)
            {
                return NotFound(ApiResponse<FeeDto>.ErrorResponse($"No fee record found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<FeeDto>.SuccessResponse(fee, "Fee record retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutFee(int id, FeeDto dto)
        {
            if (id != dto.FeeId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload FeeId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.FeeId}'." }, 400));
            }

            var success = await _feeService.UpdateFeeAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No fee record found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Fee record updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<FeeDto>>> PostFee(FeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FeeName))
            {
                return BadRequest(ApiResponse<FeeDto>.ErrorResponse("Fee creation failed.", new List<string> { "FeeName is required." }, 400));
            }

            var created = await _feeService.CreateFeeAsync(dto);
            return CreatedAtAction(nameof(GetFee), new { id = created.FeeId }, ApiResponse<FeeDto>.SuccessResponse(created, "Fee record created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFee(int id)
        {
            var success = await _feeService.DeleteFeeAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No fee record found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Fee record deleted successfully."));
        }
    }
}
