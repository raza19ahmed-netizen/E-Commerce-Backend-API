
using E_commerce.API.Models;
namespace E_commerce.API.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<Category> DeleteAsync(Category category);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
