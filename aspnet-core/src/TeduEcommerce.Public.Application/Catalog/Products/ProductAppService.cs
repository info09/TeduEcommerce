using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeduEcommerce.ProductAttributes;
using TeduEcommerce.Products;
using TeduEcommerce.Public.Catalog.Products.Attributes;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Public.Catalog.Products
{
    public class ProductAppService : ReadOnlyAppService<Product, ProductDto, Guid, PagedResultRequestDto>, IProductAppService
    {
        private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer;
        private readonly IRepository<ProductAttribute> _productAttribute;
        private readonly IRepository<ProductAttributeText> _productAttributeText;
        private readonly IRepository<ProductAttributeInt> _productAttributeInt;
        private readonly IRepository<ProductAttributeDecimal> _productAttributeDecimal;
        private readonly IRepository<ProductAttributeDateTime> _productAttributeDateTime;
        private readonly IRepository<ProductAttributeVarchar> _productAttributeVarchar;
        public ProductAppService(IReadOnlyRepository<Product, Guid> repository,
                                IBlobContainer<ProductThumbnailPictureContainer> fileContainer,
                                IRepository<ProductAttribute> productAttribute,
                                IRepository<ProductAttributeText> productAttributeText,
                                IRepository<ProductAttributeInt> productAttributeInt,
                                IRepository<ProductAttributeDecimal> productAttributeDecimal,
                                IRepository<ProductAttributeDateTime> productAttributeDateTime,
                                IRepository<ProductAttributeVarchar> productAttributeVarchar) : base(repository)
        {
            this._fileContainer = fileContainer;
            this._productAttribute = productAttribute;
            this._productAttributeInt = productAttributeInt;
            this._productAttributeDecimal = productAttributeDecimal;
            this._productAttributeDateTime = productAttributeDateTime;
            this._productAttributeText = productAttributeText;
            this._productAttributeVarchar = productAttributeVarchar;
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
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Name.ToLower().Contains(input.Keyword.ToLower()));
            query = query.WhereIf(input.CategoryId.HasValue, i => i.CategoryId == input.CategoryId.Value);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.OrderByDescending(i => i.CreationTime).Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductInListDto>(totalCount, ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data));
        }

        public async Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId)
        {
            var attributeQuery = await _productAttribute.GetQueryableAsync();
            var attributeDateTimeQuery = await _productAttributeDateTime.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeText.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeInt.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimal.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarchar.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()
                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()
                        join atext in attributeTextQuery on a.Id equals atext.AttributeId into aTextTable
                        from atext in aTextTable.DefaultIfEmpty()
                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()
                        join avarchar in attributeVarcharQuery on a.Id equals avarchar.AttributeId into aVarcharTable
                        from avarchar in aVarcharTable.DefaultIfEmpty()
                        where (adate == null || adate.ProductId == productId)
                        && (adecimal == null || adecimal.ProductId == productId)
                        && (atext == null || atext.ProductId == productId)
                        && (aint == null || aint.ProductId == productId)
                        && (avarchar == null || avarchar.ProductId == productId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = productId,
                            DateTimeValue = adate != null ? adate.Value : null,
                            DecimalValue = adecimal != null ? adecimal.Value : null,
                            IntValue = aint != null ? aint.Value : null,
                            TextValue = atext != null ? atext.Value : null,
                            VarcharValue = avarchar != null ? avarchar.Value : null,
                            DateTimeId = adate != null ? adate.Id : null,
                            DecimalId = adecimal != null ? adecimal.Id : null,
                            IntId = aint != null ? aint.Id : null,
                            TextId = atext != null ? atext.Id : null,
                            VarcharId = avarchar != null ? avarchar.Id : null,
                        };

            query = query.Where(i => i.DateTimeId != null || i.IntId != null || i.DecimalId != null || i.TextId != null || i.VarcharId != null);

            return await AsyncExecuter.ToListAsync(query);
        }

        public async Task<PagedResultDto<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input)
        {
            var attributeQuery = await _productAttribute.GetQueryableAsync();
            var attributeDateTimeQuery = await _productAttributeDateTime.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeText.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeInt.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimal.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarchar.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()
                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()
                        join atext in attributeTextQuery on a.Id equals atext.AttributeId into aTextTable
                        from atext in aTextTable.DefaultIfEmpty()
                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()
                        join avarchar in attributeVarcharQuery on a.Id equals avarchar.AttributeId into aVarcharTable
                        from avarchar in aVarcharTable.DefaultIfEmpty()
                        where (adate == null || adate.ProductId == input.ProductId)
                        && (adecimal == null || adecimal.ProductId == input.ProductId)
                        && (atext == null || atext.ProductId == input.ProductId)
                        && (aint == null || aint.ProductId == input.ProductId)
                        && (avarchar == null || avarchar.ProductId == input.ProductId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = input.ProductId,
                            DateTimeValue = adate != null ? adate.Value : null,
                            DecimalValue = adecimal != null ? adecimal.Value : null,
                            IntValue = aint != null ? aint.Value : null,
                            TextValue = atext != null ? atext.Value : null,
                            VarcharValue = avarchar != null ? avarchar.Value : null,
                            DateTimeId = adate != null ? adate.Id : null,
                            DecimalId = adecimal != null ? adecimal.Id : null,
                            IntId = aint != null ? aint.Id : null,
                            TextId = atext != null ? atext.Id : null,
                            VarcharId = avarchar != null ? avarchar.Id : null,
                        };

            query = query.Where(i => i.DateTimeId != null || i.IntId != null || i.DecimalId != null || i.TextId != null || i.VarcharId != null);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.OrderByDescending(i => i.Label).Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductAttributeValueDto>(totalCount, data);
        }

        public async Task<string> GetThumbnailImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var thumbnailContent = await _fileContainer.GetAllBytesOrNullAsync(fileName);

            if (thumbnailContent == null) return null;

            var result = Convert.ToBase64String(thumbnailContent);
            return result;
        }
    }
}
