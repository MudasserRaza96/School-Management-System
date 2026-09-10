using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);

            if (student == null)
            {
                return NotFound("Sorry! No Student is found. Try next time. Good luck.");
            }

            return student;
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            var (succeeded, errorMessage, concurrencyError) = await _studentService.UpdateStudentAsync(id, student);

            if (!succeeded)
            {
                if (concurrencyError)
                {
                    return NotFound();
                }
                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            var (succeeded, errorMessage, createdStudent) = await _studentService.CreateStudentAsync(student);

            if (!succeeded || createdStudent == null)
            {
                return BadRequest(errorMessage);
            }

            return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.StudentId }, createdStudent);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
