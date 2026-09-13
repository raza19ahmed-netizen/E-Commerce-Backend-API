using E_commerce.API.Data;
using E_commerce.API.DTOs;
namespace E_commerce.API.Interfaces
{
    public interface ICategoryServices
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<CategoryDto> AddAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto);
        Task DeleteAsync(int id);

    }
}
