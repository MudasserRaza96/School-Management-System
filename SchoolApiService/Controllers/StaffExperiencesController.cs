using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffExperiencesController : ControllerBase
    {
        private readonly IStaffExperienceService _staffExperienceService;

        public StaffExperiencesController(IStaffExperienceService staffExperienceService)
        {
            _staffExperienceService = staffExperienceService;
        }

        // GET: api/StaffExperiences
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffExperience>>> GetdbsStaffExperience()
        {
            var experiences = await _staffExperienceService.GetAllStaffExperiencesAsync();
            return Ok(experiences);
        }

        // GET: api/StaffExperiences/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffExperience>> GetStaffExperience(int id)
        {
            var staffExperience = await _staffExperienceService.GetStaffExperienceByIdAsync(id);

            if (staffExperience == null)
            {
                return NotFound();
            }

            return staffExperience;
        }

        // PUT: api/StaffExperiences/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaffExperience(int id, StaffExperience staffExperience)
        {
            var (succeeded, concurrencyError) = await _staffExperienceService.UpdateStaffExperienceAsync(id, staffExperience);

            if (!succeeded)
            {
                if (concurrencyError)
                {
                    return NotFound();
                }
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/StaffExperiences
        [HttpPost]
        public async Task<ActionResult<StaffExperience>> PostStaffExperience(StaffExperience staffExperience)
        {
            var createdExperience = await _staffExperienceService.CreateStaffExperienceAsync(staffExperience);
            return CreatedAtAction("GetStaffExperience", new { id = createdExperience.StaffExperienceId }, createdExperience);
        }

        // DELETE: api/StaffExperiences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaffExperience(int id)
        {
            var deleted = await _staffExperienceService.DeleteStaffExperienceAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
