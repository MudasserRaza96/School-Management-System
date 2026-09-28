using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
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

        public async Task<IEnumerable<MonthlyPaymentDto>> GetAllPaymentByStudentIdAsync(int studentId)
        {
            var payments = await _context.monthlyPayments
                .Include(p => p.Student)
                .Where(p => p.StudentId == studentId)
                .ToListAsync();

            return payments.Select(p => new MonthlyPaymentDto
            {
                MonthlyPaymentId = p.MonthlyPaymentId,
                StudentId = p.StudentId ?? 0,
                StudentName = p.Student?.StudentName,
                Amount = p.TotalAmount,
                PaymentDate = p.PaymentDate
            });
        }

        public async Task<IEnumerable<OthersPaymentDto>> GetAllOtherPaymentByStudentIdAsync(int studentId)
        {
            var payments = await _context.othersPayments
                .Include(p => p.Student)
                .Where(p => p.StudentId == studentId)
                .ToListAsync();

            return payments.Select(p => new OthersPaymentDto
            {
                OthersPaymentId = p.OthersPaymentId,
                StudentId = p.StudentId ?? 0,
                StudentName = p.Student?.StudentName,
                Amount = p.TotalAmount,
                PaymentDate = p.PaymentDate
            });
        }

        public async Task<IEnumerable<DueBalanceDto>> GetDueBalancesAsync()
        {
            var dueBalances = await _context.dbsDueBalance
                .Include(d => d.Student)
                .ToListAsync();

            return dueBalances.Select(d => new DueBalanceDto
            {
                DueBalanceId = d.DueBalanceId,
                StudentId = d.StudentId ?? 0,
                StudentName = d.Student?.StudentName,
                DueAmount = d.DueBalanceAmount ?? 0
            });
        }

        public async Task<DueBalanceDto?> GetDueBalanceByIdAsync(int id)
        {
            var d = await _context.dbsDueBalance
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.DueBalanceId == id);

            if (d == null) return null;

            return new DueBalanceDto
            {
                DueBalanceId = d.DueBalanceId,
                StudentId = d.StudentId ?? 0,
                StudentName = d.Student?.StudentName,
                DueAmount = d.DueBalanceAmount ?? 0
            };
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
