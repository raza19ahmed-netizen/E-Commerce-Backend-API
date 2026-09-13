using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.API.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;

    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Address> AddAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();

        return address;
    }

    public async Task<IEnumerable<Address>> GetByUserIdAsync(int userId)
    {
        return await _context.Addresses
            .Where(a => a.UserId == userId)//"Har Address ko check karo aur sirf wahi rakho jiska UserId current userId ke equal hai."
            .OrderByDescending(a => a.IsDefault)//"Default address sabse upar dikhna chahiye."
            .ThenByDescending(a => a.Id)//Agar IsDefault same hai, to ID ke basis par latest/highest ID pehle dikhao.
            .ToListAsync();//Ab query actually execute hogi aur result list me aa jayega
    }

    public async Task<Address?> GetByIdAsync(int id)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == id);//Pehla matching record do; agar koi nahi mila to null.
    }

    public async Task UpdateAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Address address)
    {
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
    }
}