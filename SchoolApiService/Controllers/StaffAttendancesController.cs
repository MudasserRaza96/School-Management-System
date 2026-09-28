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
    public class StaffAttendancesController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public StaffAttendancesController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<StaffAttendanceDto>>>> GetStaffAttendances()
        {
            var records = await _context.dbsStaffAttendance
                .Include(m => m.Staffs)
                .ToListAsync();

            var dtos = records.Select(r => new StaffAttendanceDto
            {
                StaffAttendanceId = r.StaffAttendanceId,
                Date = r.WorkingDate,
                Status = r.IsPresent ? "Present" : "Absent"
            });

            return Ok(ApiResponse<IEnumerable<StaffAttendanceDto>>.SuccessResponse(dtos, "Staff attendances retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StaffAttendanceDto>>> GetStaffAttendance(int id)
        {
            var record = await _context.dbsStaffAttendance
                .Include(m => m.Staffs)
                .FirstOrDefaultAsync(m => m.StaffAttendanceId == id);

            if (record == null)
            {
                return NotFound(ApiResponse<StaffAttendanceDto>.ErrorResponse($"No staff attendance record found with ID {id}.", statusCode: 404));
            }

            var dto = new StaffAttendanceDto
            {
                StaffAttendanceId = record.StaffAttendanceId,
                Date = record.WorkingDate,
                Status = record.IsPresent ? "Present" : "Absent"
            };

            return Ok(ApiResponse<StaffAttendanceDto>.SuccessResponse(dto, "Staff attendance record retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StaffAttendanceDto>>> PostStaffAttendance(StaffAttendanceDto dto)
        {
            var entity = new StaffAttendance
            {
                WorkingDate = dto.Date,
                IsPresent = dto.Status.Equals("Present", StringComparison.OrdinalIgnoreCase)
            };

            _context.dbsStaffAttendance.Add(entity);
            await _context.SaveChangesAsync();
            dto.StaffAttendanceId = entity.StaffAttendanceId;

            return CreatedAtAction(nameof(GetStaffAttendance), new { id = dto.StaffAttendanceId }, ApiResponse<StaffAttendanceDto>.SuccessResponse(dto, "Staff attendance created successfully.", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutStaffAttendance(int id, StaffAttendanceDto dto)
        {
            if (id != dto.StaffAttendanceId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload StaffAttendanceId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.StaffAttendanceId}'." }, 400));
            }

            var record = await _context.dbsStaffAttendance.FindAsync(id);
            if (record == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No staff attendance record found with ID {id} to update.", statusCode: 404));
            }

            record.WorkingDate = dto.Date;
            record.IsPresent = dto.Status.Equals("Present", StringComparison.OrdinalIgnoreCase);

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Staff attendance updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStaffAttendance(int id)
        {
            var record = await _context.dbsStaffAttendance.FindAsync(id);
            if (record == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No staff attendance record found with ID {id} to delete.", statusCode: 404));
            }

            _context.dbsStaffAttendance.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Staff attendance deleted successfully."));
        }
    }
}
