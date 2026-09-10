using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendancesController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // GET: api/Attendances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetdbsAttendance()
        {
            var attendances = await _attendanceService.GetAllAttendancesAsync();
            return Ok(attendances);
        }

        // GET: api/Attendances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Attendance>> GetAttendance(int id)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            return attendance;
        }

        [HttpGet("GetList/{Type}")]
        public async Task<ActionResult> GetList(AttendanceType Type)
        {
            var data = await _attendanceService.GetAttendanceListByTypeAsync(Type);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> PostAttendance([FromBody] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errorMessage, createdAttendance) = await _attendanceService.CreateAttendanceAsync(attendance);

            if (!succeeded || createdAttendance == null)
            {
                return BadRequest(errorMessage);
            }

            return CreatedAtAction("GetAttendance", new { id = createdAttendance.AttendanceId }, createdAttendance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errorMessage, concurrencyError) = await _attendanceService.UpdateAttendanceAsync(id, attendance);

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

        // DELETE: api/Attendances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var deleted = await _attendanceService.DeleteAttendanceAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
