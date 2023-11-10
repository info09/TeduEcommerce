using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeduEcommerce.Public.Catalog.ProductCategories;
using TeduEcommerce.Public.Catalog.Products;

namespace TeduEcommerce.Public.Web.Pages.Products
{
    public class CategoryModel : PageModel
    {
        public ProductCategoryDto Category { get; set; }

        public List<ProductCategoryInListDto> Categories { get; set; }

        public PagedResult<ProductInListDto> ProductData { get; set; }

        private readonly IProductAppService _productAppService;
        private readonly IProductCategoryAppService _productCategoryAppService;

        public CategoryModel(IProductAppService productAppService, IProductCategoryAppService productCategoryAppService)
        {
            this._productAppService = productAppService;
            this._productCategoryAppService = productCategoryAppService;
        }

        public async Task OnGetAsync(string code, int page = 1)
        {
            Category = await _productCategoryAppService.GetByCodeAsync(code);
            Categories = await _productCategoryAppService.GetListAllAsync();
            ProductData = await _productAppService.GetListFilterAsync(new ProductListFilterDto()
            {
                CurrentPage = page,
                CategoryId = Category.Id
            });
        }
    }
}
