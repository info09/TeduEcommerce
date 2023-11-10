using AutoMapper;
using TeduEcommerce.Manufacturers;
using TeduEcommerce.ProductAttributes;
using TeduEcommerce.ProductCategories;
using TeduEcommerce.Products;
using TeduEcommerce.Public.Catalog.Manufacturers;
using TeduEcommerce.Public.Catalog.ProductAttributes;
using TeduEcommerce.Public.Catalog.ProductCategories;
using TeduEcommerce.Public.Catalog.Products;

namespace TeduEcommerce.Public;

public class TeduEcommercePublicApplicationAutoMapperProfile : Profile
{
    public TeduEcommercePublicApplicationAutoMapperProfile()
    {
        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();

        //ProductCategory
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();

        //ProductAttribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();

        //Manufacturer
        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();
    }
}
