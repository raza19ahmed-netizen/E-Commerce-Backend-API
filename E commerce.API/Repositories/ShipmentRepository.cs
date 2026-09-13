using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly ApplicationDbContext _context;

    public ShipmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Shipment> AddAsync(Shipment shipment)
    {
        await _context.Shipments.AddAsync(shipment);
        await _context.SaveChangesAsync();

        return shipment;
    }

    public async Task<Shipment?> GetByIdAsync(int id)
    {
        return await _context.Shipments
            .Include(s => s.Order)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shipment?> GetByOrderIdAsync(int orderId)
    {
        return await _context.Shipments
            .Include(s => s.Order)
            .FirstOrDefaultAsync(s => s.OrderId == orderId);
    }

    public async Task UpdateAsync(Shipment shipment)
    {
        _context.Shipments.Update(shipment);

        if (shipment.Order != null)
        {
            _context.Orders.Update(shipment.Order);
        }

        await _context.SaveChangesAsync();
    }
}