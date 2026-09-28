using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly SchoolDbContext _context;

        public DepartmentService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var depts = await _context.dbsDepartment.ToListAsync();
            return depts.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName ?? string.Empty
            });
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var d = await _context.dbsDepartment.FindAsync(id);
            if (d == null) return null;
            return new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName ?? string.Empty
            };
        }

        public async Task<DepartmentDto> CreateAsync(DepartmentDto dto)
        {
            var entity = new Department
            {
                DepartmentName = dto.DepartmentName
            };
            _context.dbsDepartment.Add(entity);
            await _context.SaveChangesAsync();
            dto.DepartmentId = entity.DepartmentId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, DepartmentDto dto)
        {
            if (id != dto.DepartmentId) return false;

            var entity = await _context.dbsDepartment.FindAsync(id);
            if (entity == null) return false;

            entity.DepartmentName = dto.DepartmentName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dept = await _context.dbsDepartment.FindAsync(id);
            if (dept == null) return false;

            _context.dbsDepartment.Remove(dept);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsDepartment.AnyAsync(e => e.DepartmentId == id);
        }
    }
}
