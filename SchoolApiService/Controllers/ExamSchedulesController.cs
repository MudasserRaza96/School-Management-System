using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamSchedulesController : ControllerBase
    {
        private readonly IExamScheduleService _examScheduleService;

        public ExamSchedulesController(IExamScheduleService examScheduleService)
        {
            _examScheduleService = examScheduleService;
        }

        // GET: api/ExamSchedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamScheduleVM>>> GetdbsExamSchedule()
        {
            var examSchedules = await _examScheduleService.GetAllExamSchedulesAsync();
            return Ok(examSchedules);
        }

        public record GetExamScheduleOptionsResponse(int ExamScheduleId, string ExamScheduleName);

        [HttpGet("GetExamScheduleOptions")]
        public async Task<IEnumerable<GetExamScheduleOptionsResponse>> GetExamScheduleOptions()
        {
            return await _examScheduleService.GetExamScheduleOptionsAsync();
        }

        // GET: api/ExamSchedules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamScheduleVM>> GetExamSchedule(int id)
        {
            var examSchedule = await _examScheduleService.GetExamScheduleByIdAsync(id);

            if (examSchedule == null)
            {
                return NotFound();
            }

            return examSchedule;
        }

        // PUT: api/ExamSchedules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamSchedule(int id, ExamSchedule examSchedule)
        {
            var (succeeded, concurrencyError) = await _examScheduleService.UpdateExamScheduleAsync(id, examSchedule);

            if (!succeeded)
            {
                if (concurrencyError)
                {
                    return NotFound();
                }
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/ExamSchedules
        [HttpPost]
        public async Task<ActionResult<ExamSchedule>> PostExamSchedule(ExamSchedule examSchedule)
        {
            var createdSchedule = await _examScheduleService.CreateExamScheduleAsync(examSchedule);
            return CreatedAtAction("GetExamSchedule", new { id = createdSchedule.ExamScheduleId }, createdSchedule);
        }

        // DELETE: api/ExamSchedules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamSchedule(int id)
        {
            var deleted = await _examScheduleService.DeleteExamScheduleAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
