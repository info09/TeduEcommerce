using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeduEcommerce.Roles;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace TeduEcommerce.Admin.Roles
{
    public class RoleAppService : CrudAppService<IdentityRole, RoleDto, Guid, PagedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>, IRoleAppService
    {
        public RoleAppService(IRepository<IdentityRole, Guid> repository) : base(repository)
        {
        }

        public async Task DeleteMulti(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<RoleInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<IdentityRole>, List<RoleInListDto>>(data);
        }

        public async Task<PagedResultDto<RoleInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Name.ToLower().Trim().Contains(input.Keyword.ToLower().Trim()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<RoleInListDto> { TotalCount = totalCount, Items = ObjectMapper.Map<List<IdentityRole>, List<RoleInListDto>>(data) };
        }

        public override async Task<RoleDto> CreateAsync(CreateUpdateRoleDto input)
        {
            var query = await Repository.GetQueryableAsync();
            var isNameExisted = query.Any(i => i.Name.ToLower().Trim() == input.Name.ToLower().Trim());
            if (isNameExisted)
                throw new BusinessException(TeduEcommerceDomainErrorCodes.RoleNameAlreadyExists).WithData("Name", input.Name);

            var role = new IdentityRole(GuidGenerator.Create(), input.Name);
            role.ExtraProperties[RoleConsts.DescriptionFieldName] = input.Description;
            var data = await Repository.InsertAsync(role);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<IdentityRole, RoleDto>(data);
        }

        public override async Task<RoleDto> UpdateAsync(Guid id, CreateUpdateRoleDto input)
        {
            var role = await Repository.GetAsync(id);
            if (role == null) throw new EntityNotFoundException(typeof(IdentityRole), id);

            var query = await Repository.GetQueryableAsync();
            var isNameExisted = query.Any(i => i.Name.ToLower().Trim() == input.Name.ToLower().Trim());
            if (isNameExisted && role.Name != input.Name) throw new BusinessException(TeduEcommerceDomainErrorCodes.RoleNameAlreadyExists);

            role.ExtraProperties[RoleConsts.DescriptionFieldName] = input.Description;
            var data = await Repository.UpdateAsync(role);

            await UnitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<IdentityRole, RoleDto>(data);
        }
    }
}
