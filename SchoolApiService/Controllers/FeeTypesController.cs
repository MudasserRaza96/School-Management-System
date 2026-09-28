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
    public class FeeTypesController : ControllerBase
    {
        private readonly IFeeTypeService _feeTypeService;

        public FeeTypesController(IFeeTypeService feeTypeService)
        {
            _feeTypeService = feeTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeeTypeDto>>>> GetFeeTypes()
        {
            var types = await _feeTypeService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<FeeTypeDto>>.SuccessResponse(types, "Fee types retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FeeTypeDto>>> GetFeeType(int id)
        {
            var type = await _feeTypeService.GetByIdAsync(id);
            if (type == null)
            {
                return NotFound(ApiResponse<FeeTypeDto>.ErrorResponse($"No fee type found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<FeeTypeDto>.SuccessResponse(type, "Fee type retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutFeeType(int id, FeeTypeDto dto)
        {
            if (id != dto.FeeTypeId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload FeeTypeId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.FeeTypeId}'." }, 400));
            }

            var success = await _feeTypeService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No fee type found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Fee type updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<FeeTypeDto>>> PostFeeType(FeeTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FeeTypeName))
            {
                return BadRequest(ApiResponse<FeeTypeDto>.ErrorResponse("Fee type creation failed.", new List<string> { "FeeTypeName is required." }, 400));
            }

            var created = await _feeTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetFeeType), new { id = created.FeeTypeId }, ApiResponse<FeeTypeDto>.SuccessResponse(created, "Fee type created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFeeType(int id)
        {
            var success = await _feeTypeService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No fee type found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Fee type deleted successfully."));
        }
    }
}
