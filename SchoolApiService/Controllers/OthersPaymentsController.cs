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
    public class OthersPaymentsController : ControllerBase
    {
        private readonly IOthersPaymentService _othersPaymentService;

        public OthersPaymentsController(IOthersPaymentService othersPaymentService)
        {
            _othersPaymentService = othersPaymentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OthersPaymentDto>>>> GetOthersPayments()
        {
            var list = await _othersPaymentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<OthersPaymentDto>>.SuccessResponse(list, "Other payments retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OthersPaymentDto>>> GetOthersPayment(int id)
        {
            var p = await _othersPaymentService.GetByIdAsync(id);
            if (p == null)
            {
                return NotFound(ApiResponse<OthersPaymentDto>.ErrorResponse($"No other payment record found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<OthersPaymentDto>.SuccessResponse(p, "Other payment record retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<OthersPaymentDto>>> PutOthersPayment(int id, OthersPaymentDto dto)
        {
            if (id != dto.OthersPaymentId)
            {
                return BadRequest(ApiResponse<OthersPaymentDto>.ErrorResponse("URL ID and payload OthersPaymentId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.OthersPaymentId}'." }, 400));
            }

            var updated = await _othersPaymentService.UpdateAsync(id, dto);
            if (updated == null)
            {
                return NotFound(ApiResponse<OthersPaymentDto>.ErrorResponse($"No other payment record found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<OthersPaymentDto>.SuccessResponse(updated, "Other payment record updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OthersPaymentDto>>> PostOthersPayment(OthersPaymentDto dto)
        {
            var created = await _othersPaymentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetOthersPayment), new { id = created.OthersPaymentId }, ApiResponse<OthersPaymentDto>.SuccessResponse(created, "Other payment record created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteOthersPayment(int id)
        {
            var success = await _othersPaymentService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No other payment record found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Other payment record deleted successfully."));
        }
    }
}
