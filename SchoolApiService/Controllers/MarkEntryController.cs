using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkEntryController : ControllerBase
    {
        private readonly IMarkEntryService _markEntryService;

        public MarkEntryController(IMarkEntryService markEntryService)
        {
            _markEntryService = markEntryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarkEntry>>> GetdbsMarkEntry()
        {
            var entries = await _markEntryService.GetAllMarkEntriesAsync();
            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MarkEntry>> GetMarkEntry(int id)
        {
            var markEntry = await _markEntryService.GetMarkEntryByIdAsync(id);

            if (markEntry == null)
            {
                return NotFound("Sorry! No Mark is found. Try next time. Good luck.");
            }

            return markEntry;
        }

        [HttpPost("GetStudents")]
        public async Task<IActionResult> GetStudents([FromBody] MarkEntry markEntry)
        {
            var details = await _markEntryService.PopulateStudentMarksDetailsAsync(markEntry);
            return Ok(details);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarks([FromBody] MarkEntry markEntry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errorMessage, statusCode, createdEntry) = await _markEntryService.CreateMarkEntryAsync(markEntry);

            if (!succeeded || createdEntry == null)
            {
                if (statusCode == 500)
                {
                    return StatusCode(500, errorMessage);
                }
                return BadRequest(errorMessage);
            }

            return CreatedAtAction("GetMarkEntry", new { id = createdEntry.MarkEntryId }, createdEntry);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMarks([FromBody] MarkEntry markEntry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errorMessage, updatedEntry) = await _markEntryService.UpdateMarkEntryAsync(markEntry);

            if (!succeeded || updatedEntry == null)
            {
                if (errorMessage == "Mark entry not found.")
                {
                    return NotFound(errorMessage);
                }
                return BadRequest(errorMessage);
            }

            return Ok(updatedEntry);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarkEntry(int id)
        {
            var deleted = await _markEntryService.DeleteMarkEntryAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
