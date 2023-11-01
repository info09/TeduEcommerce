import type { CreateUpdateRoleDto, RoleDto, RoleInListDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto, PagedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { BaseListFilterDto } from '../../models';
import type { GetPermissionListResultDto, UpdatePermissionsDto } from '../../volo/abp/permission-management/models';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  apiName = 'Default';
  

  create = (input: CreateUpdateRoleDto) =>
    this.restService.request<any, RoleDto>({
      method: 'POST',
      url: '/api/app/role',
      body: input,
    },
    { apiName: this.apiName });
  

  delete = (id: string) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/role/${id}`,
    },
    { apiName: this.apiName });
  

  deleteMultiByIds = (ids: string[]) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/role/multi',
      params: { ids },
    },
    { apiName: this.apiName });
  

  get = (id: string) =>
    this.restService.request<any, RoleDto>({
      method: 'GET',
      url: `/api/app/role/${id}`,
    },
    { apiName: this.apiName });
  

  getList = (input: PagedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<RoleDto>>({
      method: 'GET',
      url: '/api/app/role',
      params: { maxResultCount: input.maxResultCount, skipCount: input.skipCount },
    },
    { apiName: this.apiName });
  

  getListAll = () =>
    this.restService.request<any, RoleInListDto[]>({
      method: 'GET',
      url: '/api/app/role/all',
    },
    { apiName: this.apiName });
  

  getListFilter = (input: BaseListFilterDto) =>
    this.restService.request<any, PagedResultDto<RoleInListDto>>({
      method: 'GET',
      url: '/api/app/role/filter',
      params: { keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName });
  

  getPermission = (providerName: string, providerKey: string) =>
    this.restService.request<any, GetPermissionListResultDto>({
      method: 'GET',
      url: '/api/app/role/permission',
      params: { providerName, providerKey },
    },
    { apiName: this.apiName });
  

  update = (id: string, input: CreateUpdateRoleDto) =>
    this.restService.request<any, RoleDto>({
      method: 'PUT',
      url: `/api/app/role/${id}`,
      body: input,
    },
    { apiName: this.apiName });
  

  updatePermissions = (providerName: string, providerKey: string, input: UpdatePermissionsDto) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/role/permissions',
      params: { providerName, providerKey },
      body: input,
    },
    { apiName: this.apiName });

  constructor(private restService: RestService) {}
}
