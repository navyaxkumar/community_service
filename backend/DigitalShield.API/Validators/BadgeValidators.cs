using DigitalShield.API.DTOs.Badge;
using FluentValidation;

namespace DigitalShield.API.Validators;

public class CreateBadgeDtoValidator : AbstractValidator<CreateBadgeDto>
{
    public CreateBadgeDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Icon).MaximumLength(200);
        RuleFor(x => x.RequiredPoints).GreaterThanOrEqualTo(0);
    }
}

public class UpdateBadgeDtoValidator : AbstractValidator<UpdateBadgeDto>
{
    public UpdateBadgeDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Icon).MaximumLength(200);
        RuleFor(x => x.RequiredPoints).GreaterThanOrEqualTo(0);
    }
}
