
using E_commerce.API.DTOs;
using E_commerce.API.Helpers;
using E_commerce.API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace E_commerce.API.Controllers  // controller ka kaam hai client/swagger se request lena ,service ko dena, fir service ka result client ko dena//
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // is controller ki Api ko keval Authenticate user acces karsakta hai//
    public class ProductController : ControllerBase 
    {
        private readonly IProductService _productService; //yaha controller kahe raha mjhe product ka kaam karne ke liye product ki service chahiye//
        public ProductController(IProductService productService) //.NET Dependency Injection ke through IProductService ka object Productcontroller ko milta hai//
        {
            _productService = productService;
        }
        [Authorize] // is controller ki Api ko keval Authenticate user acces karsakta hai//
        [HttpGet]
        public async Task<IActionResult> GetProducts(string? search, int? categoryId, decimal? minPrice,decimal? maxPrice, string? sortBy, string? sortOrder, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Page number must be greater than 0."));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Page size must be between 1 and 100."));
            }

            // Price Range Validation
            if (minPrice.HasValue &&
                maxPrice.HasValue &&
                minPrice > maxPrice)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Minimum price cannot be greater than maximum price."));
            }

            // SortBy Validation
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var allowedSortFields = new[] // yaha client ki allowed field ki list bna di//
                {
            "name",
            "price",
            "stock"
        };

                if (!allowedSortFields.Contains(sortBy.ToLower())) // client ne jo sortBy bheja hai, kya vo meri allowed list me hai?//
                {
                    return BadRequest(new ApiResponse<object>(
                        false,
                        "Invalid sort field. Use name, price or stock."));
                }
            }

            // SortOrder Validation
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                var allowedSortOrders = new[]
                {
            "asc",
            "desc"
        };

                if (!allowedSortOrders.Contains(sortOrder.ToLower())) // client ne jo sortOrder bheja hai kya vo asc ya desc me se hi bheja hai//
                {
                    return BadRequest(new ApiResponse<object>(
                        false,
                        "Invalid sort order. Use asc or desc."));
                }
            }
            var result = await _productService.GetAllAsync(search, categoryId, minPrice, maxPrice, sortBy, sortOrder, pageNumber, pageSize); //execution product service me chalagaya//
            return Ok(new ApiResponse<PagedResult<ProductDto>>(true,"product retrieved succesfully.",result));

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id) 
        {
            
                var result = await _productService.GetByIdAsync(id);

              //  if (result == null)
              //  {
                //    return NotFound(new ApiResponse<ProductDto>(false, "product not found."));
               // }
                return Ok(new ApiResponse<ProductDto>(true, "product retrieved successfully.",result));
            
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        
        public async Task<IActionResult> AddProduct(CreateProductDto dto)
        {
            
                var result = await _productService.AddAsync(dto);
                return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, new ApiResponse<ProductDto>(true, "product retrieved succesfully.", result));
            
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
        
        {
            if (id !=dto.Id)
            {
                return BadRequest(new ApiResponse<object>(false, "Id mismatch."));

            }
            await _productService.UpdateAsync(id, dto);
            return Ok(new ApiResponse<object>(true, "product upadted Successfully."));


        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new ApiResponse<object>(true, "product delete Succesfully."));

        }

    }
}
