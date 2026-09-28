using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface ICommonService
    {
        string[] GetFrequencies();
        Task<IEnumerable<MonthlyPaymentDto>> GetAllPaymentByStudentIdAsync(int studentId);
        Task<IEnumerable<OthersPaymentDto>> GetAllOtherPaymentByStudentIdAsync(int studentId);
        Task<IEnumerable<DueBalanceDto>> GetDueBalancesAsync();
        Task<DueBalanceDto?> GetDueBalanceByIdAsync(int id);
        Task<IEnumerable<object>> GetPaymentDetailsByStudentIdAsync(int studentId);
    }
}
