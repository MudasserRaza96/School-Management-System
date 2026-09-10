using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IFeeTypeService
    {
        Task<IEnumerable<FeeType>> GetAllAsync();
        Task<FeeType?> GetByIdAsync(int id);
        Task<FeeType> CreateAsync(FeeType feeType);
        Task<bool> UpdateAsync(int id, FeeType feeType);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
