using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeduEcommerce.Public.Catalog.ProductCategories;
using TeduEcommerce.Public.Catalog.Products;
using TeduEcommerce.Public.Web.Models;
using Volo.Abp.Caching;

namespace TeduEcommerce.Public.Web.Pages.Home
{
    public class IndexModel : PublicPageModel
    {
        private readonly IDistributedCache<HomeCacheItem> _distributedCache;
        private readonly IProductCategoryAppService _productCategoryAppService;
        private readonly IProductAppService _productAppService;

        public List<ProductCategoryInListDto> Categories { set; get; }
        public List<ProductInListDto> TopSellerProducts { set; get; }

        public IndexModel(IDistributedCache<HomeCacheItem> distributedCache, IProductAppService productAppService, IProductCategoryAppService productCategoryAppService)
        {
            this._distributedCache = distributedCache;
            this._productAppService = productAppService;
            this._productCategoryAppService = productCategoryAppService;
        }
        public async Task OnGetAsync()
        {
            var cacheItem = await _distributedCache.GetOrAddAsync(TeduEcommercePublicConsts.CacheKeys.HomeData, async () =>
            {
                var allCategories = await _productCategoryAppService.GetListAllAsync();
                var rootCategories = allCategories.Where(i => i.ParentId == null).ToList();
                foreach (var category in rootCategories)
                {
                    category.Children = rootCategories.Where(i => i.ParentId == category.Id).ToList();
                }
                var topSellerProducts = await _productAppService.GetListTopSellerAsync(10);
                return new HomeCacheItem()
                {
                    Categories = rootCategories,
                    TopSellerProducts = topSellerProducts
                };
            },
            () => new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            });

            TopSellerProducts = cacheItem.TopSellerProducts;
            Categories = cacheItem.Categories;
        }
    }
}
