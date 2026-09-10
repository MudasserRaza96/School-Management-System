using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OthersPaymentsController : ControllerBase
    {
        private readonly IOthersPaymentService _othersPaymentService;

        public OthersPaymentsController(IOthersPaymentService othersPaymentService)
        {
            _othersPaymentService = othersPaymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetothersPayments()
        {
            try
            {
                var othersPayments = await _othersPaymentService.GetAllAsync();
                return Ok(othersPayments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOthersPaymentById(int id)
        {
            try
            {
                var monthlyPayment = await _othersPaymentService.GetByIdAsync(id);

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
        public async Task<IActionResult> UpdateOthersPayment(int id, [FromBody] OthersPayment updatedPayment)
        {
            if (id != updatedPayment.OthersPaymentId)
            {
                return BadRequest("Invalid ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _othersPaymentService.UpdateAsync(id, updatedPayment);
                if (result == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, "Internal Server Error: An error occurred while processing the request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOthersPayment([FromBody] OthersPayment othersPayment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _othersPaymentService.CreateAsync(othersPayment);
                return Ok(created);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return StatusCode(500, "Internal Server Error: An error occurred while processing the request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOthersPayment(int id)
        {
            var success = await _othersPaymentService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
