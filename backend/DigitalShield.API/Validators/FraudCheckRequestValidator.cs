using System.Net;
using DigitalShield.API.DTOs.Fraud;
using DigitalShield.API.Services;
using FluentValidation;

namespace DigitalShield.API.Validators;

public class FraudCheckRequestValidator : AbstractValidator<FraudCheckRequestDto>
{
    public FraudCheckRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(type => string.Equals(type, "Message", StringComparison.OrdinalIgnoreCase) || string.Equals(type, "Url", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Type must be Message or Url.");
        RuleFor(x => x.Content)
            .MaximumLength(FraudAnalysisService.MaxMessageLength)
            .When(x => x.Content is not null);
        RuleFor(x => x.Url)
            .MaximumLength(FraudAnalysisService.MaxUrlLength)
            .When(x => x.Url is not null);
        RuleFor(x => x.Content)
            .NotEmpty()
            .When(x => string.Equals(x.Type, "Message", StringComparison.OrdinalIgnoreCase));
        RuleFor(x => x.Url)
            .NotEmpty()
            .Must(BeHttpUrl)
            .When(x => string.Equals(x.Type, "Url", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Url must be an absolute HTTP or HTTPS URL.");
    }

    private static bool BeHttpUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
            !string.IsNullOrWhiteSpace(uri.Host);
    }
}
