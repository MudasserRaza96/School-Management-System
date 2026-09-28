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
    public class MonthlyPaymentsController : ControllerBase
    {
        private readonly IMonthlyPaymentService _monthlyPaymentService;

        public MonthlyPaymentsController(IMonthlyPaymentService monthlyPaymentService)
        {
            _monthlyPaymentService = monthlyPaymentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<MonthlyPaymentDto>>>> GetMonthlyPayments()
        {
            var list = await _monthlyPaymentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<MonthlyPaymentDto>>.SuccessResponse(list, "Monthly payments retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MonthlyPaymentDto>>> GetMonthlyPayment(int id)
        {
            var p = await _monthlyPaymentService.GetByIdAsync(id);
            if (p == null)
            {
                return NotFound(ApiResponse<MonthlyPaymentDto>.ErrorResponse($"No monthly payment found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<MonthlyPaymentDto>.SuccessResponse(p, "Monthly payment retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<MonthlyPaymentDto>>> PutMonthlyPayment(int id, MonthlyPaymentDto dto)
        {
            if (id != dto.MonthlyPaymentId)
            {
                return BadRequest(ApiResponse<MonthlyPaymentDto>.ErrorResponse("URL ID and payload MonthlyPaymentId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.MonthlyPaymentId}'." }, 400));
            }

            var updated = await _monthlyPaymentService.UpdateAsync(id, dto);
            if (updated == null)
            {
                return NotFound(ApiResponse<MonthlyPaymentDto>.ErrorResponse($"No monthly payment found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<MonthlyPaymentDto>.SuccessResponse(updated, "Monthly payment updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MonthlyPaymentDto>>> PostMonthlyPayment(MonthlyPaymentDto dto)
        {
            var created = await _monthlyPaymentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetMonthlyPayment), new { id = created.MonthlyPaymentId }, ApiResponse<MonthlyPaymentDto>.SuccessResponse(created, "Monthly payment created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMonthlyPayment(int id)
        {
            var success = await _monthlyPaymentService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No monthly payment found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Monthly payment deleted successfully."));
        }
    }
}
