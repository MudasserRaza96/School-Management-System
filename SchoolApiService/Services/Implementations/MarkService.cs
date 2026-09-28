using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class MarkService : IMarkService
    {
        private readonly SchoolDbContext _context;

        public MarkService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MarkDto>> GetAllMarksAsync()
        {
            var marks = await _context.dbsMark.Include(m => m.Student).ToListAsync();
            return marks.Select(m => new MarkDto
            {
                MarkId = m.MarkId,
                StudentId = m.StudentId,
                StudentName = m.Student?.StudentName,
                SubjectId = m.SubjectId,
                ObtainedMarks = m.ObtainedScore,
                Grade = m.Grade.ToString()
            });
        }

        public async Task<MarkDto?> GetMarkByIdAsync(int id)
        {
            var m = await _context.dbsMark.Include(m => m.Student).FirstOrDefaultAsync(m => m.MarkId == id);
            if (m == null) return null;

            return new MarkDto
            {
                MarkId = m.MarkId,
                StudentId = m.StudentId,
                StudentName = m.Student?.StudentName,
                SubjectId = m.SubjectId,
                ObtainedMarks = m.ObtainedScore,
                Grade = m.Grade.ToString()
            };
        }

        public async Task<(bool Succeeded, string? ErrorMessage, MarkDto? CreatedMark)> CreateMarkAsync(MarkDto dto)
        {
            Grade gradeEnum = Grade.F;
            if (!string.IsNullOrWhiteSpace(dto.Grade) && Enum.TryParse<Grade>(dto.Grade, true, out var g))
            {
                gradeEnum = g;
            }

            var entity = new Mark
            {
                StudentId = dto.StudentId,
                SubjectId = dto.SubjectId,
                ObtainedScore = (int)dto.ObtainedMarks,
                Grade = gradeEnum
            };

            _context.dbsMark.Add(entity);
            await _context.SaveChangesAsync();
            dto.MarkId = entity.MarkId;
            return (true, null, dto);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateMarkAsync(int id, MarkDto dto)
        {
            if (id != dto.MarkId) return (false, "Invalid MarkId", false);

            var entity = await _context.dbsMark.FindAsync(id);
            if (entity == null) return (false, "Mark record not found.", true);

            Grade gradeEnum = Grade.F;
            if (!string.IsNullOrWhiteSpace(dto.Grade) && Enum.TryParse<Grade>(dto.Grade, true, out var g))
            {
                gradeEnum = g;
            }

            entity.StudentId = dto.StudentId;
            entity.SubjectId = dto.SubjectId;
            entity.ObtainedScore = (int)dto.ObtainedMarks;
            entity.Grade = gradeEnum;

            await _context.SaveChangesAsync();
            return (true, null, false);
        }

        public async Task<bool> DeleteMarkAsync(int id)
        {
            var mark = await _context.dbsMark.FindAsync(id);
            if (mark == null) return false;

            _context.dbsMark.Remove(mark);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkExistsAsync(int id)
        {
            return await _context.dbsMark.AnyAsync(e => e.MarkId == id);
        }
    }
}
