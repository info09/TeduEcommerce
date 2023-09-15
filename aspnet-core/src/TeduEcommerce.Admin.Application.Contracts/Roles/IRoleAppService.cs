using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace TeduEcommerce.Admin.Roles
{
    public interface IRoleAppService : ICrudAppService<RoleDto, Guid, PagedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>
    {
        Task<List<RoleInListDto>> GetListAllAsync();

        Task DeleteMulti(IEnumerable<Guid> ids);

        Task<PagedResultDto<RoleInListDto>> GetListFilterAsync(BaseListFilterDto input);
    }
}
