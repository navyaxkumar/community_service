using DigitalShield.API.DTOs.Fraud;
using DigitalShield.API.Fraud.Analyzer;
using DigitalShield.API.Fraud.Recommendations;
using DigitalShield.API.Fraud.Scoring;
using DigitalShield.API.Interfaces.Services;

namespace DigitalShield.API.Services;

public class FraudAnalysisService : IFraudAnalysisService
{
    public const int MaxMessageLength = 10_000;
    public const int MaxUrlLength = 2_048;
    private const string Disclaimer = "This assessment uses predefined risk indicators and is not proof that content is fraudulent or safe.";
    private readonly IFraudAnalyzer _analyzer;
    private readonly IRecommendationService _recommendationService;
    private readonly IRiskScoringService _scoringService;

    public FraudAnalysisService(
        IFraudAnalyzer analyzer,
        IRiskScoringService scoringService,
        IRecommendationService recommendationService)
    {
        _analyzer = analyzer;
        _scoringService = scoringService;
        _recommendationService = recommendationService;
    }

    public FraudAnalysisResponseDto Analyze(FraudCheckRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!Enum.TryParse<FraudInputType>(request.Type, true, out var inputType))
        {
            throw new ArgumentException("Type must be Message or Url.", nameof(request));
        }

        var content = request.Content?.Trim() ?? string.Empty;
        var url = request.Url?.Trim();
        if (inputType == FraudInputType.Message && string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content is required.", nameof(request));
        }
        if (inputType == FraudInputType.Url && string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL is required.", nameof(request));
        }
        if (content.Length > MaxMessageLength || (url?.Length ?? 0) > MaxUrlLength)
        {
            throw new ArgumentException("The submitted content exceeds the allowed size.", nameof(request));
        }

        var input = new FraudAnalysisInput(inputType, content, url, request.SenderKnown);
        var indicators = _analyzer.Analyze(input);
        var score = _scoringService.Score(indicators);
        var recommendations = _recommendationService.GetRecommendations(indicators, score.RiskLevel);

        return new FraudAnalysisResponseDto
        {
            Score = score.Score,
            RiskLevel = score.RiskLevel.ToString(),
            Indicators = indicators.Select(indicator => new FraudIndicatorDto
            {
                Code = indicator.Code,
                Title = indicator.Title,
                Score = indicator.Score,
                Explanation = indicator.Explanation,
                Category = indicator.Category
            }).ToList(),
            Recommendations = recommendations.Select(recommendation => new RecommendationDto
            {
                Code = recommendation.Code,
                Message = recommendation.Message
            }).ToList(),
            Disclaimer = Disclaimer
        };
    }
}
