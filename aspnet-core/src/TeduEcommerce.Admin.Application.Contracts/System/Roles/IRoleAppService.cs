using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.PermissionManagement;

namespace TeduEcommerce.Admin.System.Roles
{
    public interface IRoleAppService : ICrudAppService<RoleDto, Guid, PagedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>
    {
        Task<List<RoleInListDto>> GetListAllAsync();

        Task DeleteMulti(IEnumerable<Guid> ids);

        Task<PagedResultDto<RoleInListDto>> GetListFilterAsync(BaseListFilterDto input);

        Task<GetPermissionListResultDto> GetPermissionAsync(string providerName, string providerKey);

        Task UpdatePermissionsAsync(string providerName, string providerKey, UpdatePermissionsDto input);
    }
}
