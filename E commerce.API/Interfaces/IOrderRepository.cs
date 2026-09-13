using E_commerce.API.Models;   // mjhe order class user karni ha jo models namespace me bani hai

namespace E_commerce.API.Interfaces;

public interface IOrderRepository
{
    Task<Order> AddAsync(Order order); // database me ek order add/save karna hai

    Task<Order?> GetByIdAsync(int id);// mjhe Order ki Id do main database me us Id ka Order dhoondh kar dunga

    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);// mjhe ek order nhi multiple order return karne hai user ke

    Task<Order?> GetByIdAndUserIdAsync(int orderId, int userId);//Sirf OrderId se order mat do; check karo ki ye order isi logged-in user ka bhi hai."

    Task UpdateAsync(Order order);//Existing Order me koi change hua hai, use database me update karna hai

}