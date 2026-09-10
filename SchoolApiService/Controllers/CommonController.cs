using Microsoft.AspNetCore.Mvc;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonService _commonService;

        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        // GET: api/Common/Frequency
        [HttpGet("Frequency")]
        public ActionResult<string[]> GetFrequency()
        {
            var frequencies = _commonService.GetFrequencies();
            return Ok(frequencies);
        }

        [HttpGet("GetAllPaymentByStudentId/{studentId}")]
        public async Task<ActionResult<IEnumerable<MonthlyPayment>>> GetAllPaymentByStudentId(int studentId)
        {
            var payments = (await _commonService.GetAllPaymentByStudentIdAsync(studentId)).ToList();

            if (payments.Count == 0)
            {
                return NotFound();
            }

            return payments;
        }

        [HttpGet("GetAllOtherPaymentByStudentId/{studentId}")]
        public async Task<ActionResult<IEnumerable<OthersPayment>>> GetAllOtherPaymentByStudentId(int studentId)
        {
            var otherPayments = (await _commonService.GetAllOtherPaymentByStudentIdAsync(studentId)).ToList();

            if (otherPayments.Count == 0)
            {
                return NotFound();
            }

            return otherPayments;
        }

        // GET: api/Common/DueBalances
        [HttpGet("DueBalances")]
        public async Task<ActionResult<IEnumerable<DueBalance>>> GetDueBalances()
        {
            var dueBalances = await _commonService.GetDueBalancesAsync();
            return Ok(dueBalances);
        }

        // GET: api/Common/DueBalances/5
        [HttpGet("DueBalances/{id}")]
        public async Task<ActionResult<DueBalance>> GetDueBalance(int id)
        {
            var dueBalance = await _commonService.GetDueBalanceByIdAsync(id);

            if (dueBalance == null)
            {
                return NotFound();
            }

            return dueBalance;
        }

        [HttpGet("GetPaymentDetailsByStudentId/{studentId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPaymentDetailsByStudentId(int studentId)
        {
            var result = (await _commonService.GetPaymentDetailsByStudentIdAsync(studentId)).ToList();

            if (result.Count == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
