using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TeduEcommerce.Public.Catalog.ProductCategories
{
    public interface IProductCategoryAppService : IReadOnlyAppService<ProductCategoryDto, Guid, PagedResultRequestDto>
    {
        Task<List<ProductCategoryInListDto>> GetListAllAsync();
        Task<PagedResult<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input);
        Task<ProductCategoryDto> GetByCodeAsync(string code);
        Task<ProductCategoryDto> GetBySlugAsync(string slug);
    }
}
