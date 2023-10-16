using AutoMapper;
using TeduEcommerce.Admin.Catalog.Manufacturers;
using TeduEcommerce.Admin.Catalog.ProductAttributes;
using TeduEcommerce.Admin.Catalog.ProductCategories;
using TeduEcommerce.Admin.Catalog.Products;
using TeduEcommerce.Admin.System.Roles;
using TeduEcommerce.Admin.System.Users;
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

        //User
        CreateMap<IdentityUser, UserDto>();
        CreateMap<IdentityUser, UserInListDto>();
    }
}
