using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly SchoolDbContext _context;

        public SubjectService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllAsync()
        {
            var subjs = await _context.dbsSubject.ToListAsync();
            return subjs.Select(s => new SubjectDto
            {
                SubjectId = s.SubjectId,
                SubjectName = s.SubjectName ?? string.Empty,
                SubjectCode = s.SubjectCode
            });
        }

        public async Task<SubjectDto?> GetByIdAsync(int id)
        {
            var s = await _context.dbsSubject.FindAsync(id);
            if (s == null) return null;
            return new SubjectDto
            {
                SubjectId = s.SubjectId,
                SubjectName = s.SubjectName ?? string.Empty,
                SubjectCode = s.SubjectCode
            };
        }

        public async Task<SubjectDto> CreateAsync(SubjectDto dto)
        {
            var entity = new Subject
            {
                SubjectName = dto.SubjectName,
                SubjectCode = dto.SubjectCode
            };
            _context.dbsSubject.Add(entity);
            await _context.SaveChangesAsync();
            dto.SubjectId = entity.SubjectId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, SubjectDto dto)
        {
            if (id != dto.SubjectId) return false;

            var entity = await _context.dbsSubject.FindAsync(id);
            if (entity == null) return false;

            entity.SubjectName = dto.SubjectName;
            entity.SubjectCode = dto.SubjectCode;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subj = await _context.dbsSubject.FindAsync(id);
            if (subj == null) return false;

            _context.dbsSubject.Remove(subj);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsSubject.AnyAsync(e => e.SubjectId == id);
        }
    }
}
