using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TeduEcommerce.Public.Catalog.Manufacturers
{
    public interface IManufacturerAppService : IReadOnlyAppService<ManufacturerDto, Guid, PagedResultRequestDto>
    {
        Task<List<ManufacturerInListDto>> GetListAllAsync();
        Task<PagedResult<ManufacturerInListDto>> GetListFilterAsync(BaseListFilterDto input);
    }
}
