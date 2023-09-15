import type { AddUpdateProductAttributeDto, ProductAttributeListFilterDto, ProductAttributeValueDto } from './attributes/models';
import type { CreateUpdateProductDto, ProductDto, ProductInListDto, ProductListFilterDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto, PagedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  apiName = 'Default';
  

  addProductAttribute = (input: AddUpdateProductAttributeDto) =>
    this.restService.request<any, ProductAttributeValueDto>({
      method: 'POST',
      url: '/api/app/product/product-attribute',
      body: input,
    },
    { apiName: this.apiName });
  

  create = (input: CreateUpdateProductDto) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: '/api/app/product',
      body: input,
    },
    { apiName: this.apiName });
  

  delete = (id: string) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/product/${id}`,
    },
    { apiName: this.apiName });
  

  deleteMutiple = (ids: string[]) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/product/mutiple',
      params: { ids },
    },
    { apiName: this.apiName });
  

  generateSuggestNewCode = () =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/product/generate-suggest-new-code',
    },
    { apiName: this.apiName });
  

  get = (id: string) =>
    this.restService.request<any, ProductDto>({
      method: 'GET',
      url: `/api/app/product/${id}`,
    },
    { apiName: this.apiName });
  

  getList = (input: PagedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<ProductDto>>({
      method: 'GET',
      url: '/api/app/product',
      params: { maxResultCount: input.maxResultCount, skipCount: input.skipCount },
    },
    { apiName: this.apiName });
  

  getListAll = () =>
    this.restService.request<any, ProductInListDto[]>({
      method: 'GET',
      url: '/api/app/product/all',
    },
    { apiName: this.apiName });
  

  getListFilter = (input: ProductListFilterDto) =>
    this.restService.request<any, PagedResultDto<ProductInListDto>>({
      method: 'GET',
      url: '/api/app/product/filter',
      params: { categoryId: input.categoryId, keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName });
  

  getListProductAttributeAll = (productId: string) =>
    this.restService.request<any, ProductAttributeValueDto[]>({
      method: 'GET',
      url: `/api/app/product/product-attribute-all/${productId}`,
    },
    { apiName: this.apiName });
  

  getListProductAttributes = (input: ProductAttributeListFilterDto) =>
    this.restService.request<any, PagedResultDto<ProductAttributeValueDto>>({
      method: 'GET',
      url: '/api/app/product/product-attributes',
      params: { productId: input.productId, keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName });
  

  getThumbnailImage = (fileName: string) =>
    this.restService.request<any, string>({
      method: 'GET',
      responseType: 'text',
      url: '/api/app/product/thumbnail-image',
      params: { fileName },
    },
    { apiName: this.apiName });
  

  removeProductAttribute = (attributeId: string, id: string) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/product/${id}/product-attribute/${attributeId}`,
    },
    { apiName: this.apiName });
  

  saveThumbnailImage = (fileName: string, base64: string) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/product/save-thumbnail-image',
      params: { fileName, base64 },
    },
    { apiName: this.apiName });
  

  update = (id: string, input: CreateUpdateProductDto) =>
    this.restService.request<any, ProductDto>({
      method: 'PUT',
      url: `/api/app/product/${id}`,
      body: input,
    },
    { apiName: this.apiName });
  

  updateProductAttribute = (id: string, input: AddUpdateProductAttributeDto) =>
    this.restService.request<any, ProductAttributeValueDto>({
      method: 'PUT',
      url: `/api/app/product/${id}/product-attribute`,
      body: input,
    },
    { apiName: this.apiName });

  constructor(private restService: RestService) {}
}
