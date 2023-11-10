using Volo.Abp.Application.Dtos;

namespace TeduEcommerce.Public
{
    public class BaseListFilterDto : PagedResultRequestBase
    {
        public string Keyword { get; set; }
    }
}
