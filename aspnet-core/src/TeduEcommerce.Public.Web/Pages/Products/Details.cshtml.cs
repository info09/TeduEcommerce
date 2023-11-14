using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TeduEcommerce.Public.Catalog.ProductCategories;
using TeduEcommerce.Public.Catalog.Products;

namespace TeduEcommerce.Public.Web.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly IProductAppService _productAppService;
        private readonly IProductCategoryAppService _productCategoryAppService;

        public DetailsModel(IProductAppService productAppService, IProductCategoryAppService productCategoryAppService)
        {
            this._productAppService = productAppService;
            this._productCategoryAppService = productCategoryAppService;
        }

        public ProductDto Product { get; set; }
        public ProductCategoryDto Category { get; set; }

        public async Task OnGetAsync(string categorySlug, string slug)
        {
            Product = await _productAppService.GetBySlugAsync(slug);
            Category = await _productCategoryAppService.GetBySlugAsync(categorySlug);
        }
    }
}
