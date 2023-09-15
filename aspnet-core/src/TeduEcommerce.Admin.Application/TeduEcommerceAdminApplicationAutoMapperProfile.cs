using AutoMapper;
using TeduEcommerce.Admin.Manufacturers;
using TeduEcommerce.Admin.ProductAttributes;
using TeduEcommerce.Admin.ProductCategories;
using TeduEcommerce.Admin.Products;
using TeduEcommerce.Admin.Roles;
using TeduEcommerce.Manufacturers;
using TeduEcommerce.ProductAttributes;
using TeduEcommerce.ProductCategories;
using TeduEcommerce.Products;
using TeduEcommerce.Roles;
using Volo.Abp.Identity;

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

        //ProductAttribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();
        CreateMap<CreateUpdateProductAttributeDto, ProductAttribute>();

        //Role
        CreateMap<IdentityRole, RoleDto>().ForMember(i => i.Description, 
            map => map.MapFrom(i => i.ExtraProperties.ContainsKey(RoleConsts.DescriptionFieldName) ? 
                                    i.ExtraProperties[RoleConsts.DescriptionFieldName] : null));

        CreateMap<IdentityRole, RoleInListDto>().ForMember(i => i.Description,
            map => map.MapFrom(i => i.ExtraProperties.ContainsKey(RoleConsts.DescriptionFieldName) ?
                                    i.ExtraProperties[RoleConsts.DescriptionFieldName] : null));

        CreateMap<CreateUpdateRoleDto, IdentityRole>();
    }
}
