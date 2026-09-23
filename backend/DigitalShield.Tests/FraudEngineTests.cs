using DigitalShield.API.DTOs.Fraud;
using DigitalShield.API.Fraud.Analyzer;
using DigitalShield.API.Fraud.Recommendations;
using DigitalShield.API.Fraud.Rules;
using DigitalShield.API.Fraud.Scoring;
using DigitalShield.API.Services;
using DigitalShield.API.Validators;

namespace DigitalShield.Tests;

public class FraudEngineTests
{
    [Fact]
    public void EveryRule_TriggersForItsRepresentativeIndicator()
    {
        var cases = new (IFraudRule Rule, FraudAnalysisInput Input, string Code)[]
        {
            (new SensitiveInformationRule(), Message("Please share your OTP"), "SENSITIVE_INFO"),
            (new SuspiciousUrlRule(), Url("http://192.168.1.10/login"), "SUSPICIOUS_URL"),
            (new PaymentRequestRule(), Message("Please send money now"), "PAYMENT_REQUEST"),
            (new UrgencyRule(), Message("URGENT act now"), "URGENCY"),
            (new RewardClaimRule(), Message("You won a cash reward"), "REWARD_CLAIM"),
            (new UnknownSenderRule(), Message("Hello", false), "UNKNOWN_SENDER"),
            (new AccountSuspensionRule(), Message("Your account will be blocked"), "ACCOUNT_SUSPENSION")
        };

        foreach (var testCase in cases)
        {
            var result = testCase.Rule.Evaluate(testCase.Input);
            Assert.True(result.Triggered, testCase.Code);
            Assert.Equal(testCase.Code, result.Code);
            Assert.True(result.Score > 0);
        }
    }

    [Fact]
    public void Rules_AreCaseInsensitiveAndDoNotTriggerOnUnrelatedText()
    {
        Assert.True(new UrgencyRule().Evaluate(Message("uRgEnT" )).Triggered);
        Assert.True(new SensitiveInformationRule().Evaluate(Message("Your OTP is required" )).Triggered);
        Assert.False(new PaymentRequestRule().Evaluate(Message("The payment was received yesterday" )).Triggered);
        Assert.False(new RewardClaimRule().Evaluate(Message("Please review the policy" )).Triggered);
    }

    [Fact]
    public void UnknownSenderRule_OnlyTriggersWhenSenderIsExplicitlyUnknown()
    {
        Assert.False(new UnknownSenderRule().Evaluate(Message("Hello")).Triggered);
        Assert.False(new UnknownSenderRule().Evaluate(Message("Hello", true)).Triggered);
        Assert.True(new UnknownSenderRule().Evaluate(Message("Hello", false)).Triggered);
    }

    [Fact]
    public void SuspiciousUrlRule_AnalyzesUrlWithoutFetchingIt()
    {
        var rule = new SuspiciousUrlRule();
        Assert.False(rule.Evaluate(Url("https://example.com")).Triggered);
        Assert.True(rule.Evaluate(Url("http://example.com")).Triggered);
        Assert.True(rule.Evaluate(Url("https://xn--pple-43d.example")).Triggered);
        Assert.True(rule.Evaluate(Message("Visit https://bit.ly/example")).Triggered);
        Assert.False(rule.Evaluate(Message("No link here")).Triggered);
    }

    [Fact]
    public void Scoring_UsesBoundariesAndCountsEachRuleOnce()
    {
        var scoring = new RiskScoringService();
        AssertScore(scoring.Score(Array.Empty<FraudRuleResult>()), 0, RiskLevel.Low);
        AssertScore(scoring.Score(new[] { Result("A", 29) }), 29, RiskLevel.Low);
        AssertScore(scoring.Score(new[] { Result("A", 30) }), 30, RiskLevel.Suspicious);
        AssertScore(scoring.Score(new[] { Result("A", 59) }), 59, RiskLevel.Suspicious);
        AssertScore(scoring.Score(new[] { Result("A", 60) }), 60, RiskLevel.High);
        AssertScore(scoring.Score(new[] { Result("A", 30), Result("A", 30) }), 30, RiskLevel.Suspicious);
        Assert.Equal(100, scoring.Score(new[] { Result("A", 100), Result("B", 100) }).Score);
    }

    [Fact]
    public void Analyzer_IsDeterministicAndReturnsOnlyTriggeredIndicators()
    {
        var analyzer = new FraudAnalyzer(new IFraudRule[]
        {
            new UrgencyRule(), new SensitiveInformationRule(), new PaymentRequestRule()
        });
        var input = Message("URGENT! Send money and your OTP now.");

        var first = analyzer.Analyze(input).Select(result => result.Code).ToArray();
        var second = analyzer.Analyze(input).Select(result => result.Code).ToArray();

        Assert.Equal(first, second);
        Assert.Equal(new[] { "PAYMENT_REQUEST", "SENSITIVE_INFO", "URGENCY" }, first);
    }

    [Fact]
    public void FraudAnalysisService_ReturnsExplainableRecommendationsAndDisclaimer()
    {
        var service = new FraudAnalysisService(
            new FraudAnalyzer(new IFraudRule[] { new SensitiveInformationRule(), new UrgencyRule() }),
            new RiskScoringService(),
            new RecommendationService());

        var result = service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "URGENT. Send your OTP immediately."
        });

        Assert.Equal(40, result.Score);
        Assert.Equal("Suspicious", result.RiskLevel);
        Assert.Equal(2, result.Indicators.Count);
        Assert.NotEmpty(result.Recommendations);
        Assert.Contains("not proof", result.Disclaimer, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FraudInputValidation_RejectsUnknownTypeMalformedUrlAndOversizedInput()
    {
        var validator = new FraudCheckRequestValidator();
        Assert.False(validator.Validate(new FraudCheckRequestDto { Type = "Unknown", Content = "x" }).IsValid);
        Assert.False(validator.Validate(new FraudCheckRequestDto { Type = "Url", Url = "javascript:alert(1)" }).IsValid);
        Assert.False(validator.Validate(new FraudCheckRequestDto { Type = "Message", Content = new string('x', FraudAnalysisService.MaxMessageLength + 1) }).IsValid);
        Assert.False(validator.Validate(new FraudCheckRequestDto { Type = null! }).IsValid);
    }

    [Fact]
    public void FraudAnalysisService_RejectsOversizedInputAndHandlesUnicode()
    {
        var service = new FraudAnalysisService(
            new FraudAnalyzer(new IFraudRule[] { new UrgencyRule() }),
            new RiskScoringService(),
            new RecommendationService());
        Assert.Throws<ArgumentException>(() => service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = new string('x', FraudAnalysisService.MaxMessageLength + 1)
        }));

        var result = service.Analyze(new FraudCheckRequestDto { Type = "Message", Content = "😀\tこんにちは" });
        Assert.Equal("Low", result.RiskLevel);
    }

    private static FraudAnalysisInput Message(string content, bool? senderKnown = null) =>
        new(FraudInputType.Message, content, null, senderKnown);

    private static FraudAnalysisInput Url(string url) =>
        new(FraudInputType.Url, string.Empty, url, null);

    private static FraudRuleResult Result(string code, int score) =>
        new(code, code, score, "test", "test", true);

    private static void AssertScore(RiskScoreResult result, int score, RiskLevel riskLevel)
    {
        Assert.Equal(score, result.Score);
        Assert.Equal(riskLevel, result.RiskLevel);
    }
}
