using AutoMapper;
using TeduEcommerce.Admin.Manufacturers;
using TeduEcommerce.Admin.ProductCategories;
using TeduEcommerce.Admin.Products;
using TeduEcommerce.Manufacturers;
using TeduEcommerce.ProductCategories;
using TeduEcommerce.Products;

namespace TeduEcommerce.Admin;

public class TeduEcommerceAdminApplicationAutoMapperProfile : Profile
{
    public TeduEcommerceAdminApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        
        //ProductCategory
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();
        CreateMap<CreateUpdateProductCategoryDto, ProductCategory>();

        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();
        CreateMap<CreateUpdateProductDto, Product>();

        //Manufacturer
        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();
        CreateMap<CreateUpdateManufacturerDto, Manufacturer>();
    }
}
