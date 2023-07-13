using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TeduEcommerce.ProductCategories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Admin.ProductCategories
{
    public class ProductCategoriesAppService : CrudAppService<ProductCategory, ProductCategoryDto, Guid, PagedResultRequestDto, CreateUpdateProductCategoryDto, CreateUpdateProductCategoryDto>, IProductCategoriesAppService
    {
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository) : base(repository)
        {
            _productCategoryRepository = repository;
        }

        public async Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await _productCategoryRepository.GetQueryableAsync();

            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), i => i.Name.Trim().ToLower().Contains(input.Keyword.Trim().ToLower()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            var items = ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);

            return new PagedResultDto<ProductCategoryInListDto>(totalCount, items);
        }
    }
}
