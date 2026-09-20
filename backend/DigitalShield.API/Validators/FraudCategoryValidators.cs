using DigitalShield.API.DTOs.FraudCategory;
using FluentValidation;

namespace DigitalShield.API.Validators;

public class CreateFraudCategoryDtoValidator : AbstractValidator<CreateFraudCategoryDto>
{
    public CreateFraudCategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateFraudCategoryDtoValidator : AbstractValidator<UpdateFraudCategoryDto>
{
    public UpdateFraudCategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
