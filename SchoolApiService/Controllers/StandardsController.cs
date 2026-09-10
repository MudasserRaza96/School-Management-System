using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandardsController : ControllerBase
    {
        private readonly IStandardService _standardService;

        public StandardsController(IStandardService standardService)
        {
            _standardService = standardService;
        }

        // GET: api/Standards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Standard>>> GetdbsStandard()
        {
            var standards = await _standardService.GetAllAsync();
            return Ok(standards);
        }

        // GET: api/Standards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Standard>> GetStandard(int id)
        {
            var standard = await _standardService.GetByIdAsync(id);

            if (standard == null)
            {
                return NotFound();
            }

            return standard;
        }

        // PUT: api/Standards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStandard(int id, Standard standard)
        {
            if (id != standard.StandardId)
            {
                return BadRequest();
            }

            try
            {
                var result = await _standardService.UpdateAsync(id, standard);
                if (!result)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                if (!await _standardService.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/Standards
        [HttpPost]
        public async Task<ActionResult<Standard>> PostStandard(Standard standard)
        {
            var created = await _standardService.CreateAsync(standard);
            return CreatedAtAction("GetStandard", new { id = created.StandardId }, created);
        }

        // DELETE: api/Standards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStandard(int id)
        {
            var (success, errorMessage) = await _standardService.DeleteAsync(id);
            if (!success)
            {
                if (errorMessage != null)
                {
                    return BadRequest(errorMessage);
                }
                return NotFound();
            }

            return NoContent();
        }
    }
}
