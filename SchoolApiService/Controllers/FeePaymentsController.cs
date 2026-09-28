using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Models;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FeePaymentsController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public FeePaymentsController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeePaymentDto>>>> GetFeePayments()
        {
            var feePayments = await _context.dbsFeePayment
                .Include(fp => fp.FeePaymentDetails)
                .ToListAsync();

            var dtos = feePayments.Select(fp => new FeePaymentDto
            {
                FeePaymentId = fp.FeePaymentId,
                StudentId = fp.StudentId,
                AmountPaid = fp.TotalAmount,
                PaymentDate = fp.PaymentDate
            });

            return Ok(ApiResponse<IEnumerable<FeePaymentDto>>.SuccessResponse(dtos, "Fee payments retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FeePaymentDto>>> GetFeePaymentById(int id)
        {
            var fp = await _context.dbsFeePayment
                .Include(fp => fp.FeePaymentDetails)
                .FirstOrDefaultAsync(fp => fp.FeePaymentId == id);

            if (fp == null)
            {
                return NotFound(ApiResponse<FeePaymentDto>.ErrorResponse($"No fee payment found with ID {id}.", statusCode: 404));
            }

            var dto = new FeePaymentDto
            {
                FeePaymentId = fp.FeePaymentId,
                StudentId = fp.StudentId,
                AmountPaid = fp.TotalAmount,
                PaymentDate = fp.PaymentDate
            };

            return Ok(ApiResponse<FeePaymentDto>.SuccessResponse(dto, "Fee payment retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<FeePaymentDto>>> CreateFeePayment(FeePaymentDto dto)
        {
            var entity = new FeePayment
            {
                StudentId = dto.StudentId,
                TotalAmount = dto.AmountPaid,
                PaymentDate = dto.PaymentDate
            };

            _context.dbsFeePayment.Add(entity);
            await _context.SaveChangesAsync();
            dto.FeePaymentId = entity.FeePaymentId;

            return CreatedAtAction(nameof(GetFeePaymentById), new { id = dto.FeePaymentId }, ApiResponse<FeePaymentDto>.SuccessResponse(dto, "Fee payment created successfully.", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateFeePayment(int id, FeePaymentDto dto)
        {
            if (id != dto.FeePaymentId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload FeePaymentId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.FeePaymentId}'." }, 400));
            }

            var entity = await _context.dbsFeePayment.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No fee payment found with ID {id} to update.", statusCode: 404));
            }

            entity.StudentId = dto.StudentId;
            entity.TotalAmount = dto.AmountPaid;
            entity.PaymentDate = dto.PaymentDate;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Fee payment updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFeePayment(int id)
        {
            var entity = await _context.dbsFeePayment.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No fee payment found with ID {id} to delete.", statusCode: 404));
            }

            _context.dbsFeePayment.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Fee payment deleted successfully."));
        }
    }
}
