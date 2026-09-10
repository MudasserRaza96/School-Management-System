using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarksController : ControllerBase
    {
        private readonly IMarkService _markService;

        public MarksController(IMarkService markService)
        {
            _markService = markService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mark>>> GetdbsMark()
        {
            var marks = await _markService.GetAllMarksAsync();
            return Ok(marks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Mark>> GetMark(int id)
        {
            var mark = await _markService.GetMarkByIdAsync(id);

            if (mark == null)
            {
                return NotFound("Sorry! No Mark is found. Try next time. Good luck.");
            }

            return mark;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMark(int id, Mark mark)
        {
            var (succeeded, errorMessage, concurrencyError) = await _markService.UpdateMarkAsync(id, mark);

            if (!succeeded)
            {
                if (concurrencyError)
                {
                    return NotFound("Mark not found.");
                }
                return BadRequest(errorMessage);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Mark>> PostMark(Mark mark)
        {
            var (succeeded, errorMessage, result) = await _markService.CreateMarkAsync(mark);

            if (!succeeded || result == null)
            {
                return BadRequest(errorMessage);
            }

            return CreatedAtAction("GetdbsMark", new { id = mark.MarkId }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMark(int id)
        {
            var deleted = await _markService.DeleteMarkAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
