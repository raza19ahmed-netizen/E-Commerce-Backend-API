using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class ReturnRepository : IReturnRepository
{
    private readonly ApplicationDbContext _context;

    public ReturnRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReturnRequest> AddAsync(ReturnRequest returnRequest)
    {
        _context.ReturnRequests.Add(returnRequest);
        await _context.SaveChangesAsync();

        return returnRequest;
    }

    public async Task<ReturnRequest?> GetByIdAsync(int id)
    {
        return await _context.ReturnRequests
            .Include(r => r.Order)
            .ThenInclude(o => o.Payment)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<ReturnRequest?> GetByOrderIdAsync(int orderId)
    {
        return await _context.ReturnRequests
            .Include(r => r.Order)
            .ThenInclude(o => o.Payment)
            .FirstOrDefaultAsync(r => r.OrderId == orderId);
    }

    public async Task<IEnumerable<ReturnRequest>> GetByUserIdAsync(int userId)
    {
        return await _context.ReturnRequests
            .Include(r => r.Order)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(ReturnRequest returnRequest)
    {
        _context.ReturnRequests.Update(returnRequest);

        if (returnRequest.Order != null)
        {
            _context.Orders.Update(returnRequest.Order);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ReturnRequest>> GetAllAsync()
    {
        return await _context.ReturnRequests
            .Include(r => r.Order)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}