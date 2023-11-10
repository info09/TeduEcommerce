using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TeduEcommerce.Public.Catalog.ProductCategories;
using TeduEcommerce.Public.Web.Models;
using Volo.Abp.Caching;

namespace TeduEcommerce.Public.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IProductCategoryAppService _productCategoryAppService;
        private readonly IDistributedCache<HeaderCacheItem> _headerCacheItem;

        public HeaderViewComponent(IProductCategoryAppService appService, IDistributedCache<HeaderCacheItem> headerCacheItem)
        {
            _productCategoryAppService = appService;
            _headerCacheItem = headerCacheItem;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cacheItem = await _headerCacheItem.GetOrAddAsync(TeduEcommercePublicConsts.CacheKeys.HeaderData, async () =>
            {
                var model = await _productCategoryAppService.GetListAllAsync();
                return new HeaderCacheItem() { Categories = model };
            }, () => new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            });
            var model = await _productCategoryAppService.GetListAllAsync();
            return View(cacheItem.Categories);
        }
    }
}
