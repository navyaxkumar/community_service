using DigitalShield.API.DTOs.Progress;
using FluentValidation;

namespace DigitalShield.API.Validators;

public class CreateUserProgressDtoValidator : AbstractValidator<CreateUserProgressDto>
{
    public CreateUserProgressDtoValidator()
    {
        RuleFor(x => x.LearningModuleId).GreaterThan(0);
        RuleFor(x => x.ProgressPercentage).InclusiveBetween(0, 100);
    }
}

public class UpdateUserProgressDtoValidator : AbstractValidator<UpdateUserProgressDto>
{
    public UpdateUserProgressDtoValidator()
    {
        RuleFor(x => x.ProgressPercentage).InclusiveBetween(0, 100);
    }
}
