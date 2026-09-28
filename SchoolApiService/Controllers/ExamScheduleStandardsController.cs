using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.DTOs;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamScheduleStandardsController : ControllerBase
    {
        private readonly IExamScheduleStandardService _examScheduleStandardService;

        public ExamScheduleStandardsController(IExamScheduleStandardService examScheduleStandardService)
        {
            _examScheduleStandardService = examScheduleStandardService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamScheduleStandardVM>>>> GetExamScheduleStandards()
        {
            var data = await _examScheduleStandardService.GetAllExamScheduleStandardsAsync();
            return Ok(ApiResponse<IEnumerable<ExamScheduleStandardVM>>.SuccessResponse(data, "Exam schedule standards retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ExamScheduleStandardVM>>> GetExamScheduleStandard(int id)
        {
            var data = await _examScheduleStandardService.GetExamScheduleStandardByIdAsync(id);
            if (data == null)
            {
                return NotFound(ApiResponse<ExamScheduleStandardVM>.ErrorResponse($"No exam schedule standard found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<ExamScheduleStandardVM>.SuccessResponse(data, "Exam schedule standard retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutExamScheduleStandard(int id, UpdateExamScheduleStandardVM request)
        {
            var (succeeded, errorMessage) = await _examScheduleStandardService.UpdateExamScheduleStandardAsync(id, request);
            if (!succeeded)
            {
                if (errorMessage == "Exam schedule standard Id not found.")
                {
                    return NotFound(ApiResponse<object>.ErrorResponse($"No exam schedule standard found with ID {id} to update.", statusCode: 404));
                }
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage ?? "Update failed.", statusCode: 400));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam schedule standard updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> PostExamScheduleStandard(CreateExamScheduleStandardVM request)
        {
            var (succeeded, errorMessage) = await _examScheduleStandardService.CreateExamScheduleStandardAsync(request);
            if (!succeeded)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage ?? "Creation failed.", statusCode: 400));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam schedule standard created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteExamScheduleStandard(int id)
        {
            var succeeded = await _examScheduleStandardService.DeleteExamScheduleStandardAsync(id);
            if (!succeeded)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No exam schedule standard found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam schedule standard deleted successfully."));
        }
    }
}
