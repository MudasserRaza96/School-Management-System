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
    public class FeeStructuresController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public FeeStructuresController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeeStructureDto>>>> GetFeeStructures()
        {
            var list = await _context.dbsFeeStructure.ToListAsync();
            var dtos = list.Select(f => new FeeStructureDto
            {
                FeeStructureId = f.FeeStructureId,
                StandardId = f.StandardId,
                FeeTypeId = f.FeeTypeId,
                Amount = f.Amount
            });

            return Ok(ApiResponse<IEnumerable<FeeStructureDto>>.SuccessResponse(dtos, "Fee structures retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FeeStructureDto>>> GetFeeStructure(int id)
        {
            var f = await _context.dbsFeeStructure.FindAsync(id);
            if (f == null)
            {
                return NotFound(ApiResponse<FeeStructureDto>.ErrorResponse($"No fee structure found with ID {id}.", statusCode: 404));
            }

            var dto = new FeeStructureDto
            {
                FeeStructureId = f.FeeStructureId,
                StandardId = f.StandardId,
                FeeTypeId = f.FeeTypeId,
                Amount = f.Amount
            };

            return Ok(ApiResponse<FeeStructureDto>.SuccessResponse(dto, "Fee structure retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<FeeStructureDto>>> PostFeeStructure(FeeStructureDto dto)
        {
            var entity = new FeeStructure
            {
                StandardId = dto.StandardId,
                FeeTypeId = dto.FeeTypeId,
                Amount = dto.Amount
            };

            _context.dbsFeeStructure.Add(entity);
            await _context.SaveChangesAsync();
            dto.FeeStructureId = entity.FeeStructureId;

            return CreatedAtAction(nameof(GetFeeStructure), new { id = dto.FeeStructureId }, ApiResponse<FeeStructureDto>.SuccessResponse(dto, "Fee structure created successfully.", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateFeeStructure(int id, FeeStructureDto dto)
        {
            var entity = await _context.dbsFeeStructure.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No fee structure found with ID {id} to update.", statusCode: 404));
            }

            entity.StandardId = dto.StandardId;
            entity.FeeTypeId = dto.FeeTypeId;
            entity.Amount = dto.Amount;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Fee structure updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFeeStructure(int id)
        {
            var entity = await _context.dbsFeeStructure.FindAsync(id);
            if (entity == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No fee structure found with ID {id} to delete.", statusCode: 404));
            }

            _context.dbsFeeStructure.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Fee structure deleted successfully."));
        }
    }
}
