using DigitalShield.API.DTOs.Scenario;
using FluentValidation;

namespace DigitalShield.API.Validators;

public class CreateScenarioDtoValidator : AbstractValidator<CreateScenarioDto>
{
    public CreateScenarioDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Situation).NotEmpty();
        RuleFor(x => x.CorrectAction).NotEmpty();
        RuleFor(x => x.FraudCategoryId).GreaterThan(0).When(x => x.FraudCategoryId.HasValue);
    }
}

public class UpdateScenarioDtoValidator : AbstractValidator<UpdateScenarioDto>
{
    public UpdateScenarioDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Situation).NotEmpty();
        RuleFor(x => x.CorrectAction).NotEmpty();
        RuleFor(x => x.FraudCategoryId).GreaterThan(0).When(x => x.FraudCategoryId.HasValue);
    }
}
