using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class FeeTypeService : IFeeTypeService
    {
        private readonly SchoolDbContext _context;

        public FeeTypeService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeeTypeDto>> GetAllAsync()
        {
            var types = await _context.dbsFeeType.ToListAsync();
            return types.Select(t => new FeeTypeDto
            {
                FeeTypeId = t.FeeTypeId,
                FeeTypeName = t.TypeName ?? string.Empty
            });
        }

        public async Task<FeeTypeDto?> GetByIdAsync(int id)
        {
            var t = await _context.dbsFeeType.FindAsync(id);
            if (t == null) return null;

            return new FeeTypeDto
            {
                FeeTypeId = t.FeeTypeId,
                FeeTypeName = t.TypeName ?? string.Empty
            };
        }

        public async Task<FeeTypeDto> CreateAsync(FeeTypeDto dto)
        {
            var entity = new FeeType
            {
                TypeName = dto.FeeTypeName
            };
            _context.dbsFeeType.Add(entity);
            await _context.SaveChangesAsync();
            dto.FeeTypeId = entity.FeeTypeId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, FeeTypeDto dto)
        {
            if (id != dto.FeeTypeId) return false;

            var entity = await _context.dbsFeeType.FindAsync(id);
            if (entity == null) return false;

            entity.TypeName = dto.FeeTypeName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var type = await _context.dbsFeeType.FindAsync(id);
            if (type == null) return false;

            _context.dbsFeeType.Remove(type);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsFeeType.AnyAsync(e => e.FeeTypeId == id);
        }
    }
}
