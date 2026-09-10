using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IAcademicMonthService
    {
        Task<IEnumerable<AcademicMonth>> GetAllAsync();
        Task<AcademicMonth?> GetByIdAsync(int id);
        Task<AcademicMonth> CreateAsync(AcademicMonth academicMonth);
        Task<bool> UpdateAsync(int id, AcademicMonth academicMonth);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
