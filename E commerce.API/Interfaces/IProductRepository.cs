using E_commerce.API.Models;

namespace E_commerce.API.Interfaces
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> Products, int TotalItems)> GetProductsAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy, string? sortOrder, int pageNumber, int pageSize);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<bool> ExistsByCategoryIdAsync(int categoryId);

    }
}
