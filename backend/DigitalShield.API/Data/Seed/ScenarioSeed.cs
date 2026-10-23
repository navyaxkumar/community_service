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
            "Do not rush. Verify the provider through official sources, avoid guaranteed-profit claims, and do not send money to personal accounts."),

        // --- Age-Adaptive Scenarios ---
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "The Shiny Game Gem Popup",
            "[Audience: EarlyLearners | Difficulty: Beginner] A game promises free gems if you type your mom's phone number.",
            """
            <!--digitalshield-metadata:{"audience":"EarlyLearners","difficulty":"Beginner","duration":2,"safetyTips":"Never type phone numbers in game popups."}-->
            You are playing a color game on a tablet. A popup box with smiling balloons says: "You won 5,000 shiny gems! Type your mom's phone number now!"
            """,
            "Do not type any numbers. Put the tablet down and ask a trusted grown-up to look at the screen."),
        new(
            FraudCategorySeed.OnlineShoppingScamsName,
            "Roblox Rare Skin Trade Offer",
            "[Audience: Children | Difficulty: Beginner] An online player asks for your password to give you rare skins.",
            """
            <!--digitalshield-metadata:{"audience":"Children","difficulty":"Beginner","duration":3,"safetyTips":"Passwords should never be shared for items or trades."}-->
            While playing your favorite multiplayer game, another player whispers: "Give me your password for 5 minutes and I will load legendary skins onto your avatar."
            """,
            "Never share your password with anyone online. Block the user and report the message to a parent or moderator."),
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Sponsored Clothing Brand Collab DM",
            "[Audience: Teens | Difficulty: Intermediate] A direct message offers $500 for a collab but sends a link to claim it.",
            """
            <!--digitalshield-metadata:{"audience":"Teens","difficulty":"Intermediate","duration":4,"safetyTips":"Never log into third-party sites via direct message links."}-->
            You get a DM on Instagram from an account named 'FashionAmbassador_Official' saying they want you to model hoodies for $500. They ask you to click a link and log in to fill the shipping form.
            """,
            "Do not click or log into external links sent via DM. Real brand partnerships do not require third-party login pages."),
        new(
            FraudCategorySeed.FakeJobScamsName,
            "Paid Summer Internship Application Fee",
            "[Audience: YoungAdults | Difficulty: Intermediate] A recruiter asks for a background check fee before sending an offer.",
            """
            <!--digitalshield-metadata:{"audience":"YoungAdults","difficulty":"Intermediate","duration":4,"safetyTips":"Legitimate employers never charge applicant fees."}-->
            After applying on LinkedIn, a recruiter contacts you on Telegram offering an immediate remote research assistant position at $25/hour, but insists you pay a $40 document processing fee first.
            """,
            "Never pay upfront fees for employment. Reputable companies cover onboarding costs and conduct interviews on formal platforms."),
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Electricity Disconnection Threat Call",
            "[Audience: Seniors | Difficulty: Beginner] A caller claims your power will be cut off in 45 minutes.",
            """
            <!--digitalshield-metadata:{"audience":"Seniors","difficulty":"Beginner","duration":3,"safetyTips":"Utility companies do not demand immediate phone payments."}-->
            A caller claims to be from the State Electricity Board, stating your bill is overdue and power will be cut in 45 minutes unless you immediately pay via a link they sent to your phone.
            """,
            "Stay calm and hang up immediately. Check your actual electricity bill or call the official power utility customer service number from your monthly statement."),
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Urgent Vendor Wire Redirection Request",
            "[Audience: Professionals | Difficulty: Advanced] An email from a known supplier requests invoice payment to a new bank account.",
            """
            <!--digitalshield-metadata:{"audience":"Professionals","difficulty":"Advanced","duration":5,"safetyTips":"Enforce out-of-band phone verification before modifying routing instructions."}-->
            On Friday afternoon, your accounts payable team receives an email from an established supplier's representative saying their primary bank is undergoing an audit, requesting Monday's $60,000 payment be wired to a new routing number.
            """,
            "Halt the payment. Perform mandatory out-of-band verification by calling the supplier's CFO or account manager at a known, independently verified telephone number.")
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
