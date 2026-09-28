using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.DTOs;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendancesController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AttendanceDto>>>> GetAttendances()
        {
            var attendances = await _attendanceService.GetAllAttendancesAsync();
            return Ok(ApiResponse<IEnumerable<AttendanceDto>>.SuccessResponse(attendances, "Attendances retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AttendanceDto>>> GetAttendance(int id)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);
            if (attendance == null)
            {
                return NotFound(ApiResponse<AttendanceDto>.ErrorResponse($"No attendance record found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<AttendanceDto>.SuccessResponse(attendance, "Attendance record retrieved successfully."));
        }

        [HttpGet("Type/{type}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AttList>>>> GetAttendanceListByType(AttendanceType type)
        {
            var list = await _attendanceService.GetAttendanceListByTypeAsync(type);
            return Ok(ApiResponse<IEnumerable<AttList>>.SuccessResponse(list, "Attendance list retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutAttendance(int id, AttendanceDto dto)
        {
            if (id != dto.AttendanceId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload AttendanceId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.AttendanceId}'." }, 400));
            }

            var (succeeded, errorMessage, _) = await _attendanceService.UpdateAttendanceAsync(id, dto);
            if (!succeeded)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Attendance update failed.", new List<string> { errorMessage ?? "Failed to update attendance." }, 400));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Attendance record updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AttendanceDto>>> PostAttendance(AttendanceDto dto)
        {
            var (succeeded, errorMessage, createdAttendance) = await _attendanceService.CreateAttendanceAsync(dto);
            if (!succeeded || createdAttendance == null)
            {
                return BadRequest(ApiResponse<AttendanceDto>.ErrorResponse("Attendance creation failed.", new List<string> { errorMessage ?? "Failed to create attendance." }, 400));
            }

            return CreatedAtAction(nameof(GetAttendance), new { id = createdAttendance.AttendanceId }, ApiResponse<AttendanceDto>.SuccessResponse(createdAttendance, "Attendance record created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAttendance(int id)
        {
            var success = await _attendanceService.DeleteAttendanceAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No attendance record found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Attendance record deleted successfully."));
        }
    }
}
