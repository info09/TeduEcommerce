using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeduEcommerce.Roles;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SimpleStateChecking;

namespace TeduEcommerce.Admin.Roles
{
    public class RoleAppService : CrudAppService<IdentityRole, RoleDto, Guid, PagedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>, IRoleAppService
    {
        protected PermissionManagementOptions Options { get; }
        protected IPermissionManager PermissionManager { get; }
        protected IPermissionDefinitionManager PermissionDefinitionManager { get; }
        protected ISimpleStateCheckerManager<PermissionDefinition> SimpleStateCheckerManager { get; }

        public RoleAppService(IRepository<IdentityRole, Guid> repository, IPermissionManager permissionManager,
        IPermissionDefinitionManager permissionDefinitionManager,
        IOptions<PermissionManagementOptions> options,
        ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager) : base(repository)
        {
            Options = options.Value;
            PermissionManager = permissionManager;
            PermissionDefinitionManager = permissionDefinitionManager;
            SimpleStateCheckerManager = simpleStateCheckerManager;
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

        public async Task<GetPermissionListResultDto> GetPermissionAsync(string providerName, string providerKey)
        {
            var result = new GetPermissionListResultDto()
            {
                EntityDisplayName = providerName,
                Groups = new List<PermissionGroupDto>()
            };

            foreach (var group in PermissionDefinitionManager.GetGroups().Where(i => i.Name.StartsWith("AbpIdentity") || i.Name.StartsWith("TeduEcomAdmin")))
            {
                PermissionGroupDto groupDto = CreatePermissionGroupDto(group);

                var neededCheckPermission = new List<PermissionDefinition>();

                foreach (var permission in group.GetPermissionsWithChildren().Where(i => i.IsEnabled)
                                                                            .Where(i => !i.Providers.Any() || i.Providers.Contains(providerName)))
                {
                    if (await SimpleStateCheckerManager.IsEnabledAsync(permission))
                    {
                        neededCheckPermission.Add(permission);
                    }
                }

                if (!neededCheckPermission.Any()) continue;

                var grantInfoDtos = neededCheckPermission.Select(CreatePermissionGrantInfoDto).ToList();

                var multipleGrantInfo = await PermissionManager.GetAsync(neededCheckPermission.Select(i => i.Name).ToArray(), providerName, providerKey);

                foreach (var grantInfo in multipleGrantInfo.Result)
                {
                    var grantInfoDto = grantInfoDtos.First(i => i.Name == grantInfo.Name);

                    grantInfoDto.IsGranted = grantInfo.IsGranted;

                    foreach (var provider in grantInfo.Providers)
                    {
                        grantInfoDto.GrantedProviders.Add(new ProviderInfoDto()
                        {
                            ProviderName = provider.Name,
                            ProviderKey = provider.Key,
                        });
                    }

                    groupDto.Permissions.Add(grantInfoDto);
                }

                if (groupDto.Permissions.Any())
                {
                    result.Groups.Add(groupDto);
                }
            }

            return result;

            
        }

        private PermissionGrantInfoDto CreatePermissionGrantInfoDto(PermissionDefinition permission)
        {
            return new PermissionGrantInfoDto()
            {
                Name = permission.Name,
                DisplayName = permission.DisplayName?.Localize(StringLocalizerFactory),
                ParentName = permission.Parent?.Name,
                AllowedProviders = permission.Providers,
                GrantedProviders = new List<ProviderInfoDto>()
            };
        }

        private PermissionGroupDto CreatePermissionGroupDto(PermissionGroupDefinition group)
        {
            return new PermissionGroupDto()
            {
                Name = group.Name,
                DisplayName = group.DisplayName?.Localize(StringLocalizerFactory),
                Permissions = new List<PermissionGrantInfoDto>()
            };
        }

        public virtual async Task UpdatePermissionsAsync(string providerName, string providerKey, UpdatePermissionsDto input)
        {
            // await CheckProviderPolicy(providerName);

            foreach (var permissionDto in input.Permissions)
            {
                await PermissionManager.SetAsync(permissionDto.Name, providerName, providerKey, permissionDto.IsGranted);
            }
        }

        protected virtual async Task CheckProviderPolicy(string providerName)
        {
            var policyName = Options.ProviderPolicies.GetOrDefault(providerName);
            if (policyName.IsNullOrEmpty())
            {
                throw new AbpException($"No policy defined to get/set permissions for the provider '{providerName}'. Use {nameof(PermissionManagementOptions)} to map the policy.");
            }

            await AuthorizationService.CheckAsync(policyName);
        }
    }
}
