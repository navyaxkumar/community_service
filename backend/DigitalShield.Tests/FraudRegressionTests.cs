using DigitalShield.API.DTOs.Fraud;
using DigitalShield.API.Fraud.Analyzer;
using DigitalShield.API.Fraud.Recommendations;
using DigitalShield.API.Fraud.Rules;
using DigitalShield.API.Fraud.Scoring;
using DigitalShield.API.Fraud.ThreatIntelligence;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Services;

namespace DigitalShield.Tests;

public class FraudRegressionTests
{
    private readonly IFraudAnalysisService _service;

    public FraudRegressionTests()
    {
        var rules = new IFraudRule[]
        {
            new SensitiveInformationRule(),
            new SuspiciousUrlRule(),
            new PaymentRequestRule(),
            new UrgencyRule(),
            new RewardClaimRule(),
            new UnknownSenderRule(),
            new AccountSuspensionRule(),
            new EducationalContextRule(),
            new RemoteAccessRule(),
            new JobScamRule(),
            new QrPaymentRule(),
            new BrandImpersonationRule()
        };

        var analyzer = new FraudAnalyzer(rules);
        var scoringService = new RiskScoringService();
        var recommendationService = new RecommendationService();
        var threatProvider = new LocalThreatIntelligenceProvider();

        _service = new FraudAnalysisService(analyzer, scoringService, recommendationService, threatProvider);
    }

    [Fact]
    public void EducationalWarning_Otp_ClassifiedAsLowRisk()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "Remember that your bank will never ask for your password or OTP."
        });

        Assert.Equal("Low", result.RiskLevel);
        Assert.True(result.Score <= 29, $"Expected low score, got {result.Score}");
        Assert.DoesNotContain(result.Indicators, i => i.Code == "SENSITIVE_INFO");
    }

    [Fact]
    public void EducationalWarning_PasswordSafety_ClassifiedAsLowRisk()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "Cybercrime awareness advisory: Never share your passwords or credentials with anyone claiming to be customer support."
        });

        Assert.Equal("Low", result.RiskLevel);
        Assert.True(result.Score <= 29);
    }

    [Fact]
    public void BenignUrgentMessage_MeetingRescheduled_ClassifiedAsLowRisk()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "Meeting rescheduled urgently to 4 PM."
        });

        Assert.Equal("Low", result.RiskLevel);
        Assert.Equal(0, result.Score);
        Assert.DoesNotContain(result.Indicators, i => i.Code == "URGENCY");
    }

    [Fact]
    public void ActualOtpTheftRequest_ClassifiedAsHighOrCritical()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "Your State Bank account will be blocked today. Please send us your OTP immediately to restore service."
        });

        Assert.True(result.RiskLevel == "High" || result.RiskLevel == "Critical", $"Actual: {result.RiskLevel}");
        Assert.True(result.Score >= 60);
        Assert.Contains(result.Indicators, i => i.Code == "SENSITIVE_INFO");
        Assert.Contains(result.Indicators, i => i.Code == "ACCOUNT_SUSPENSION");
    }

    [Fact]
    public void ThreateningAccountSuspension_WithPhishingLink_ClassifiedAsCritical()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "URGENT: Your account has been suspended! Enter your netbanking password at http://sbi-login-verify.com to avoid permanent closure."
        });

        Assert.Equal("Critical", result.RiskLevel);
        Assert.True(result.Score >= 85, $"Expected critical score, got {result.Score}");
        Assert.Contains(result.Indicators, i => i.Code == "SUSPICIOUS_URL");
        Assert.Contains(result.Indicators, i => i.Code == "SENSITIVE_INFO");
    }

    [Fact]
    public void JobDepositScam_ClassifiedAsHighRisk()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "Part-time job offer! Earn Rs 5000 per day by liking YouTube videos and rating hotels. Pay Rs 500 registration fee to start today."
        });

        Assert.True(result.RiskLevel == "High" || result.RiskLevel == "Critical");
        Assert.Contains(result.Indicators, i => i.Code == "JOB_SCAM");
    }

    [Fact]
    public void EducationalJobScamDiscussion_ClassifiedAsLowRisk()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "How to spot fake jobs: Legitimate employers never ask candidates to pay an upfront registration fee or security deposit."
        });

        Assert.Equal("Low", result.RiskLevel);
        Assert.True(result.Score <= 29);
        Assert.DoesNotContain(result.Indicators, i => i.Code == "JOB_SCAM");
    }

    [Fact]
    public void RemoteAccessSoftware_RequestedByUnknownSender_ClassifiedAsHighOrCritical()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "Customer care refund desk: Please install AnyDesk on your mobile phone and share the 9-digit code so we can process your pending refund.",
            SenderKnown = false
        });

        Assert.True(result.RiskLevel == "High" || result.RiskLevel == "Critical");
        Assert.Contains(result.Indicators, i => i.Code == "REMOTE_ACCESS");
    }

    [Fact]
    public void BenignMentionOfRemoteAccess_ClassifiedAsLowRisk()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "Hi team, our company IT support uses TeamViewer for scheduled workstation updates on Tuesdays."
        });

        Assert.Equal("Low", result.RiskLevel);
        Assert.DoesNotContain(result.Indicators, i => i.Code == "REMOTE_ACCESS");
    }

    [Fact]
    public void UrlWithEmbeddedCredentials_DetectedAsMisleadingAuthority()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Url",
            Url = "https://google.com@phishing-target.com/login"
        });

        Assert.True(result.Score >= 30);
        Assert.Contains(result.Indicators, i => i.Code == "SUSPICIOUS_URL");
        Assert.Contains(result.Indicators, i => i.Title.Contains("authority", StringComparison.OrdinalIgnoreCase) ||
                                                i.Explanation.Contains("mislead", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void LegitimateMultiLabelDomain_NotFlaggedAsSuspicious()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Url",
            Url = "https://portal.library.ox.ac.uk/catalog/search"
        });

        Assert.Equal("Low", result.RiskLevel);
        Assert.Equal(0, result.Score);
        Assert.Empty(result.Indicators);
    }

    [Fact]
    public void PunycodeDomain_DetectedAsHomographRisk()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Url",
            Url = "https://xn--pple-43d.example"
        });

        Assert.Contains(result.Indicators, i => i.Code == "SUSPICIOUS_URL");
    }

    [Fact]
    public void LookalikeBrandDomain_DetectedWithBrandEvidence()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Url",
            Url = "http://sbi-kyc-update.com/netbanking"
        });

        Assert.True(result.Score >= 35);
        Assert.Contains(result.Indicators, i => i.Code == "SUSPICIOUS_URL");
        Assert.Contains(result.Indicators, i => i.Title.Contains("lookalike", StringComparison.OrdinalIgnoreCase) ||
                                                i.Title.Contains("SBI", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ShortenedUrl_FlaggedWithReasonForCaution()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Url",
            Url = "https://bit.ly/secure-login-portal"
        });

        Assert.Contains(result.Indicators, i => i.Code == "SUSPICIOUS_URL");
        Assert.Contains(result.Indicators, i => i.Title.Contains("Shortened", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MalformedUrl_InputValidationRejectsGracefully()
    {
        var validator = new DigitalShield.API.Validators.FraudCheckRequestValidator();
        var validation = validator.Validate(new FraudCheckRequestDto
        {
            Type = "Url",
            Url = "not a valid url"
        });

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void QrPaymentScam_DetectedWithHighRisk()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Message",
            Content = "You have received a cashback prize of Rs 2,500. Scan this QR code and enter your UPI PIN to claim money into your bank."
        });

        Assert.True(result.RiskLevel == "High" || result.RiskLevel == "Critical");
        Assert.Contains(result.Indicators, i => i.Code == "QR_PAYMENT_SCAM");
    }

    [Fact]
    public void LocalThreatIntelligence_ReportsNoExternalCheckPerformed()
    {
        var result = _service.Analyze(new FraudCheckRequestDto
        {
            Type = "Url",
            Url = "https://example.com"
        });

        Assert.False(result.ExternalReputationCheckPerformed);
        Assert.Equal("Local Threat Engine", result.ThreatIntelligenceSource);
        Assert.Contains("predefined risk indicators", result.Disclaimer, StringComparison.OrdinalIgnoreCase);
    }
}
