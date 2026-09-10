using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeeTypesController : ControllerBase
    {
        private readonly IFeeTypeService _feeTypeService;

        public FeeTypesController(IFeeTypeService feeTypeService)
        {
            _feeTypeService = feeTypeService;
        }

        // GET: api/FeeTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeeType>>> GetdbsFeeType()
        {
            var feeTypes = await _feeTypeService.GetAllAsync();
            return Ok(feeTypes);
        }

        // GET: api/FeeTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FeeType>> GetFeeType(int id)
        {
            var feeType = await _feeTypeService.GetByIdAsync(id);

            if (feeType == null)
            {
                return NotFound();
            }

            return feeType;
        }

        // PUT: api/FeeTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeeType(int id, FeeType feeType)
        {
            if (id != feeType.FeeTypeId)
            {
                return BadRequest();
            }

            try
            {
                var result = await _feeTypeService.UpdateAsync(id, feeType);
                if (!result)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                if (!await _feeTypeService.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/FeeTypes
        [HttpPost]
        public async Task<ActionResult<FeeType>> PostFeeType(FeeType feeType)
        {
            var created = await _feeTypeService.CreateAsync(feeType);
            return CreatedAtAction("GetFeeType", new { id = created.FeeTypeId }, created);
        }

        // DELETE: api/FeeTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeeType(int id)
        {
            var success = await _feeTypeService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
