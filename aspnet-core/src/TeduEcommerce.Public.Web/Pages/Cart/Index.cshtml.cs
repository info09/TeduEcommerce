using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TeduEcommerce.Public.Catalog.Products;
using TeduEcommerce.Public.Web.Models;

namespace TeduEcommerce.Public.Web.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly IProductAppService _productAppService;

        public IndexModel(IProductAppService productAppService)
        {
            this._productAppService = productAppService;

        }

        [BindProperty]
        public List<CartItem> CartItems { get; set; }

        public async Task OnGetAsync(string action, string id)
        {
            //var cart = HttpContext.Session.GetStringSession(TeduEcommerceConsts.Cart);
            var cart = HttpContext.Session.Get(TeduEcommerceConsts.Cart);
            var productCarts = new Dictionary<string, CartItem>();
            if (cart != null)
            {
                productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
            }

            if (!string.IsNullOrEmpty(action))
            {
                if (action == "add")
                {
                    var product = await _productAppService.GetAsync(Guid.Parse(id));
                    if (cart == null)
                    {
                        productCarts.Add(id, new CartItem()
                        {
                            Product = product,
                            Quantity = 1
                        });
                        var value = JsonSerializer.Serialize(productCarts);
                        byte[] bytes = Encoding.UTF8.GetBytes(value);
                        HttpContext.Session.Set(TeduEcommerceConsts.Cart, bytes);
                    }
                    else
                    {
                        productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
                        if (productCarts.ContainsKey(id))
                        {
                            productCarts[id].Quantity += 1;
                        }
                        else
                        {
                            productCarts.Add(id, new CartItem()
                            {
                                Product = product,
                                Quantity = 1
                            });
                            var value = JsonSerializer.Serialize(productCarts);
                            byte[] bytes = Encoding.UTF8.GetBytes(value);
                            HttpContext.Session.Set(TeduEcommerceConsts.Cart, bytes);
                        }
                    }
                }
                else if (action == "remove")
                {
                    productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
                    if (productCarts.ContainsKey(id))
                    {
                        productCarts.Remove(id);
                    }
                    var value = JsonSerializer.Serialize(productCarts);
                    byte[] bytes = Encoding.UTF8.GetBytes(value);
                    HttpContext.Session.Set(TeduEcommerceConsts.Cart, bytes);
                }
            }
            CartItems = productCarts.Values.ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cart = HttpContext.Session.Get(TeduEcommerceConsts.Cart);
            var productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
            foreach (var item in productCarts)
            {
                var cartItem = CartItems.FirstOrDefault(x => x.Product.Id == item.Value.Product.Id);
                cartItem.Product = await _productAppService.GetAsync(cartItem.Product.Id);
                item.Value.Quantity = cartItem != null ? cartItem.Quantity : 0;
            }

            var value = JsonSerializer.Serialize(productCarts);
            var bytes = Encoding.UTF8.GetBytes(value);

            HttpContext.Session.Set(TeduEcommerceConsts.Cart, bytes);
            return Redirect("/shop-cart.html");
        }
    }
}
