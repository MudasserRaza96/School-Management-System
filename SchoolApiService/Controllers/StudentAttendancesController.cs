using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Models;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAttendancesController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public StudentAttendancesController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<StudentAttendanceDto>>>> GetStudentAttendances()
        {
            var records = await _context.dbsStudentAttendance
                .Include(m => m.Students)
                .ToListAsync();

            var dtos = records.Select(r => new StudentAttendanceDto
            {
                StudentAttendanceId = r.StudentAttendanceId,
                AttendanceDate = r.WorkingDate,
                StudentId = r.Students?.FirstOrDefault()?.StudentId ?? 0,
                StudentName = r.Students?.FirstOrDefault()?.StudentName,
                Status = r.IsPresent ? "Present" : "Absent"
            });

            return Ok(ApiResponse<IEnumerable<StudentAttendanceDto>>.SuccessResponse(dtos, "Student attendances retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentAttendanceDto>>> GetStudentAttendance(int id)
        {
            var record = await _context.dbsStudentAttendance
                .Include(m => m.Students)
                .FirstOrDefaultAsync(m => m.StudentAttendanceId == id);

            if (record == null)
            {
                return NotFound(ApiResponse<StudentAttendanceDto>.ErrorResponse($"No student attendance record found with ID {id}.", statusCode: 404));
            }

            var dto = new StudentAttendanceDto
            {
                StudentAttendanceId = record.StudentAttendanceId,
                AttendanceDate = record.WorkingDate,
                StudentId = record.Students?.FirstOrDefault()?.StudentId ?? 0,
                StudentName = record.Students?.FirstOrDefault()?.StudentName,
                Status = record.IsPresent ? "Present" : "Absent"
            };

            return Ok(ApiResponse<StudentAttendanceDto>.SuccessResponse(dto, "Student attendance record retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StudentAttendanceDto>>> PostStudentAttendance(StudentAttendanceDto dto)
        {
            var entity = new StudentAttendance
            {
                WorkingDate = dto.AttendanceDate,
                IsPresent = dto.Status.Equals("Present", StringComparison.OrdinalIgnoreCase)
            };

            if (dto.StudentId > 0)
            {
                var student = await _context.dbsStudent.FindAsync(dto.StudentId);
                if (student != null)
                {
                    entity.Students = new List<Student> { student };
                }
            }

            _context.dbsStudentAttendance.Add(entity);
            await _context.SaveChangesAsync();
            dto.StudentAttendanceId = entity.StudentAttendanceId;

            return CreatedAtAction(nameof(GetStudentAttendance), new { id = dto.StudentAttendanceId }, ApiResponse<StudentAttendanceDto>.SuccessResponse(dto, "Student attendance record created successfully.", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutStudentAttendance(int id, StudentAttendanceDto dto)
        {
            if (id != dto.StudentAttendanceId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload StudentAttendanceId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.StudentAttendanceId}'." }, 400));
            }

            var record = await _context.dbsStudentAttendance.Include(a => a.Students).FirstOrDefaultAsync(e => e.StudentAttendanceId == id);
            if (record == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No student attendance record found with ID {id} to update.", statusCode: 404));
            }

            record.WorkingDate = dto.AttendanceDate;
            record.IsPresent = dto.Status.Equals("Present", StringComparison.OrdinalIgnoreCase);

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Student attendance record updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStudentAttendance(int id)
        {
            var record = await _context.dbsStudentAttendance.FindAsync(id);
            if (record == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No student attendance record found with ID {id} to delete.", statusCode: 404));
            }

            _context.dbsStudentAttendance.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Student attendance record deleted successfully."));
        }
    }
}
