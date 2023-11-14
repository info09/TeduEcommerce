using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeduEcommerce.ProductCategories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Public.Catalog.ProductCategories
{
    public class ProductCategoryAppService : ReadOnlyAppService<ProductCategory, ProductCategoryDto, Guid, PagedResultRequestDto>, IProductCategoryAppService
    {
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        public ProductCategoryAppService(IRepository<ProductCategory, Guid> repository) : base(repository)
        {
            this._productCategoryRepository = repository;
        }

        public async Task<ProductCategoryDto> GetByCodeAsync(string code)
        {
            var category = await _productCategoryRepository.GetAsync(i => i.Code == code);
            return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(category);
        }

        public async Task<ProductCategoryDto> GetBySlugAsync(string slug)
        {
            var productCategory = await _productCategoryRepository.GetAsync(i => i.Slug == slug);
            return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(productCategory);
        }

        public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);

            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        public async Task<PagedResult<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), i => i.Name.ToLower().Contains(input.Keyword.ToLower()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);

            var data = await AsyncExecuter.ToListAsync(query.OrderByDescending(i => i.CreationTime).Skip((input.CurrentPage) * input.PageSize).Take(input.PageSize));

            return new PagedResult<ProductCategoryInListDto>(ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data), totalCount, input.CurrentPage, input.PageSize);
        }
    }
}
