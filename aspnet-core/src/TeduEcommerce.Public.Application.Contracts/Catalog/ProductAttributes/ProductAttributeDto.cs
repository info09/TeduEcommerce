using System;
using Volo.Abp.Application.Dtos;

namespace TeduEcommerce.Public.Catalog.ProductAttributes
{
    public class ProductAttributeDto : IEntityDto<Guid>
    {
        public Guid Id { get; set; }
    }
}
