using TeduEcommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace TeduEcommerce.Admin.Permissions;

public class TeduEcommerceAdminPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //CatalogGroup
        var catalogGroup = context.AddGroup(TeduEcommerceAdminPermissions.CatalogGroupName);

        //AddManufacturer
        var manufacturerPermission = catalogGroup.AddPermission(TeduEcommerceAdminPermissions.Manufacturer.Default, L("Permission:Catalog.Manufacturer"));
        manufacturerPermission.AddChild(TeduEcommerceAdminPermissions.Manufacturer.Create, L("Permission:Catalog.Manufacturer.Create"));
        manufacturerPermission.AddChild(TeduEcommerceAdminPermissions.Manufacturer.Update, L("Permission:Catalog.Manufacturer.Update"));
        manufacturerPermission.AddChild(TeduEcommerceAdminPermissions.Manufacturer.Delete, L("Permission:Catalog.Manufacturer.Delete"));

        //Product Category
        var productCategoryPermission = catalogGroup.AddPermission(TeduEcommerceAdminPermissions.ProductCategory.Default, L("Permission:Catalog.ProductCategory"));
        productCategoryPermission.AddChild(TeduEcommerceAdminPermissions.ProductCategory.Create, L("Permission:Catalog.ProductCategory.Create"));
        productCategoryPermission.AddChild(TeduEcommerceAdminPermissions.ProductCategory.Update, L("Permission:Catalog.ProductCategory.Update"));
        productCategoryPermission.AddChild(TeduEcommerceAdminPermissions.ProductCategory.Delete, L("Permission:Catalog.ProductCategory.Delete"));

        //AddProduct
        var productPermission = catalogGroup.AddPermission(TeduEcommerceAdminPermissions.Product.Default, L("Permission:Catalog.Product"));
        productPermission.AddChild(TeduEcommerceAdminPermissions.Product.Create, L("Permission:Catalog.Product.Create"));
        productPermission.AddChild(TeduEcommerceAdminPermissions.Product.Update, L("Permission:Catalog.Product.Update"));
        productPermission.AddChild(TeduEcommerceAdminPermissions.Product.Delete, L("Permission:Catalog.Product.Delete"));
        productPermission.AddChild(TeduEcommerceAdminPermissions.Product.AttributeManage, L("Permission:Catalog.Product.AttributeManage"));

        //AddAttribute
        var attributePermission = catalogGroup.AddPermission(TeduEcommerceAdminPermissions.Attribute.Default, L("Permission:Catalog.Attribute"));
        attributePermission.AddChild(TeduEcommerceAdminPermissions.Attribute.Create, L("Permission:Catalog.Attribute.Create"));
        attributePermission.AddChild(TeduEcommerceAdminPermissions.Attribute.Update, L("Permission:Catalog.Attribute.Update"));
        attributePermission.AddChild(TeduEcommerceAdminPermissions.Attribute.Delete, L("Permission:Catalog.Attribute.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TeduEcommerceResource>(name);
    }
}
