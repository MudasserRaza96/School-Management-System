using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicMonthsController : ControllerBase
    {
        private readonly IAcademicMonthService _academicMonthService;

        public AcademicMonthsController(IAcademicMonthService academicMonthService)
        {
            _academicMonthService = academicMonthService;
        }

        // GET: api/AcademicMonths
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicMonth>>> GetdbsAcademicMonths()
        {
            var months = await _academicMonthService.GetAllAsync();
            return Ok(months);
        }

        // GET: api/AcademicMonths/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicMonth>> GetAcademicMonth(int id)
        {
            var academicMonth = await _academicMonthService.GetByIdAsync(id);

            if (academicMonth == null)
            {
                return NotFound();
            }

            return academicMonth;
        }

        // PUT: api/AcademicMonths/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAcademicMonth(int id, AcademicMonth academicMonth)
        {
            if (id != academicMonth.MonthId)
            {
                return BadRequest();
            }

            try
            {
                var result = await _academicMonthService.UpdateAsync(id, academicMonth);
                if (!result)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                if (!await _academicMonthService.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/AcademicMonths
        [HttpPost]
        public async Task<ActionResult<AcademicMonth>> PostAcademicMonth(AcademicMonth academicMonth)
        {
            var created = await _academicMonthService.CreateAsync(academicMonth);
            return CreatedAtAction("GetAcademicMonth", new { id = created.MonthId }, created);
        }

        // DELETE: api/AcademicMonths/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAcademicMonth(int id)
        {
            var success = await _academicMonthService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
