using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffSalariesController : ControllerBase
    {
        private readonly IStaffSalaryService _staffSalaryService;

        public StaffSalariesController(IStaffSalaryService staffSalaryService)
        {
            _staffSalaryService = staffSalaryService;
        }

        // GET: api/StaffSalaries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffSalary>>> GetdbsStaffSalary()
        {
            var salaries = await _staffSalaryService.GetAllStaffSalariesAsync();
            return Ok(salaries);
        }

        // GET: api/StaffSalaries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffSalary>> GetStaffSalary(int id)
        {
            var staffSalary = await _staffSalaryService.GetStaffSalaryByIdAsync(id);

            if (staffSalary == null)
            {
                return NotFound();
            }

            return staffSalary;
        }

        // PUT: api/StaffSalaries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaffSalary(int id, StaffSalary staffSalary)
        {
            var (succeeded, concurrencyError) = await _staffSalaryService.UpdateStaffSalaryAsync(id, staffSalary);

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

        // POST: api/StaffSalaries
        [HttpPost]
        public async Task<ActionResult<StaffSalary>> PostStaffSalary(StaffSalary staffSalary)
        {
            var createdSalary = await _staffSalaryService.CreateStaffSalaryAsync(staffSalary);
            return CreatedAtAction("GetStaffSalary", new { id = createdSalary.StaffSalaryId }, createdSalary);
        }

        // DELETE: api/StaffSalaries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaffSalary(int id)
        {
            var deleted = await _staffSalaryService.DeleteStaffSalaryAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
