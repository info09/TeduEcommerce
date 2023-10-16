using FluentValidation;

namespace TeduEcommerce.Admin.Catalog.ProductCategories
{
    public class CreateUpdateProductCategoryDtoValidator : AbstractValidator<CreateUpdateProductCategoryDto>
    {
        public CreateUpdateProductCategoryDtoValidator()
        {
            RuleFor(i => i.Name).NotEmpty().MaximumLength(50);
            RuleFor(i => i.Code).NotEmpty().MaximumLength(50);
            RuleFor(i => i.Slug).NotEmpty().MaximumLength(50);
            RuleFor(i => i.CoverPicture).MaximumLength(250);
            RuleFor(i => i.SeoMetaDescription).MaximumLength(250);
        }
    }
}
