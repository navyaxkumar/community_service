using DigitalShield.API.Models;

namespace DigitalShield.API.Data.Seed;

public static class FraudCategorySeed
{
    public const string PhishingOtpScamsName = "Phishing & OTP Scams";
    public const string UpiPaymentFraudName = "UPI & Payment Fraud";
    public const string FakeJobScamsName = "Fake Job Scams";
    public const string OnlineShoppingScamsName = "Online Shopping Scams";
    public const string FakeWebsitesLinksName = "Fake Websites & Links";
    public const string ImpersonationSocialEngineeringName = "Impersonation & Social Engineering";
    public const string InvestmentLoanScamsName = "Investment & Loan Scams";

    public static IReadOnlyList<FraudCategory> CreateAll()
    {
        return
        [
            Create(PhishingOtpScamsName, "Learn how fake messages, calls, and login pages pressure people into sharing OTPs, passwords, or account details."),
            Create(UpiPaymentFraudName, "Understand common payment fraud patterns involving fake collect requests, refund traps, QR code misuse, and urgent transfer pressure."),
            Create(FakeJobScamsName, "Recognize employment scams that ask for registration fees, documents, or money before a real verified hiring process."),
            Create(OnlineShoppingScamsName, "Spot fake sellers, unrealistic discounts, advance-payment traps, and unsafe marketplace conversations."),
            Create(FakeWebsitesLinksName, "Identify suspicious links, look-alike domains, fake forms, and unsafe downloads before sharing information."),
            Create(ImpersonationSocialEngineeringName, "Learn how scammers pretend to be trusted people or organizations and how to verify independently."),
            Create(InvestmentLoanScamsName, "Understand unrealistic return promises, pressure-based loan offers, and advance-fee investment traps.")
        ];
    }

    private static FraudCategory Create(string name, string description)
    {
        return new FraudCategory
        {
            Name = name,
            Description = description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
