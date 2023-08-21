﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TeduEcommerce.Admin.Products
{
    public interface IProductAppService : ICrudAppService<ProductDto, Guid, PagedResultRequestDto, CreateUpdateProductDto, CreateUpdateProductDto>
    {
        Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input);
        Task<List<ProductInListDto>> GetListAllAsync();
        Task DeleteMutipleAsync(IEnumerable<Guid> ids);

        Task<string> GetThumbnailImageAsync(string fileName);
        Task<string> GenerateSuggestNewCodeAsync();
    }
}
