using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IOthersPaymentService
    {
        Task<IEnumerable<OthersPayment>> GetAllAsync();
        Task<OthersPayment?> GetByIdAsync(int id);
        Task<OthersPayment> CreateAsync(OthersPayment othersPayment);
        Task<OthersPayment?> UpdateAsync(int id, OthersPayment updatedPayment);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
