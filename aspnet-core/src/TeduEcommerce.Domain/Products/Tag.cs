using Volo.Abp.Domain.Entities;

namespace TeduEcommerce.Products
{
    public class Tag : Entity<string>
    {
        public string Name { get; set; }
    }
}
