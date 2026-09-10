using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class CommonService : ICommonService
    {
        private readonly SchoolDbContext _context;

        public CommonService(SchoolDbContext context)
        {
            _context = context;
        }

        public string[] GetFrequencies()
        {
            return Enum.GetNames(typeof(Frequency));
        }

        public async Task<IEnumerable<MonthlyPayment>> GetAllPaymentByStudentIdAsync(int studentId)
        {
            return await _context.monthlyPayments
                .Include(p => p.PaymentDetails)
                .Include(p => p.paymentMonths)
                .Where(p => p.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OthersPayment>> GetAllOtherPaymentByStudentIdAsync(int studentId)
        {
            return await _context.othersPayments
                .Include(p => p.otherPaymentDetails)
                .Where(p => p.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DueBalance>> GetDueBalancesAsync()
        {
            return await _context.dbsDueBalance.ToListAsync();
        }

        public async Task<DueBalance?> GetDueBalanceByIdAsync(int id)
        {
            return await _context.dbsDueBalance.FindAsync(id);
        }

        public async Task<IEnumerable<object>> GetPaymentDetailsByStudentIdAsync(int studentId)
        {
            var result = await _context.PaymentDetails
                .Join(_context.monthlyPayments, pd => pd.MonthlyPaymentId, mp => mp.MonthlyPaymentId, (pd, mp) => new { pd, mp })
                .Join(_context.paymentMonths, pdmp => pdmp.mp.MonthlyPaymentId, pm => pm.MonthlyPaymentId, (pdmp, pm) => new { pdmp, pm })
                .Where(x => x.pdmp.mp.StudentId == studentId)
                .GroupBy(x => new { x.pdmp.pd.FeeName })
                .Select(group => new
                {
                    FeeName = group.Key.FeeName,
                    Months = group.Select(g => g.pm.MonthName).Distinct().ToList(),
                    NumberOfMonths = group.Count()
                })
                .OrderBy(entry => entry.FeeName)
                .ToListAsync();

            return result.Cast<object>();
        }
    }
}
