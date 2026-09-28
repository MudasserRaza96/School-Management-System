using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class StaffSalaryService : IStaffSalaryService
    {
        private readonly SchoolDbContext _context;

        public StaffSalaryService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffSalaryDto>> GetAllStaffSalariesAsync()
        {
            var salaries = await _context.dbsStaffSalary.ToListAsync();
            return salaries.Select(s => new StaffSalaryDto
            {
                StaffSalaryId = s.StaffSalaryId,
                StaffName = s.StaffName ?? string.Empty,
                BasicSalary = s.BasicSalary ?? 0,
                NetSalary = s.NetSalary ?? 0
            });
        }

        public async Task<StaffSalaryDto?> GetStaffSalaryByIdAsync(int id)
        {
            var s = await _context.dbsStaffSalary.FindAsync(id);
            if (s == null) return null;

            return new StaffSalaryDto
            {
                StaffSalaryId = s.StaffSalaryId,
                StaffName = s.StaffName ?? string.Empty,
                BasicSalary = s.BasicSalary ?? 0,
                NetSalary = s.NetSalary ?? 0
            };
        }

        public async Task<StaffSalaryDto> CreateStaffSalaryAsync(StaffSalaryDto dto)
        {
            var entity = new StaffSalary
            {
                StaffName = dto.StaffName,
                BasicSalary = dto.BasicSalary,
                NetSalary = dto.NetSalary
            };

            _context.dbsStaffSalary.Add(entity);
            await _context.SaveChangesAsync();
            dto.StaffSalaryId = entity.StaffSalaryId;
            return dto;
        }

        public async Task<bool> UpdateStaffSalaryAsync(int id, StaffSalaryDto dto)
        {
            if (id != dto.StaffSalaryId) return false;

            var entity = await _context.dbsStaffSalary.FindAsync(id);
            if (entity == null) return false;

            entity.StaffName = dto.StaffName;
            entity.BasicSalary = dto.BasicSalary;
            entity.NetSalary = dto.NetSalary;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStaffSalaryAsync(int id)
        {
            var sal = await _context.dbsStaffSalary.FindAsync(id);
            if (sal == null) return false;

            _context.dbsStaffSalary.Remove(sal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StaffSalaryExistsAsync(int id)
        {
            return await _context.dbsStaffSalary.AnyAsync(e => e.StaffSalaryId == id);
        }
    }
}
