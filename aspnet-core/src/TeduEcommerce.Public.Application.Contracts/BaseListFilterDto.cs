using Volo.Abp.Application.Dtos;

namespace TeduEcommerce.Public
{
    public class BaseListFilterDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
