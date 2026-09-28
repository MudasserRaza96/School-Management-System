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
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<StudentResponseDto>>>> GetStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(ApiResponse<IEnumerable<StudentResponseDto>>.SuccessResponse(students, "Students retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentResponseDto>>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound(ApiResponse<StudentResponseDto>.ErrorResponse($"No student was found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(student, "Student retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<StudentResponseDto>>> PutStudent(int id, StudentUpdateDto dto)
        {
            if (id != dto.StudentId)
            {
                return BadRequest(ApiResponse<StudentResponseDto>.ErrorResponse("Student update failed.", new List<string> { $"Provided URL ID '{id}' does not match payload StudentId '{dto.StudentId}'." }, 400));
            }

            var (succeeded, errorMessage, updatedStudent) = await _studentService.UpdateStudentAsync(id, dto);
            if (!succeeded)
            {
                return BadRequest(ApiResponse<StudentResponseDto>.ErrorResponse("Student update failed.", new List<string> { errorMessage ?? "Failed to update student." }, 400));
            }

            return Ok(ApiResponse<StudentResponseDto>.SuccessResponse(updatedStudent!, "Student updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudentResponseDto>>> PostStudent(StudentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StudentName))
            {
                return BadRequest(ApiResponse<StudentResponseDto>.ErrorResponse("Student creation failed.", new List<string> { "Student name is required." }, 400));
            }

            var (succeeded, errorMessage, createdStudent) = await _studentService.CreateStudentAsync(dto);
            if (!succeeded || createdStudent == null)
            {
                return BadRequest(ApiResponse<StudentResponseDto>.ErrorResponse("Student creation failed.", new List<string> { errorMessage ?? "Failed to create student." }, 400));
            }

            return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.StudentId }, ApiResponse<StudentResponseDto>.SuccessResponse(createdStudent, "Student created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStudent(int id)
        {
            var success = await _studentService.DeleteStudentAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No student was found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Student deleted successfully."));
        }
    }
}
