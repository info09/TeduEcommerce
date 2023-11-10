using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TeduEcommerce.Public.Catalog.ProductCategories;

namespace TeduEcommerce.Public.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IProductCategoryAppService _productCategoryAppService;

        public HeaderViewComponent(IProductCategoryAppService appService)
        {
            _productCategoryAppService = appService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _productCategoryAppService.GetListAllAsync();
            return View(model);
        }
    }
}
