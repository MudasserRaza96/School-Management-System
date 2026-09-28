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
    public class DueBalancesController : ControllerBase
    {
        private readonly IDueBalanceService _dueBalanceService;

        public DueBalancesController(IDueBalanceService dueBalanceService)
        {
            _dueBalanceService = dueBalanceService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<DueBalanceDto>>>> GetDueBalances()
        {
            var list = await _dueBalanceService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<DueBalanceDto>>.SuccessResponse(list, "Due balances retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DueBalanceDto>>> GetDueBalance(int id)
        {
            var d = await _dueBalanceService.GetByIdAsync(id);
            if (d == null)
            {
                return NotFound(ApiResponse<DueBalanceDto>.ErrorResponse($"No due balance found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<DueBalanceDto>.SuccessResponse(d, "Due balance retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutDueBalance(int id, DueBalanceDto dto)
        {
            if (id != dto.DueBalanceId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload DueBalanceId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.DueBalanceId}'." }, 400));
            }

            var success = await _dueBalanceService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No due balance found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Due balance updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<DueBalanceDto>>> PostDueBalance(DueBalanceDto dto)
        {
            var created = await _dueBalanceService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDueBalance), new { id = created.DueBalanceId }, ApiResponse<DueBalanceDto>.SuccessResponse(created, "Due balance created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteDueBalance(int id)
        {
            var success = await _dueBalanceService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No due balance found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Due balance deleted successfully."));
        }
    }
}
