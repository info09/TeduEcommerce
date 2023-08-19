﻿using System;
using System.Threading.Tasks;
using TeduEcommerce.ProductCategories;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace TeduEcommerce.Products
{
    public class ProductManager : DomainService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;

        public ProductManager(IRepository<Product, Guid> productRepository, IRepository<ProductCategory, Guid> productCategoryRepository)
        {
            this._productRepository = productRepository;
            this._productCategoryRepository = productCategoryRepository;
        }

        public async Task<Product> CreateAsync(Guid manufacturerId, string name, string code, string slug, ProductType productType, string sKU, int sortOrder, bool visibility, bool isActive, Guid categoryId, string seoMetaDescription, string description, double sellPrice)
        {
            if (await _productRepository.AnyAsync(i => i.Name == name))
                throw new UserFriendlyException("Tên sản phẩm đã tồn tại", TeduEcommerceDomainErrorCodes.ProductNameAlreadyExists);
            
            if (await _productRepository.AnyAsync(i => i.Code == code))
                throw new UserFriendlyException("Mã sản phẩm đã tồn tại", TeduEcommerceDomainErrorCodes.ProductCodeAlreadyExists);
            
            if (await _productRepository.AnyAsync(i => i.SKU == sKU))
                throw new UserFriendlyException("Mã SKU sản phẩm đã tồn tại", TeduEcommerceDomainErrorCodes.ProductSKUAlreadyExists);

            var category = await _productCategoryRepository.GetAsync(categoryId);

            return new Product(Guid.NewGuid(), manufacturerId, name, code, slug, productType, sKU, sortOrder, visibility, isActive, categoryId, seoMetaDescription, description, null, sellPrice, category?.Name, category?.Slug);
        }
    }
}
