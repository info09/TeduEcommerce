using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeduEcommerce.Admin.Permissions;
using TeduEcommerce.ProductAttributes;
using TeduEcommerce.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Admin.Catalog.ProductAttributes
{
    [Authorize(TeduEcommerceAdminPermissions.Attribute.Default, Policy = "AdminOnly")]
    public class ProductAttributeAppService : CrudAppService<ProductAttribute, ProductAttributeDto, Guid, PagedResultRequestDto, CreateUpdateProductAttributeDto, CreateUpdateProductAttributeDto>, IProductAttributeAppService
    {
        private readonly ProductAttributeCodeGenerator _productAttributeCodeGenerator;
        public ProductAttributeAppService(IRepository<ProductAttribute, Guid> repository,
                                            ProductAttributeCodeGenerator productAttributeCodeGenerator) : base(repository)
        {
            _productAttributeCodeGenerator = productAttributeCodeGenerator;

            GetPolicyName = TeduEcommerceAdminPermissions.Attribute.Default;
            GetListPolicyName = TeduEcommerceAdminPermissions.Attribute.Default;
            CreatePolicyName = TeduEcommerceAdminPermissions.Attribute.Create;
            UpdatePolicyName = TeduEcommerceAdminPermissions.Attribute.Update;
            DeletePolicyName = TeduEcommerceAdminPermissions.Attribute.Delete;
        }

        [Authorize(TeduEcommerceAdminPermissions.Attribute.Delete)]
        public async Task DeleteMulti(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        [Authorize(TeduEcommerceAdminPermissions.Attribute.Default)]
        public async Task<List<ProductAttributeInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data);
        }

        [Authorize(TeduEcommerceAdminPermissions.Attribute.Default)]
        public async Task<PagedResultDto<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Label.ToLower().Contains(input.Keyword.ToLower().Trim()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductAttributeInListDto>(totalCount, ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data));
        }

        public async Task<string> GenerateSuggestNewCodeAttributeAsync()
        {
            return await _productAttributeCodeGenerator.GenerateAsync();
        }
    }
}
