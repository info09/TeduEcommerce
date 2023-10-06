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
