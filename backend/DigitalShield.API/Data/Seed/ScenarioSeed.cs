using DigitalShield.API.Models;

namespace DigitalShield.API.Data.Seed;

public static class ScenarioSeed
{
    public static IReadOnlyList<ScenarioDefinition> All =>
    [
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Unexpected OTP Verification Message",
            "A message pressures the user to share an OTP to avoid account blocking.",
            "You receive a message saying, \"Your account will be blocked today. Share the OTP you just received to keep it active.\"",
            "Do not share the OTP. Stop the interaction, avoid using links in the message, and verify through the official app, website, or support number."),
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Fake Bank Support Caller",
            "A caller claims to be from bank support and asks for private verification details.",
            "A caller says they are from your bank and need your OTP and card PIN to reverse a suspicious transaction.",
            "End the call. Real support should not ask for OTPs, PINs, or passwords. Contact the bank through an official channel and report the call."),
        new(
            FraudCategorySeed.UpiPaymentFraudName,
            "Payment Collect Request Trap",
            "A buyer sends a collect request and says it is needed to pay the seller.",
            "You are selling an item online. The buyer says, \"Approve this request so I can send you the money.\" The app screen asks you to enter your payment PIN.",
            "Cancel the request. Entering a PIN may send money out. Confirm payment only by checking your own account balance or transaction history."),
        new(
            FraudCategorySeed.FakeJobScamsName,
            "Job Offer Registration Fee",
            "A recruiter offers quick hiring but asks for an upfront fee.",
            "A person claiming to be a recruiter offers a work-from-home job and asks you to pay a registration fee before the interview.",
            "Do not pay. Verify the job on the company's official careers page and use official contact channels before sharing documents or money."),
        new(
            FraudCategorySeed.OnlineShoppingScamsName,
            "Unrealistic Shopping Discount",
            "A seller offers a large discount and asks for direct advance payment.",
            "A social media seller offers a new phone at a very low price but asks you to pay directly outside the shopping platform.",
            "Avoid direct advance payment. Use trusted platforms, review seller history, and keep payment and communication inside protected channels where possible."),
        new(
            FraudCategorySeed.FakeWebsitesLinksName,
            "Look-Alike Login Page",
            "A link opens a page that looks like a familiar service but asks for sensitive information.",
            "You receive a link to https://secure-update.example.invalid asking you to log in and enter an OTP to keep your wallet active.",
            "Do not enter details. Close the page and open the official app or website yourself. Report the suspicious link if the service provides a reporting channel."),
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Family Emergency Message",
            "A message pretends to be a family member needing urgent money.",
            "You receive a message saying, \"I changed my number. I am in trouble and need money immediately. Please do not call.\"",
            "Pause and verify through another trusted channel. Call the known number or contact another family member before sending money."),
        new(
            FraudCategorySeed.InvestmentLoanScamsName,
            "Guaranteed Investment Return",
            "A group promises quick guaranteed profit and asks for immediate payment.",
            "A chat group claims you can double your money in a week if you pay today to join a special investment plan.",
            "Do not rush. Verify the provider through official sources, avoid guaranteed-profit claims, and do not send money to personal accounts.")
    ];
}

public sealed record ScenarioDefinition(
    string CategoryName,
    string Title,
    string Description,
    string Situation,
    string CorrectAction)
{
    public Scenario Create(FraudCategory category)
    {
        return new Scenario
        {
            Title = Title,
            Description = Description,
            Situation = Situation,
            CorrectAction = CorrectAction,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            FraudCategory = category
        };
    }
}
