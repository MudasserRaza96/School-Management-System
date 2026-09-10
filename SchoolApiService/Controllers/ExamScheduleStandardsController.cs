using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamScheduleStandardsController : ControllerBase
    {
        private readonly IExamScheduleStandardService _examScheduleStandardService;

        public ExamScheduleStandardsController(IExamScheduleStandardService examScheduleStandardService)
        {
            _examScheduleStandardService = examScheduleStandardService;
        }

        // GET: api/ExamScheduleStandards
        [HttpGet]
        public async Task<IEnumerable<ExamScheduleStandardVM>> GetdbsExamScheduleStandard()
        {
            return await _examScheduleStandardService.GetAllExamScheduleStandardsAsync();
        }

        // GET: api/ExamScheduleStandards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamScheduleStandardVM>> GetExamScheduleStandard(int id)
        {
            var examScheduleStandard = await _examScheduleStandardService.GetExamScheduleStandardByIdAsync(id);

            if (examScheduleStandard == null)
            {
                return NotFound();
            }

            return examScheduleStandard;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamScheduleStandard(int id, UpdateExamScheduleStandardVM request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errorMessage) = await _examScheduleStandardService.UpdateExamScheduleStandardAsync(id, request);

            if (!succeeded)
            {
                if (errorMessage == "Exam schedule standard Id not found.")
                {
                    return NotFound(errorMessage);
                }
                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        // POST: api/ExamScheduleStandards
        [HttpPost]
        public async Task<IActionResult> PostExamScheduleStandard(CreateExamScheduleStandardVM request)
        {
            var (succeeded, errorMessage) = await _examScheduleStandardService.CreateExamScheduleStandardAsync(request);

            if (!succeeded)
            {
                throw new Exception(errorMessage ?? "Error creating exam schedule standard.");
            }

            return Ok();
        }

        // DELETE: api/ExamScheduleStandards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamScheduleStandard(int id)
        {
            var deleted = await _examScheduleStandardService.DeleteExamScheduleStandardAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
