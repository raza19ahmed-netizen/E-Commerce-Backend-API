using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> AddAsync(Payment payment)
    {
        _context.Payments.Add(payment);

        if (payment.Order != null)
        {
            _context.Orders.Update(payment.Order);
        }

        await _context.SaveChangesAsync();

        return payment;
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Payment?> GetByOrderIdAsync(int orderId)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);

        if (payment.Order != null)
        {
            _context.Orders.Update(payment.Order);
        }

        await _context.SaveChangesAsync();
    }
}