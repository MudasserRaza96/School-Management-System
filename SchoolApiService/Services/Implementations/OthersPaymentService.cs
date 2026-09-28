using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class OthersPaymentService : IOthersPaymentService
    {
        private readonly SchoolDbContext _context;

        public OthersPaymentService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OthersPaymentDto>> GetAllAsync()
        {
            var list = await _context.othersPayments.Include(p => p.Student).ToListAsync();
            return list.Select(p => new OthersPaymentDto
            {
                OthersPaymentId = p.OthersPaymentId,
                StudentId = p.StudentId ?? 0,
                StudentName = p.Student?.StudentName,
                Amount = p.TotalAmount,
                PaymentDate = p.PaymentDate
            });
        }

        public async Task<OthersPaymentDto?> GetByIdAsync(int id)
        {
            var p = await _context.othersPayments.Include(p => p.Student).FirstOrDefaultAsync(p => p.OthersPaymentId == id);
            if (p == null) return null;

            return new OthersPaymentDto
            {
                OthersPaymentId = p.OthersPaymentId,
                StudentId = p.StudentId ?? 0,
                StudentName = p.Student?.StudentName,
                Amount = p.TotalAmount,
                PaymentDate = p.PaymentDate
            };
        }

        public async Task<OthersPaymentDto> CreateAsync(OthersPaymentDto dto)
        {
            var entity = new OthersPayment
            {
                StudentId = dto.StudentId,
                TotalAmount = dto.Amount,
                PaymentDate = dto.PaymentDate
            };

            _context.othersPayments.Add(entity);
            await _context.SaveChangesAsync();
            dto.OthersPaymentId = entity.OthersPaymentId;
            return dto;
        }

        public async Task<OthersPaymentDto?> UpdateAsync(int id, OthersPaymentDto dto)
        {
            var entity = await _context.othersPayments.FindAsync(id);
            if (entity == null) return null;

            entity.StudentId = dto.StudentId;
            entity.TotalAmount = dto.Amount;
            entity.PaymentDate = dto.PaymentDate;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _context.othersPayments.FindAsync(id);
            if (p == null) return false;

            _context.othersPayments.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.othersPayments.AnyAsync(e => e.OthersPaymentId == id);
        }
    }
}
