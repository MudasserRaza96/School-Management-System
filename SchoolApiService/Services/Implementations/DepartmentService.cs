using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
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

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.dbsDepartment.ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.dbsDepartment.FindAsync(id);
        }

        public async Task<Department> CreateAsync(Department department)
        {
            _context.dbsDepartment.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<bool> UpdateAsync(int id, Department department)
        {
            if (id != department.DepartmentId) return false;

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _context.dbsDepartment.FindAsync(id);
            if (department == null) return false;

            _context.dbsDepartment.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsDepartment.AnyAsync(e => e.DepartmentId == id);
        }
    }
}
