using System.Collections.Generic;
using TeduEcommerce.Public.Catalog.ProductCategories;
using TeduEcommerce.Public.Catalog.Products;

namespace TeduEcommerce.Public.Web.Models
{
    public class HomeCacheItem
    {
        public List<ProductCategoryInListDto> Categories { get; set; }
        public List<ProductInListDto> TopSellerProducts { get; set; }
    }
}
