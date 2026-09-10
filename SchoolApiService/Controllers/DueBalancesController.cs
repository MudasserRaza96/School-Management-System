using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DueBalancesController : ControllerBase
    {
        private readonly IDueBalanceService _dueBalanceService;

        public DueBalancesController(IDueBalanceService dueBalanceService)
        {
            _dueBalanceService = dueBalanceService;
        }

        // GET: api/DueBalances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DueBalance>>> GetdbsDueBalance()
        {
            var dueBalances = await _dueBalanceService.GetAllAsync();
            return Ok(dueBalances);
        }

        // GET: api/DueBalances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DueBalance>> GetDueBalance(int id)
        {
            var dueBalance = await _dueBalanceService.GetByIdAsync(id);

            if (dueBalance == null)
            {
                return NotFound();
            }

            return dueBalance;
        }

        // PUT: api/DueBalances/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDueBalance(int id, DueBalance dueBalance)
        {
            if (id != dueBalance.DueBalanceId)
            {
                return BadRequest();
            }

            try
            {
                var result = await _dueBalanceService.UpdateAsync(id, dueBalance);
                if (!result)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                if (!await _dueBalanceService.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/DueBalances
        [HttpPost]
        public async Task<ActionResult<DueBalance>> PostDueBalance(DueBalance dueBalance)
        {
            var created = await _dueBalanceService.CreateAsync(dueBalance);
            return CreatedAtAction("GetDueBalance", new { id = created.DueBalanceId }, created);
        }

        // DELETE: api/DueBalances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDueBalance(int id)
        {
            var success = await _dueBalanceService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
