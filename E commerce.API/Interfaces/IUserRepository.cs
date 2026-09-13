using E_commerce.API.Models;

namespace E_commerce.API.Interfaces;

public interface IUserRepository // user ke data ko database se lene aur database me add karna ke liye //
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);

    Task<User> AddAsync(User user);

    Task UpdateAsync(User user);
    
}