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
    public class MarkEntriesController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public MarkEntriesController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<MarkEntryDto>>>> GetMarkEntries()
        {
            var entries = await _context.dbsMarkEntry
                .Include(m => m.Staff)
                .Include(m => m.Subject)
                .Include(m => m.Marks)
                .ToListAsync();

            var dtos = entries.Select(e => new MarkEntryDto
            {
                MarkEntryId = e.MarkEntryId,
                StudentId = e.StudentId ?? 0,
                StudentName = e.Student?.StudentName,
                ExamScheduleId = e.ExamScheduleId ?? 0,
                TotalObtainedMarks = e.TotalObtainedMarks ?? 0,
                FinalGrade = e.FinalGrade
            });

            return Ok(ApiResponse<IEnumerable<MarkEntryDto>>.SuccessResponse(dtos, "Mark entries retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MarkEntryDto>>> GetMarkEntry(int id)
        {
            var e = await _context.dbsMarkEntry
                .Include(m => m.Staff)
                .Include(m => m.Subject)
                .Include(m => m.Marks)
                .FirstOrDefaultAsync(m => m.MarkEntryId == id);

            if (e == null)
            {
                return NotFound(ApiResponse<MarkEntryDto>.ErrorResponse($"No mark entry found with ID {id}.", statusCode: 404));
            }

            var dto = new MarkEntryDto
            {
                MarkEntryId = e.MarkEntryId,
                StudentId = e.StudentId ?? 0,
                StudentName = e.Student?.StudentName,
                ExamScheduleId = e.ExamScheduleId ?? 0,
                TotalObtainedMarks = e.TotalObtainedMarks ?? 0,
                FinalGrade = e.FinalGrade
            };

            return Ok(ApiResponse<MarkEntryDto>.SuccessResponse(dto, "Mark entry retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MarkEntryDto>>> PostMarkEntry(MarkEntryDto dto)
        {
            var entity = new MarkEntry
            {
                StudentId = dto.StudentId,
                ExamScheduleId = dto.ExamScheduleId,
                TotalObtainedMarks = dto.TotalObtainedMarks,
                FinalGrade = dto.FinalGrade
            };

            _context.dbsMarkEntry.Add(entity);
            await _context.SaveChangesAsync();
            dto.MarkEntryId = entity.MarkEntryId;

            return CreatedAtAction(nameof(GetMarkEntry), new { id = dto.MarkEntryId }, ApiResponse<MarkEntryDto>.SuccessResponse(dto, "Mark entry created successfully.", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutMarkEntry(int id, MarkEntryDto dto)
        {
            if (id != dto.MarkEntryId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload MarkEntryId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.MarkEntryId}'." }, 400));
            }

            var entity = await _context.dbsMarkEntry.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No mark entry found with ID {id} to update.", statusCode: 404));
            }

            entity.StudentId = dto.StudentId;
            entity.ExamScheduleId = dto.ExamScheduleId;
            entity.TotalObtainedMarks = dto.TotalObtainedMarks;
            entity.FinalGrade = dto.FinalGrade;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Mark entry updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMarkEntry(int id)
        {
            var entity = await _context.dbsMarkEntry.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No mark entry found with ID {id} to delete.", statusCode: 404));
            }

            _context.dbsMarkEntry.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Mark entry deleted successfully."));
        }
    }
}
