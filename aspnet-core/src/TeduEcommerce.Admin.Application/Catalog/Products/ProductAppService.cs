using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeduEcommerce.Admin.Catalog.Products.Attributes;
using TeduEcommerce.ProductAttributes;
using TeduEcommerce.ProductCategories;
using TeduEcommerce.Products;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Admin.Catalog.Products
{
    [Authorize]
    public class ProductAppService : CrudAppService<Product, ProductDto, Guid, PagedResultRequestDto, CreateUpdateProductDto, CreateUpdateProductDto>, IProductAppService
    {
        private readonly ProductManager _productManager;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer;
        private readonly ProductCodeGenerator _productCodeGenerator;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeDateTime> _productAttributeDateTimeRepository;
        private readonly IRepository<ProductAttributeInt> _productAttributeIntRepository;
        private readonly IRepository<ProductAttributeDecimal> _productAttributeDecimalRepository;
        private readonly IRepository<ProductAttributeVarchar> _productAttributeVarcharRepository;
        private readonly IRepository<ProductAttributeText> _productAttributeTextRepository;

        public ProductAppService(IRepository<Product, Guid> repository,
                                ProductManager productManager,
                                IRepository<ProductCategory, Guid> productCategoryRepository,
                                IBlobContainer<ProductThumbnailPictureContainer> fileContainer,
                                ProductCodeGenerator productCodeGenerator,
                                IRepository<ProductAttribute> productAttributeRepository,
                                IRepository<ProductAttributeDateTime> productAttributeDateTimeRepository,
                                IRepository<ProductAttributeInt> productAttributeIntRepository,
                                IRepository<ProductAttributeDecimal> productAttributeDecimalRepository,
                                IRepository<ProductAttributeVarchar> productAttributeVarcharRepository,
                                IRepository<ProductAttributeText> productAttributeTextRepository) : base(repository)
        {
            _productManager = productManager;
            _productCategoryRepository = productCategoryRepository;
            _fileContainer = fileContainer;
            _productCodeGenerator = productCodeGenerator;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
            _productAttributeIntRepository = productAttributeIntRepository;
            _productAttributeIntRepository = productAttributeIntRepository;
            _productAttributeDecimalRepository = productAttributeDecimalRepository;
            _productAttributeVarcharRepository = productAttributeVarcharRepository;
            _productAttributeTextRepository = productAttributeTextRepository;
        }

        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            var product = await _productManager.CreateAsync(input.ManufacturerId,
                                                            input.Name,
                                                            input.Code,
                                                            input.Slug,
                                                            input.ProductType,
                                                            input.SKU,
                                                            input.SortOrder,
                                                            input.Visibility,
                                                            input.IsActive,
                                                            input.CategoryId,
                                                            input.SeoMetaDescription,
                                                            input.Description,
                                                            input.SellPrice);

            if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
                product.ThumbnailPicture = input.ThumbnailPictureName;
            }

            var result = await Repository.InsertAsync(product);

            return ObjectMapper.Map<Product, ProductDto>(result);
        }

        public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            var product = await Repository.GetAsync(id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductIsNotExists);

            product.ManufacturerId = input.ManufacturerId;
            product.Name = input.Name;
            product.Code = input.Code;
            product.Slug = input.Slug;
            product.ProductType = input.ProductType;
            product.SKU = input.SKU;
            product.SortOrder = input.SortOrder;
            product.Visibility = input.Visibility;
            product.IsActive = input.IsActive;

            if (product.CategoryId != input.CategoryId)
            {
                product.CategoryId = input.CategoryId;
                var category = await _productCategoryRepository.GetAsync(i => i.Id == input.CategoryId);
                product.CategoryName = category?.Name;
                product.CategorySlug = category?.Slug;
            }
            product.SeoMetaDescription = input.Description;
            if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
                product.ThumbnailPicture = input.ThumbnailPictureName;
            }
            product.SellPrice = input.SellPrice;

            await Repository.UpdateAsync(product);

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public async Task DeleteMutipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<ProductInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);
        }

        public async Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();

            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), i => i.Name.Trim().ToLower().Contains(input.Keyword.Trim().ToLower()));

            query = query.WhereIf(input.CategoryId.HasValue, i => i.CategoryId == input.CategoryId);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount).OrderBy(i => i.Code));
            var result = ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);

            return new PagedResultDto<ProductInListDto>(totalCount, result);
        }

        public async Task SaveThumbnailImageAsync(string fileName, string base64)
        {
            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            base64 = regex.Replace(base64, string.Empty);
            byte[] bytes = Convert.FromBase64String(base64);
            await _fileContainer.SaveAsync(fileName, bytes, overrideExisting: true);
        }

        public async Task<string> GetThumbnailImageAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            var thumbnailContent = await _fileContainer.GetAllBytesOrNullAsync(fileName);

            if (thumbnailContent == null) return null;

            var result = Convert.ToBase64String(thumbnailContent);
            return result;
        }

        public async Task<string> GenerateSuggestNewCodeAsync()
        {
            return await _productCodeGenerator.GenerateAsync();
        }

        public async Task<ProductAttributeValueDto> AddProductAttributeAsync(AddUpdateProductAttributeDto input)
        {
            var product = await Repository.GetAsync(input.ProductId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductIsNotExists);

            var attribute = await _productAttributeRepository.GetAsync(i => i.Id == input.AttributeId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

            var newAttributeId = Guid.NewGuid();
            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    if (input.DateTimeValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productDateTime = new ProductAttributeDateTime(newAttributeId, input.AttributeId, input.ProductId, input.DateTimeValue);
                    await _productAttributeDateTimeRepository.InsertAsync(productDateTime);
                    break;
                case AttributeType.Int:
                    if (input.IntValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productInt = new ProductAttributeInt(newAttributeId, input.AttributeId, input.ProductId, input.IntValue);
                    await _productAttributeIntRepository.InsertAsync(productInt);
                    break;
                case AttributeType.Decimal:
                    if (input.DecimalValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productDecimal = new ProductAttributeDecimal(newAttributeId, input.AttributeId, input.ProductId, input.DecimalValue);
                    await _productAttributeDecimalRepository.InsertAsync(productDecimal);
                    break;
                case AttributeType.Varchar:
                    if (input.VarcharValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productVarchar = new ProductAttributeVarchar(newAttributeId, input.AttributeId, input.ProductId, input.VarcharValue);
                    await _productAttributeVarcharRepository.InsertAsync(productVarchar);
                    break;
                case AttributeType.Text:
                    if (input.TextValue == null)
                    {
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productText = new ProductAttributeText(newAttributeId, input.AttributeId, input.ProductId, input.VarcharValue);
                    await _productAttributeTextRepository.InsertAsync(productText);
                    break;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new ProductAttributeValueDto()
            {
                AttributeId = input.AttributeId,
                Code = attribute.Code,
                DataType = attribute.DataType,
                DateTimeValue = input.DateTimeValue,
                DecimalValue = input.DecimalValue,
                Id = newAttributeId,
                IntValue = input.IntValue,
                Label = attribute.Label,
                ProductId = input.ProductId,
                TextValue = input.TextValue
            };
        }

        public async Task<ProductAttributeValueDto> UpdateProductAttributeAsync(Guid id, AddUpdateProductAttributeDto input)
        {
            var product = await Repository.GetAsync(id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductIsNotExists);

            var attribute = await _productAttributeRepository.GetAsync(i => i.Id == input.AttributeId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    if (input.DateTimeValue == null)
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    productAttributeDateTime.Value = input.DateTimeValue;
                    await _productAttributeDateTimeRepository.UpdateAsync(productAttributeDateTime);
                    break;

                case AttributeType.Int:
                    if (input.IntValue == null)
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeInt = await _productAttributeIntRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    productAttributeInt.Value = input.IntValue;
                    await _productAttributeIntRepository.UpdateAsync(productAttributeInt);
                    break;

                case AttributeType.Decimal:
                    if (input.DecimalValue == null)
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    productAttributeDecimal.Value = input.DecimalValue;
                    await _productAttributeDecimalRepository.UpdateAsync(productAttributeDecimal);
                    break;

                case AttributeType.Text:
                    if (input.TextValue == null)
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeText = await _productAttributeTextRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    productAttributeText.Value = input.TextValue;
                    await _productAttributeTextRepository.UpdateAsync(productAttributeText);
                    break;

                case AttributeType.Varchar:
                    if (input.VarcharValue == null)
                        throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    productAttributeVarchar.Value = input.VarcharValue;
                    await _productAttributeVarcharRepository.UpdateAsync(productAttributeVarchar);
                    break;
            }

            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new ProductAttributeValueDto()
            {
                AttributeId = input.AttributeId,
                Code = attribute.Code,
                DataType = attribute.DataType,
                DateTimeValue = input.DateTimeValue,
                DecimalValue = input.DecimalValue,
                Id = id,
                IntValue = input.IntValue,
                Label = attribute.Label,
                ProductId = input.ProductId,
                TextValue = input.TextValue
            };
        }

        public async Task RemoveProductAttributeAsync(Guid attributeId, Guid id)
        {
            var attribute = await _productAttributeRepository.GetAsync(i => i.Id == attributeId) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

            var newAttributeId = Guid.NewGuid();
            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _productAttributeDateTimeRepository.DeleteAsync(productAttributeDateTime);
                    break;
                case AttributeType.Int:
                    var productAttributeInt = await _productAttributeIntRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _productAttributeIntRepository.DeleteAsync(productAttributeInt);
                    break;
                case AttributeType.Decimal:
                    var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _productAttributeDecimalRepository.DeleteAsync(productAttributeDecimal);
                    break;
                case AttributeType.Varchar:
                    var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _productAttributeVarcharRepository.DeleteAsync(productAttributeVarchar);
                    break;
                case AttributeType.Text:
                    var productAttributeText = await _productAttributeTextRepository.GetAsync(i => i.Id == id) ?? throw new BusinessException(TeduEcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
                    await _productAttributeTextRepository.DeleteAsync(productAttributeText);
                    break;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

            var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
            var attrubuteIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
            var attrubuteDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
            var attrubuteTextQuery = await _productAttributeTextRepository.GetQueryableAsync();
            var attrubuteVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()
                        join adecimal in attrubuteDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()
                        join aint in attrubuteIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()
                        join atext in attrubuteTextQuery on a.Id equals atext.AttributeId into aTextTable
                        from atext in aTextTable.DefaultIfEmpty()
                        join avarchar in attrubuteVarcharQuery on a.Id equals avarchar.AttributeId into aVarcharTable
                        from avarchar in aVarcharTable.DefaultIfEmpty()
                        where (adate == null || adate.ProductId == productId)
                        && (adecimal == null || adecimal.ProductId == productId)
                        && (aint == null || aint.ProductId == productId)
                        && (atext == null || atext.ProductId == productId)
                        && (avarchar == null || avarchar.ProductId == productId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = productId,
                            DateTimeValue = adate.Value,
                            IntValue = aint.Value,
                            DecimalValue = adecimal.Value,
                            TextValue = atext.Value,
                            VarcharValue = avarchar.Value,
                            DecimalId = adecimal.Id,
                            IntId = aint.Id,
                            DateTimeId = adate.Id,
                            TextId = atext.Id,
                            VarcharId = avarchar.Id,
                        };

            query = query.Where(i => i.DateTimeId != null || i.DecimalId != null || i.IntId != null || i.TextId != null || i.VarcharId != null);

            return await AsyncExecuter.ToListAsync(query);
        }

        public async Task<PagedResultDto<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

            var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTabke
                        from adate in aDateTimeTabke.DefaultIfEmpty()
                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()
                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()
                        join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                        from aVarchar in aVarcharTable.DefaultIfEmpty()
                        join aText in attributeVarcharQuery on a.Id equals aText.AttributeId into aTextTable
                        from aText in aTextTable.DefaultIfEmpty()
                        where (adate != null || adate.ProductId == input.ProductId)
                        && (adecimal != null || adecimal.ProductId == input.ProductId)
                        && (aint != null || aint.ProductId == input.ProductId)
                        && (aVarchar != null || aVarchar.ProductId == input.ProductId)
                        && (aText != null || aText.ProductId == input.ProductId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = input.ProductId,
                            DateTimeValue = adate.Value,
                            DecimalValue = adecimal.Value,
                            IntValue = aint.Value,
                            TextValue = aText.Value,
                            VarcharValue = aVarchar.Value,
                            DecimalId = adecimal.Id,
                            IntId = aint.Id,
                            TextId = aText.Id,
                            VarcharId = aVarchar.Id,
                        };
            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(x => x.Label)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                );
            return new PagedResultDto<ProductAttributeValueDto>(totalCount, data);
        }


    }
}
