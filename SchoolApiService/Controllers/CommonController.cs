using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.DTOs;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Authorize]
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
        public ActionResult<ApiResponse<string[]>> GetFrequency()
        {
            var frequencies = _commonService.GetFrequencies();
            return Ok(ApiResponse<string[]>.SuccessResponse(frequencies, "Frequencies retrieved successfully."));
        }

        [HttpGet("GetAllPaymentByStudentId/{studentId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MonthlyPaymentDto>>>> GetAllPaymentByStudentId(int studentId)
        {
            var payments = (await _commonService.GetAllPaymentByStudentIdAsync(studentId)).ToList();

            if (payments.Count == 0)
            {
                return NotFound(ApiResponse<IEnumerable<MonthlyPaymentDto>>.ErrorResponse(
                    "No payments found for the specified student.",
                    new List<string> { $"No monthly payments found for student ID {studentId}." },
                    404
                ));
            }

            return Ok(ApiResponse<IEnumerable<MonthlyPaymentDto>>.SuccessResponse(payments, "Monthly payments retrieved successfully."));
        }

        [HttpGet("GetAllOtherPaymentByStudentId/{studentId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<OthersPaymentDto>>>> GetAllOtherPaymentByStudentId(int studentId)
        {
            var otherPayments = (await _commonService.GetAllOtherPaymentByStudentIdAsync(studentId)).ToList();

            if (otherPayments.Count == 0)
            {
                return NotFound(ApiResponse<IEnumerable<OthersPaymentDto>>.ErrorResponse(
                    "No other payments found for the specified student.",
                    new List<string> { $"No other payments found for student ID {studentId}." },
                    404
                ));
            }

            return Ok(ApiResponse<IEnumerable<OthersPaymentDto>>.SuccessResponse(otherPayments, "Other payments retrieved successfully."));
        }

        // GET: api/Common/DueBalances
        [HttpGet("DueBalances")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DueBalanceDto>>>> GetDueBalances()
        {
            var dueBalances = await _commonService.GetDueBalancesAsync();
            return Ok(ApiResponse<IEnumerable<DueBalanceDto>>.SuccessResponse(dueBalances, "Due balances retrieved successfully."));
        }

        // GET: api/Common/DueBalances/5
        [HttpGet("DueBalances/{id}")]
        public async Task<ActionResult<ApiResponse<DueBalanceDto>>> GetDueBalance(int id)
        {
            var dueBalance = await _commonService.GetDueBalanceByIdAsync(id);

            if (dueBalance == null)
            {
                return NotFound(ApiResponse<DueBalanceDto>.ErrorResponse(
                    "Due balance not found.",
                    new List<string> { $"No due balance found with ID {id}." },
                    404
                ));
            }

            return Ok(ApiResponse<DueBalanceDto>.SuccessResponse(dueBalance, "Due balance retrieved successfully."));
        }

        [HttpGet("GetPaymentDetailsByStudentId/{studentId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> GetPaymentDetailsByStudentId(int studentId)
        {
            var result = (await _commonService.GetPaymentDetailsByStudentIdAsync(studentId)).ToList();

            if (result.Count == 0)
            {
                return NotFound(ApiResponse<IEnumerable<object>>.ErrorResponse(
                    "No payment details found for the specified student.",
                    new List<string> { $"No payment details found for student ID {studentId}." },
                    404
                ));
            }

            return Ok(ApiResponse<IEnumerable<object>>.SuccessResponse(result, "Payment details retrieved successfully."));
        }
    }
}
