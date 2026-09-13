using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class RefundRepository : IRefundRepository
{
    private readonly ApplicationDbContext _context;

    public RefundRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Refund> AddAsync(Refund refund)
    {
        _context.Refunds.Add(refund);

        await _context.SaveChangesAsync();

        return refund;
    }

    public async Task<Refund?> GetByIdAsync(int id)
    {
        return await _context.Refunds
            .Include(r => r.Payment)
            .ThenInclude(p => p.Order)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Refund?> GetByPaymentIdAsync(int paymentId)
    {
        return await _context.Refunds
            .Include(r => r.Payment)
            .ThenInclude(p => p.Order)
            .FirstOrDefaultAsync(r => r.PaymentId == paymentId);
    }

    public async Task<IEnumerable<Refund>> GetAllAsync()
    {
        return await _context.Refunds
            .Include(r => r.Payment)
            .ThenInclude(p => p.Order)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Refund refund)
    {
        _context.Refunds.Update(refund);

        if (refund.Payment != null)
        {
            _context.Payments.Update(refund.Payment);

            if (refund.Payment.Order != null)
            {
                _context.Orders.Update(refund.Payment.Order);
            }
        }

        await _context.SaveChangesAsync();
    }


}