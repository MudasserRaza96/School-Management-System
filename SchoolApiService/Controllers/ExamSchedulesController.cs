using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.DTOs;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamSchedulesController : ControllerBase
    {
        private readonly IExamScheduleService _examScheduleService;

        public ExamSchedulesController(IExamScheduleService examScheduleService)
        {
            _examScheduleService = examScheduleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamScheduleVM>>>> GetExamSchedules()
        {
            var examSchedules = await _examScheduleService.GetAllExamSchedulesAsync();
            return Ok(ApiResponse<IEnumerable<ExamScheduleVM>>.SuccessResponse(examSchedules, "Exam schedules retrieved successfully."));
        }

        public record GetExamScheduleOptionsResponse(int ExamScheduleId, string ExamScheduleName);

        [HttpGet("GetExamScheduleOptions")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GetExamScheduleOptionsResponse>>>> GetExamScheduleOptions()
        {
            var options = await _examScheduleService.GetExamScheduleOptionsAsync();
            return Ok(ApiResponse<IEnumerable<GetExamScheduleOptionsResponse>>.SuccessResponse(options, "Exam schedule options retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ExamScheduleVM>>> GetExamSchedule(int id)
        {
            var examSchedule = await _examScheduleService.GetExamScheduleByIdAsync(id);
            if (examSchedule == null)
            {
                return NotFound(ApiResponse<ExamScheduleVM>.ErrorResponse($"No exam schedule found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<ExamScheduleVM>.SuccessResponse(examSchedule, "Exam schedule retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutExamSchedule(int id, ExamSchedule examSchedule)
        {
            var (succeeded, concurrencyError) = await _examScheduleService.UpdateExamScheduleAsync(id, examSchedule);
            if (!succeeded)
            {
                if (concurrencyError)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse($"No exam schedule found with ID {id} to update.", statusCode: 404));
                }
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload ExamScheduleId mismatch.", statusCode: 400));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam schedule updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExamSchedule>>> PostExamSchedule(ExamSchedule examSchedule)
        {
            var createdSchedule = await _examScheduleService.CreateExamScheduleAsync(examSchedule);
            return CreatedAtAction(nameof(GetExamSchedule), new { id = createdSchedule.ExamScheduleId }, ApiResponse<ExamSchedule>.SuccessResponse(createdSchedule, "Exam schedule created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteExamSchedule(int id)
        {
            var deleted = await _examScheduleService.DeleteExamScheduleAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No exam schedule found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam schedule deleted successfully."));
        }
    }
}
