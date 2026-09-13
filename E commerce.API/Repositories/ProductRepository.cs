using E_commerce.API.Data;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace E_commerce.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Product> Products,int TotalItems)> GetProductsAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy, string? sortOrder, int pageNumber, int pageSize) // ye method 2 cheezein ek sath return karega 1. current page ka products  2. filter ke baad total products//
        {
            var query = _context.Products // mjhe product ki table se data chahiye //
                .Include(p => p.Category) // product ke sath uski category bhi leke Aao kyu product ki category bhi hai //
                .AsQueryable(); // abhi database se data na lao pahle Query tayaar karo kyuki usme kuch filter/serching/paging karni hai//

            if(! string.IsNullOrWhiteSpace(search)) // kya user ne search text diya either true or false//
            {
                query = query.Where(p => p.Name.Contains(search)); // yaha product naam wale search hoga//

            }
            if(categoryId.HasValue) // ky user ne category id di hai either true or false//
            {
                query = query.Where(p => p.CategoryId == categoryId.Value); // sirf categoryId vale product lao aagr vo categoryId(1/2/3/4/5..) ki actaul value wali hi category nikalegi//
            }
            // Minimum Price Filter
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            // Maximum Price Filter
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.ToLower() == "name")
                {
                    query = sortOrder?.ToLower() == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                }
                else if (sortBy.ToLower() == "price")

                {
                    query = sortOrder?.ToLower() == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                }

                else if (sortBy.ToLower() == "stock")
                {
                    query = sortOrder?.ToLower() == "desc" ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock);

                }
            }

            var totalItems = await query.CountAsync(); // jo filter maine abhi tak lagaya hai,us filter se match hone wale total Product ki ginti btao//
            var products = await query.Skip((pageNumber - 1)*pageSize).Take(pageSize).ToListAsync();// pageNumber=2,pageSize=10 hai to 2-1*10 means .skip(10) pahle 10 products skip karo uske baad 10 product lo. query tayar hai ab ise data base me execute karo aur  result ki list do aur return karo//

            return (products, totalItems); // yaha 2 value ek sath return horhi hai  page ki ttotal product aur filter ke product match ka count//
            
        
        // developer ki basic thinking ye hai jitna data user ne manga hai,database se utna hi relevent data nikalo.//
        //pehle search/category se filter karo,phir pagination se require page ke record lao//
        
        }


        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByCategoryIdAsync(int categoryId)
        {
            return await _context.Products.AnyAsync(p => p.CategoryId == categoryId); // chek karega is category se koi product juda hua hai ya nhi//


        }
    }
}