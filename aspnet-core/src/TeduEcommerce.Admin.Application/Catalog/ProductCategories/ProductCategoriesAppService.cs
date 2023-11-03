using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TeduEcommerce.Admin.Permissions;
using TeduEcommerce.ProductCategories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Admin.Catalog.ProductCategories
{
    [Authorize(TeduEcommerceAdminPermissions.ProductCategory.Default, Policy = "AdminOnly")]
    public class ProductCategoriesAppService : CrudAppService<ProductCategory, ProductCategoryDto, Guid, PagedResultRequestDto, CreateUpdateProductCategoryDto, CreateUpdateProductCategoryDto>, IProductCategoriesAppService
    {
        public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository) : base(repository)
        {
            GetPolicyName = TeduEcommerceAdminPermissions.ProductCategory.Default;
            GetListPolicyName = TeduEcommerceAdminPermissions.ProductCategory.Default;
            CreatePolicyName = TeduEcommerceAdminPermissions.ProductCategory.Create;
            UpdatePolicyName = TeduEcommerceAdminPermissions.ProductCategory.Update;
            DeletePolicyName = TeduEcommerceAdminPermissions.ProductCategory.Delete;
        }

        [Authorize(TeduEcommerceAdminPermissions.ProductCategory.Delete)]
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        [Authorize(TeduEcommerceAdminPermissions.ProductCategory.Default)]
        public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        [Authorize(TeduEcommerceAdminPermissions.ProductCategory.Default)]
        public async Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();

            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), i => i.Name.Trim().ToLower().Contains(input.Keyword.Trim().ToLower()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            var items = ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);

            return new PagedResultDto<ProductCategoryInListDto>(totalCount, items);
        }
    }
}
