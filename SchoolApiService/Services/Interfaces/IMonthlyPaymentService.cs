using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IMonthlyPaymentService
    {
        Task<IEnumerable<MonthlyPayment>> GetAllAsync();
        Task<MonthlyPayment?> GetByIdAsync(int id);
        Task<MonthlyPayment> CreateAsync(MonthlyPayment monthlyPayment);
        Task<MonthlyPayment?> UpdateAsync(int id, MonthlyPayment updatedmonthlyPayment);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
