using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface ICommonService
    {
        string[] GetFrequencies();
        Task<IEnumerable<MonthlyPayment>> GetAllPaymentByStudentIdAsync(int studentId);
        Task<IEnumerable<OthersPayment>> GetAllOtherPaymentByStudentIdAsync(int studentId);
        Task<IEnumerable<DueBalance>> GetDueBalancesAsync();
        Task<DueBalance?> GetDueBalanceByIdAsync(int id);
        Task<IEnumerable<object>> GetPaymentDetailsByStudentIdAsync(int studentId);
    }
}
