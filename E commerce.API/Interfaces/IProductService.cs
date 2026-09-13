
using E_commerce.API.DTOs;
using E_commerce.API.Helpers;

namespace E_commerce.API.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(string? search, int? categoryId, decimal? minPrice , decimal? maxPrice, string? sortBy, string? sortOrder, int pageNumber, int pageSize);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> AddAsync(CreateProductDto dto);
        Task<ProductDto>UpdateAsync(int id, UpdateProductDto dto);
        Task DeleteAsync(int id);
    }
}
