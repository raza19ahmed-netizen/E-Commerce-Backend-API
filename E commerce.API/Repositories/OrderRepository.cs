using E_commerce.API.Data; // data bsae se baat kaene ke liye
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;// mjhe EF core ke database ke method chahiye(AddAsync(),SaveChangesAsync(),Include(),ThenInclude(),FirstOrDefaultAsync(),ToListAsync(),Update())

namespace E_commerce.API.Repositories;

public class OrderRepository : IOrderRepository //OrderRepository ka kaam sirf database se Order ka data lana, save karna aur update karna hai. Business decision OrderService karegi.
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> AddAsync(Order order)//"Mujhe ek Order receive karke database me save karna hai."
    {
        await _context.Orders.AddAsync(order);//"Ye Order database me add karne ke liye context ko batao."
        await _context.SaveChangesAsync(); //Ab jo change track ho raha hai, usko database me actually save karo."

        return order;
    }

    public async Task<Order?> GetByIdAsync(int id)//"Mujhe Order ki ID milegi, aur main us ID ka complete Order dhoondh kar dunga."
    {
        return await _context.Orders // orders table me search karo
            .Include(o => o.OrderItems)//"Order ke saath uske OrderItems bhi lekar aao."
            .ThenInclude(oi => oi.Product)//OrderItems bhi aa gaye, ab har OrderItem ka Product bhi lekar aao."
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id);//"Orders me se pehla Order do jiska Id requested id ke equal hai."
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)//Ek user ki saari orders chahiye."
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)//Orders ko latest order sabse upar rakh kar do."
            .ToListAsync();//Query ka final result database se lekar ek List me convert karke return karo."
    }

    public async Task<Order?> GetByIdAndUserIdAsync(int orderId, int userId)//OrderId bhi match honi chahiye aur UserId bhi match honi chahiye."
    {
        return await _context.Orders
            .Where(o => o.Id == orderId && o.UserId == userId)// orderId & userId dono request ki orderId & userId ke equal honi chahiye
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(); //Kyuki OrderId unique hai, maximum ek Order milega.
    }

    public async Task UpdateAsync(Order order)
    {
     _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}// Database ka saara Order-related kaam ek jagah rakho: Order save karna, ID se Order lana, user ke saare Orders lana, ownership verify karke specific Order lana, aur Order update karna.Service ko database ke internal details pata hone ki zarurat nahi honi chahiye."
