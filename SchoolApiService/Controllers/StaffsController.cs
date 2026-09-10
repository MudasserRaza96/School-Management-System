using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetdbsStaff()
        {
            var staffList = await _staffService.GetAllStaffAsync();
            return Ok(staffList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetStaff(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);

            if (staff == null)
            {
                return NotFound("Sorry! No Staff is found. Try next time. Good luck.");
            }

            return staff;
        }

        [HttpPost]
        public async Task<ActionResult<Staff>> PostStaff(Staff staff)
        {
            var (succeeded, errorMessage, createdStaff) = await _staffService.CreateStaffAsync(staff);

            if (!succeeded || createdStaff == null)
            {
                return BadRequest(errorMessage);
            }

            return CreatedAtAction("GetdbsStaff", new { id = createdStaff.StaffId }, createdStaff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaff(int id, Staff staff)
        {
            var (succeeded, errorMessage, concurrencyError) = await _staffService.UpdateStaffAsync(id, staff);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var deleted = await _staffService.DeleteStaffAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
