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
    public class ExamTypesController : ControllerBase
    {
        private readonly IExamTypeService _examTypeService;

        public ExamTypesController(IExamTypeService examTypeService)
        {
            _examTypeService = examTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamTypeDto>>>> GetExamTypes()
        {
            var types = await _examTypeService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ExamTypeDto>>.SuccessResponse(types, "Exam types retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ExamTypeDto>>> GetExamType(int id)
        {
            var type = await _examTypeService.GetByIdAsync(id);
            if (type == null)
            {
                return NotFound(ApiResponse<ExamTypeDto>.ErrorResponse($"No exam type found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<ExamTypeDto>.SuccessResponse(type, "Exam type retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutExamType(int id, ExamTypeDto dto)
        {
            if (id != dto.ExamTypeId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload ExamTypeId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.ExamTypeId}'." }, 400));
            }

            var success = await _examTypeService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No exam type found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam type updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExamTypeDto>>> PostExamType(ExamTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ExamTypeName))
            {
                return BadRequest(ApiResponse<ExamTypeDto>.ErrorResponse("Exam type creation failed.", new List<string> { "ExamTypeName is required." }, 400));
            }

            var created = await _examTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetExamType), new { id = created.ExamTypeId }, ApiResponse<ExamTypeDto>.SuccessResponse(created, "Exam type created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteExamType(int id)
        {
            var success = await _examTypeService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No exam type found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam type deleted successfully."));
        }
    }
}
