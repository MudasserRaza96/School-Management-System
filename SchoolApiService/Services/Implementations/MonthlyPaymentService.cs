using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class MonthlyPaymentService : IMonthlyPaymentService
    {
        private readonly SchoolDbContext _context;

        public MonthlyPaymentService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonthlyPaymentDto>> GetAllAsync()
        {
            var list = await _context.monthlyPayments.Include(m => m.Student).ToListAsync();
            return list.Select(m => new MonthlyPaymentDto
            {
                MonthlyPaymentId = m.MonthlyPaymentId,
                StudentId = m.StudentId ?? 0,
                StudentName = m.Student?.StudentName,
                Amount = m.TotalAmount,
                PaymentDate = m.PaymentDate
            });
        }

        public async Task<MonthlyPaymentDto?> GetByIdAsync(int id)
        {
            var m = await _context.monthlyPayments.Include(m => m.Student).FirstOrDefaultAsync(m => m.MonthlyPaymentId == id);
            if (m == null) return null;

            return new MonthlyPaymentDto
            {
                MonthlyPaymentId = m.MonthlyPaymentId,
                StudentId = m.StudentId ?? 0,
                StudentName = m.Student?.StudentName,
                Amount = m.TotalAmount,
                PaymentDate = m.PaymentDate
            };
        }

        public async Task<MonthlyPaymentDto> CreateAsync(MonthlyPaymentDto dto)
        {
            var entity = new MonthlyPayment
            {
                StudentId = dto.StudentId,
                TotalAmount = dto.Amount,
                PaymentDate = dto.PaymentDate
            };

            _context.monthlyPayments.Add(entity);
            await _context.SaveChangesAsync();
            dto.MonthlyPaymentId = entity.MonthlyPaymentId;
            return dto;
        }

        public async Task<MonthlyPaymentDto?> UpdateAsync(int id, MonthlyPaymentDto dto)
        {
            var entity = await _context.monthlyPayments.FindAsync(id);
            if (entity == null) return null;

            entity.StudentId = dto.StudentId;
            entity.TotalAmount = dto.Amount;
            entity.PaymentDate = dto.PaymentDate;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _context.monthlyPayments.FindAsync(id);
            if (p == null) return false;

            _context.monthlyPayments.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.monthlyPayments.AnyAsync(e => e.MonthlyPaymentId == id);
        }
    }
}
