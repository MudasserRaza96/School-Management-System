using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetdbsSubject()
        {
            var subjects = await _subjectService.GetAllAsync();
            return Ok(subjects);
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubject(int id)
        {
            var subject = await _subjectService.GetByIdAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            return subject;
        }

        // PUT: api/Subjects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(int id, Subject subject)
        {
            if (id != subject.SubjectId)
            {
                return BadRequest();
            }

            try
            {
                var result = await _subjectService.UpdateAsync(id, subject);
                if (!result)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                if (!await _subjectService.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/Subjects
        [HttpPost]
        public async Task<ActionResult<Subject>> PostSubject(Subject subject)
        {
            var created = await _subjectService.CreateAsync(subject);
            return CreatedAtAction("GetSubject", new { id = created.SubjectId }, created);
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var success = await _subjectService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
