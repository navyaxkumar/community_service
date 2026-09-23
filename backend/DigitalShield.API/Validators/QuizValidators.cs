using DigitalShield.API.DTOs.Quiz;
using FluentValidation;

namespace DigitalShield.API.Validators;

public class CreateQuizDtoValidator : AbstractValidator<CreateQuizDto>
{
    public CreateQuizDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.FraudCategoryId).GreaterThan(0).When(x => x.FraudCategoryId.HasValue);
        RuleFor(x => x.Questions).NotEmpty();
        RuleForEach(x => x.Questions).SetValidator(new CreateQuizQuestionDtoValidator());
    }
}

public class UpdateQuizDtoValidator : AbstractValidator<UpdateQuizDto>
{
    public UpdateQuizDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.FraudCategoryId).GreaterThan(0).When(x => x.FraudCategoryId.HasValue);
        RuleFor(x => x.Questions).NotEmpty();
        RuleForEach(x => x.Questions).SetValidator(new CreateQuizQuestionDtoValidator());
    }
}

public class CreateQuizQuestionDtoValidator : AbstractValidator<CreateQuizQuestionDto>
{
    public CreateQuizQuestionDtoValidator()
    {
        RuleFor(x => x.QuestionText).NotEmpty();
        RuleFor(x => x.Explanation).MaximumLength(1000);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Options).NotEmpty();
        RuleForEach(x => x.Options).SetValidator(new CreateQuizOptionDtoValidator());
    }
}

public class UpdateQuizQuestionDtoValidator : AbstractValidator<UpdateQuizQuestionDto>
{
    public UpdateQuizQuestionDtoValidator()
    {
        RuleFor(x => x.QuestionText).NotEmpty();
        RuleFor(x => x.Explanation).MaximumLength(1000);
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Options).NotEmpty();
        RuleForEach(x => x.Options).SetValidator(new CreateQuizOptionDtoValidator());
    }
}

public class CreateQuizOptionDtoValidator : AbstractValidator<CreateQuizOptionDto>
{
    public CreateQuizOptionDtoValidator()
    {
        RuleFor(x => x.OptionText).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}

public class SubmitQuizAttemptDtoValidator : AbstractValidator<SubmitQuizAttemptDto>
{
    public SubmitQuizAttemptDtoValidator()
    {
        RuleFor(x => x.QuizId).GreaterThan(0);
        RuleFor(x => x.Answers).NotEmpty();
        RuleForEach(x => x.Answers).SetValidator(new QuizAnswerDtoValidator());
    }
}

public class QuizAnswerDtoValidator : AbstractValidator<QuizAnswerDto>
{
    public QuizAnswerDtoValidator()
    {
        RuleFor(x => x.QuestionId).GreaterThan(0);
        RuleFor(x => x.OptionId).GreaterThan(0);
    }
}
