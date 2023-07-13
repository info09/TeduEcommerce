using Volo.Abp.Application.Dtos;

namespace TeduEcommerce.Admin
{
    public class BaseListFilterDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
