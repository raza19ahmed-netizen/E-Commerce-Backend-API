using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using E_commerce.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryService;

        public CategoryController(ICategoryServices categoryService)
        {
            _categoryService = categoryService;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<CategoryDto>>(true, "Category retrieved succesfully.", result));
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

          //  if (category == null)
          //  {
           //     throw new KeyNotFoundException("Category not found.");
           // }

            return Ok(new ApiResponse<CategoryDto>(true, "Category retrieved successfully.", category));
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> AddCategory(CreateCategoryDto dto)
        {
            var category = await _categoryService.AddAsync(dto);

            return CreatedAtAction(nameof(GetCategoryById),
                new { id = category.Id }, new ApiResponse<CategoryDto>(true, "Category created successfully.", category));
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new ApiResponse<object>(false, "Id mismatch."));
            }

            await _categoryService.UpdateAsync(id, dto);

            return Ok(new ApiResponse<object>(true, "Category upadted Successfully."));

        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteAsync(id);

            return Ok(new ApiResponse<object>(true, "Category delete Successfully."));

        }
    }
}