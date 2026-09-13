using AutoMapper;
using E_commerce.API.DTOs;
using E_commerce.API.Models;

namespace E_commerce.API.Helpers.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        { 
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<Product, ProductDto>() // product entity ko productDto me convert karo//
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category!= null ? src.Category.Name : null)); // developer Automapper se bolraha hai mai tmhe special instruction derha hu iski normal tarike se map na karna mai btataunga vaise karna opt.MapFrom(..) matlab value yaha se uthao,src.Category.Name matlab product ke andar jo category object hai to uska name lo other wise null dto.CategoryName me daaldo (src.Category !=null matlab true hai,src.Category =null matlab False hai)//
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        
        }

    }
}
