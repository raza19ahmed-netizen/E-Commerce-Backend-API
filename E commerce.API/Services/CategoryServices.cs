
using AutoMapper;
using E_commerce.API.DTOs;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using E_commerce.API.Repositories;

namespace E_commerce.API.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;
        public CategoryServices(ICategoryRepository repository, IMapper mapper, IProductRepository productRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _productRepository = productRepository;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                { 
                throw new KeyNotFoundException("Category not found.");
                   }
            return _mapper.Map<CategoryDto>(category);
        }
        public async Task<CategoryDto> AddAsync(CreateCategoryDto dto)
        {
            var exists = await _repository.ExistsByNameAsync(dto.Name);
            if (exists)
            {
                throw new InvalidOperationException("Category with this name already exists");
            }
            var category= _mapper.Map<Category>(dto);
            await _repository.AddAsync(category);
            return _mapper.Map<CategoryDto>(category);
        }
        public async Task <CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found");
            var exists = await _repository.ExistsByNameAsync(dto.Name, id);
            if (exists)
            {
                throw new InvalidOperationException("Category with this name already exists");
            }
            _mapper.Map(dto, category);
            await _repository.UpdateAsync(category);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException(
                    "Category not found.");
            }

            var hasProducts = await _productRepository
                .ExistsByCategoryIdAsync(id);

            if (hasProducts)
            {
                throw new InvalidOperationException(
                    "Cannot delete category because products are associated with it.");
            }

            await _repository.DeleteAsync(category);
        
    }
  
   }
}
