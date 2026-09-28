using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class ExamTypeService : IExamTypeService
    {
        private readonly SchoolDbContext _context;

        public ExamTypeService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamTypeDto>> GetAllAsync()
        {
            var types = await _context.dbsExamType.ToListAsync();
            return types.Select(t => new ExamTypeDto
            {
                ExamTypeId = t.ExamTypeId,
                ExamTypeName = t.ExamTypeName ?? string.Empty
            });
        }

        public async Task<ExamTypeDto?> GetByIdAsync(int id)
        {
            var t = await _context.dbsExamType.FindAsync(id);
            if (t == null) return null;

            return new ExamTypeDto
            {
                ExamTypeId = t.ExamTypeId,
                ExamTypeName = t.ExamTypeName ?? string.Empty
            };
        }

        public async Task<ExamTypeDto> CreateAsync(ExamTypeDto dto)
        {
            var entity = new ExamType
            {
                ExamTypeName = dto.ExamTypeName
            };
            _context.dbsExamType.Add(entity);
            await _context.SaveChangesAsync();
            dto.ExamTypeId = entity.ExamTypeId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, ExamTypeDto dto)
        {
            if (id != dto.ExamTypeId) return false;

            var entity = await _context.dbsExamType.FindAsync(id);
            if (entity == null) return false;

            entity.ExamTypeName = dto.ExamTypeName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var type = await _context.dbsExamType.FindAsync(id);
            if (type == null) return false;

            _context.dbsExamType.Remove(type);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsExamType.AnyAsync(e => e.ExamTypeId == id);
        }
    }
}
