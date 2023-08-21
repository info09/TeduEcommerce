using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TeduEcommerce.Admin.ProductAttributes
{
    public interface IProductAttributeAppService : ICrudAppService<ProductAttributeDto, Guid, PagedResultRequestDto, CreateUpdateProductAttributeDto, CreateUpdateProductAttributeDto>
    {
        Task<PagedResultDto<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input);
        Task<List<ProductAttributeInListDto>> GetListAllAsync();
        Task DeleteMulti(IEnumerable<Guid> ids);
    }
}
