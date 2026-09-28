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
    public class ExamSubjectsController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public ExamSubjectsController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ExamSubjectDto>>>> GetExamSubjects()
        {
            var subjects = await _context.dbsExamSubject
                .Include(es => es.Subject)
                .AsNoTracking()
                .ToListAsync();

            var dtos = subjects.Select(es => new ExamSubjectDto
            {
                ExamSubjectId = es.ExamSubjectId,
                ExamScheduleId = es.ExamScheduleStandardId ?? 0,
                SubjectId = es.SubjectId,
                SubjectName = es.Subject?.SubjectName,
                TotalMarks = es.TotalMarks,
                PassMarks = es.PassMarks
            });

            return Ok(ApiResponse<IEnumerable<ExamSubjectDto>>.SuccessResponse(dtos, "Exam subjects retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ExamSubjectDto>>> GetExamSubject(int id)
        {
            var es = await _context.dbsExamSubject
                .Include(es => es.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(es => es.ExamSubjectId == id);

            if (es == null)
            {
                return NotFound(ApiResponse<ExamSubjectDto>.ErrorResponse($"No exam subject found with ID {id}.", statusCode: 404));
            }

            var dto = new ExamSubjectDto
            {
                ExamSubjectId = es.ExamSubjectId,
                ExamScheduleId = es.ExamScheduleStandardId ?? 0,
                SubjectId = es.SubjectId,
                SubjectName = es.Subject?.SubjectName,
                TotalMarks = es.TotalMarks,
                PassMarks = es.PassMarks
            };

            return Ok(ApiResponse<ExamSubjectDto>.SuccessResponse(dto, "Exam subject retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ExamSubjectDto>>> PostExamSubject(ExamSubjectDto dto)
        {
            var entity = new ExamSubject
            {
                SubjectId = dto.SubjectId,
                TotalMarks = dto.TotalMarks,
                PassMarks = dto.PassMarks
            };

            _context.dbsExamSubject.Add(entity);
            await _context.SaveChangesAsync();
            dto.ExamSubjectId = entity.ExamSubjectId;

            return CreatedAtAction(nameof(GetExamSubject), new { id = dto.ExamSubjectId }, ApiResponse<ExamSubjectDto>.SuccessResponse(dto, "Exam subject created successfully.", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutExamSubject(int id, ExamSubjectDto dto)
        {
            if (id != dto.ExamSubjectId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload ExamSubjectId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.ExamSubjectId}'." }, 400));
            }

            var entity = await _context.dbsExamSubject.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No exam subject found with ID {id} to update.", statusCode: 404));
            }

            entity.SubjectId = dto.SubjectId;
            entity.TotalMarks = dto.TotalMarks;
            entity.PassMarks = dto.PassMarks;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam subject updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteExamSubject(int id)
        {
            var entity = await _context.dbsExamSubject.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No exam subject found with ID {id} to delete.", statusCode: 404));
            }

            _context.dbsExamSubject.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Exam subject deleted successfully."));
        }
    }
}
