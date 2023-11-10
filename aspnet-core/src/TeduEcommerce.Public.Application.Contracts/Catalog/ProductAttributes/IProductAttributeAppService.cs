using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TeduEcommerce.Public.Catalog.ProductAttributes
{
    public interface IProductAttributeAppService : IReadOnlyAppService<ProductAttributeDto, Guid, PagedResultRequestDto>
    {
        Task<List<ProductAttributeInListDto>> GetListAllAsync();
        Task<PagedResult<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input);
    }
}
