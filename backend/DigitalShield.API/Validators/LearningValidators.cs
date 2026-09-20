using DigitalShield.API.DTOs.Learning;
using FluentValidation;

namespace DigitalShield.API.Validators;

public class CreateLearningModuleDtoValidator : AbstractValidator<CreateLearningModuleDto>
{
    public CreateLearningModuleDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FraudCategoryId).GreaterThan(0).When(x => x.FraudCategoryId.HasValue);
    }
}

public class UpdateLearningModuleDtoValidator : AbstractValidator<UpdateLearningModuleDto>
{
    public UpdateLearningModuleDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FraudCategoryId).GreaterThan(0).When(x => x.FraudCategoryId.HasValue);
    }
}
