using AutoMapper;
using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using E_commerce.API.Models;
namespace E_commerce.API.Services
{
    public class ProductService : IProductService   // ye class IProductService interface ke sare methods implement karegi//
    {
        private readonly IProductRepository _repository; // jab bhi Product ka data datebase se lena ya save karna hoga,main _repository ko bolunga//
        private readonly ICategoryRepository _categoryRepository;// product ka data linga to uski cateory bhi check hogi//
        private readonly IMapper _mapper;                          // baar baar manual mapping nahi likhni hogi isliye Automapper use kiya//
        public ProductService(IProductRepository repository, ICategoryRepository categoryRepository, IMapper mapper) 
        {
            _repository = repository;                            // Dependency Injection ke through Repository/CategoryRepository/Mapper bnake ke Product service ko dedeta hai//
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortBy, string? sortOrder, int pageNumber, int pageSize) // developer soch raha hai mjhe user ko Products ki list deni hai,lekin simple list nahi.user ne search/category/page ki jo request ki hai uske according data dena hai,aur sath me pegination ki poori information bhi deni hai//
        {
            var result = await _repository.GetProductsAsync( search,  categoryId, minPrice ,maxPrice, sortBy, sortOrder , pageNumber,  pageSize); //mjhe database se products chahiye isliye repository ko bolo//
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(result.Products); // mjhe result milgaya ab is result ke jitne product hai usko dto me convert kardo//
            var totalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize); // Totalitem=32,pageSize=10 ,32/10 = 3.2 ..lekin 3.2 pages nhi hosakte isliye Math.Ceiling() se decimal number ko uper wale whole no. pe lejata hai//
            // return _mapper.Map<IEnumerable<ProductDto>>(products);// ye Product ki list uthao aur ProductDto ki list bna do aur return to controller //
            return new PagedResult<ProductDto>
            {
                Items = productDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = result.TotalItems,
                TotalPages = totalPages
            };
            // 1.Repository se Product entity layi. 2.Automapper se product endtity ko ProductDto me convert kiya. 3.Contrller ko productDto wapas bhejdiya//
       
        }
        public async Task<ProductDto?> GetByIdAsync(int id) // task ek Async method hai. ProductDto ek product return hoga. ? agar nhi mila to null return hosakta hai//
        {
            var product =await _repository.GetByIdAsync(id); //repository se bolo mjhe database se Id ke hisab se product chahiye fir usne query chalai SELECT * FROM Products WHERE ID= 1/2/3//
            if (product == null)   // agar data base me Product nhi mila aage ka code chalane ka koi fayada nhi//
                throw new KeyNotFoundException("Product not found.");
            return _mapper.Map<ProductDto>(product); // product milgaya lekin controller ko entity nahi bhejni use DTO bhejana hai Automapper se bolo product ki entity DTO me copy kare//
        }

        public async Task<ProductDto> AddAsync(CreateProductDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId); // user ne CategoryId bheji hai pahle check karo ki catagory database me hai ki nhi//
            if (category == null)

                throw new KeyNotFoundException("Category not found.");
            var product = _mapper.Map<Product>(dto); // category milne per DTO ko Product entity me convert karna hai... Map<Destination>(source)//
            await _repository.AddAsync(product); // ab service repository ko bolti hai ye product database me save kardo//
            return _mapper.Map<ProductDto>(product); // product Entity ko dobara ProductDto me convert karo kyuki controller aur client ko enity nhi bhejni//
                }
        public async Task<ProductDto> UpdateAsync(int id , UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id); // repository se bolo pahle purana product nikalo id match karke//
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if(category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            _mapper.Map(dto, product); // naya product na bnao jo product object pahle se bna hua hai, usi ke andar DTO ki value bhar do...Map(Source, Destination)//
            await _repository.UpdateAsync(product); // repository ko bolo is product ko data base nme add kardo
            return _mapper.Map<ProductDto>(product);
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not Found.");
            await _repository.DeleteAsync(product);
        }
    }
}
