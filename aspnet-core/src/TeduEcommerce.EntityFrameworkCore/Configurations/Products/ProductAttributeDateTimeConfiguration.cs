using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeduEcommerce.Products;

namespace TeduEcommerce.Configurations.Products
{
    internal class ProductAttributeDateTimeConfiguration : IEntityTypeConfiguration<ProductAttributeDateTime>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeDateTime> builder)
        {
            builder.ToTable(TeduEcommerceConsts.DbTablePrefix + "ProductAttributeDateTimes");
            builder.HasKey(x => x.Id);
        }
    }
}