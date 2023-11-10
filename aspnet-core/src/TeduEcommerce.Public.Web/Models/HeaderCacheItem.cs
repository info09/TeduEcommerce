using System.Collections.Generic;
using TeduEcommerce.Public.Catalog.ProductCategories;

namespace TeduEcommerce.Public.Web.Models
{
    public class HeaderCacheItem
    {
        public List<ProductCategoryInListDto> Categories { get; set; }
    }
}
