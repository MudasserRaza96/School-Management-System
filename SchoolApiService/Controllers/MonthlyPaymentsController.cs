using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlyPaymentsController : ControllerBase
    {
        private readonly IMonthlyPaymentService _monthlyPaymentService;

        public MonthlyPaymentsController(IMonthlyPaymentService monthlyPaymentService)
        {
            _monthlyPaymentService = monthlyPaymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyPayments()
        {
            try
            {
                var monthlyPayments = await _monthlyPaymentService.GetAllAsync();
                return Ok(monthlyPayments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMonthlyPaymentById(int id)
        {
            try
            {
                var monthlyPayment = await _monthlyPaymentService.GetByIdAsync(id);

                if (monthlyPayment == null)
                {
                    return NotFound($"monthlyPayment with ID {id} not found");
                }

                return Ok(monthlyPayment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMonthlyPayment(int id, [FromBody] MonthlyPayment updatedmonthlyPayment)
        {
            if (id != updatedmonthlyPayment.MonthlyPaymentId)
            {
                return BadRequest("ID Mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _monthlyPaymentService.UpdateAsync(id, updatedmonthlyPayment);
                if (updated == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMonthlyPayment([FromBody] MonthlyPayment monthlyPayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _monthlyPaymentService.CreateAsync(monthlyPayment);
                return Ok(created);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, "Internal Server Error: An error occurred while processing the request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMonthlyPayment(int id)
        {
            var success = await _monthlyPaymentService.DeleteAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
