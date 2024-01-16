using TeduEcommerce.Public.Catalog.Products;

namespace TeduEcommerce.Public.Web.Models
{
    public class CartItem
    {
        public ProductDto Product { get; set; }
        public int Quantity { get; set; }
    }
}
