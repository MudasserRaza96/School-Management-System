using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamTypesController : ControllerBase
    {
        private readonly IExamTypeService _examTypeService;

        public ExamTypesController(IExamTypeService examTypeService)
        {
            _examTypeService = examTypeService;
        }

        // GET: api/ExamTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamType>>> GetdbsExamType()
        {
            var examTypes = await _examTypeService.GetAllExamTypesAsync();
            return Ok(examTypes);
        }

        // GET: api/ExamTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamType>> GetExamType(int id)
        {
            var examType = await _examTypeService.GetExamTypeByIdAsync(id);

            if (examType == null)
            {
                return NotFound();
            }

            return examType;
        }

        // PUT: api/ExamTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamType(int id, ExamType examType)
        {
            var (succeeded, concurrencyError) = await _examTypeService.UpdateExamTypeAsync(id, examType);

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

        // POST: api/ExamTypes
        [HttpPost]
        public async Task<ActionResult<ExamType>> PostExamType(ExamType examType)
        {
            var createdExamType = await _examTypeService.CreateExamTypeAsync(examType);
            return CreatedAtAction("GetExamType", new { id = createdExamType.ExamTypeId }, createdExamType);
        }

        // DELETE: api/ExamTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamType(int id)
        {
            var deleted = await _examTypeService.DeleteExamTypeAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
