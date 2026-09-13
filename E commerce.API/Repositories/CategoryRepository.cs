using Microsoft.EntityFrameworkCore;
using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;

namespace E_commerce.API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category?> GetByIdAsync(int id)
        {
         return await _context.Categories.FindAsync(id);
        }
        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;

        }

        public async Task<Category> DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return category;

        }

        public async Task<bool> ExistsByNameAsync(string name , int? excludeId = null) //developer kahe rahe hai mjhe ek Category ka name diya jayega mai database check karke true or false bataunga isliye return type "bool" hai
        {
            var query = _context.Categories.Where(c => c.Name.ToLower() == name.ToLower()); // developer bolraha hai Category table me mjhe wahi category chahiye jinka Name user ke diye hue name ke barabar hai//
            if (excludeId.HasValue) // developer pooch raha hai kya mjhe koi catogoryId ko ignore karna hai//
            {
                query = query.Where(c => c.Id != excludeId.Value); // current Category ki Id ko comparison se htado//
            }
            return await query.AnyAsync(); // developer ka final question kya filtered query me kam se kam ek Category mili agar mili to true nhi to false//
        }

    }
}
