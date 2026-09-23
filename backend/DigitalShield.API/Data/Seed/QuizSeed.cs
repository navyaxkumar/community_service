using DigitalShield.API.Models;

namespace DigitalShield.API.Data.Seed;

public static class QuizSeed
{
    public static IReadOnlyList<QuizDefinition> All =>
    [
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Phishing and OTP Safety Quiz",
            "Practice safe responses to OTP requests, suspicious messages, and fake support claims.",
            [
                Question("What is the safest response when an unexpected caller asks for your OTP?",
                    "Unexpected OTP requests are a warning sign. OTPs should not be shared with callers or message senders.",
                    1,
                    Correct("Do not share it and verify through the official app or support channel."),
                    Wrong("Read the OTP if the caller knows your name."),
                    Wrong("Share only half of the OTP."),
                    Wrong("Forward the OTP message to the caller.")),
                Question("A message says your wallet KYC expires tonight and includes a link. What should you do first?",
                    "Using the official app or website avoids relying on a link from an unverified message.",
                    2,
                    Correct("Open the official app or website yourself to check."),
                    Wrong("Tap the link and enter your password quickly."),
                    Wrong("Reply with your account number."),
                    Wrong("Ask the sender to call you and guide you.")),
                Question("Which detail is a common warning sign in phishing messages?",
                    "Urgency and threats are often used to make people act without checking.",
                    3,
                    Correct("Urgent threats asking for immediate action."),
                    Wrong("A message written in simple language."),
                    Wrong("A message arriving during the day."),
                    Wrong("A message that mentions safety.")),
                Question("What should you do after accidentally sharing a password?",
                    "Fast damage control through official channels can reduce risk.",
                    4,
                    Correct("Change it through the official service and contact support if needed."),
                    Wrong("Wait a few days to see what happens."),
                    Wrong("Send the new password to the same caller."),
                    Wrong("Post about it publicly with account details.")),
                Question("Who should be trusted with your OTP?",
                    "OTPs are private verification codes and should not be shared with anyone.",
                    5,
                    Correct("No one who contacts you unexpectedly."),
                    Wrong("Anyone claiming to be senior support."),
                    Wrong("A seller if they promise a refund."),
                    Wrong("A caller who says the request is urgent."))
            ]),
        new(
            FraudCategorySeed.UpiPaymentFraudName,
            "UPI and Payment Request Safety Quiz",
            "Check your understanding of collect requests, QR codes, and payment confirmation screens.",
            [
                Question("What can happen if you approve a collect request and enter your payment PIN?",
                    "A collect request can move money from your account when approved.",
                    1,
                    Correct("Money may be sent from your account."),
                    Wrong("Money is always received by you."),
                    Wrong("The request only checks your name."),
                    Wrong("The PIN is ignored by the app.")),
                Question("Someone says to scan a QR code to receive a refund. What is safest?",
                    "QR codes commonly start a payment flow. Refunds should use official processes.",
                    2,
                    Correct("Avoid scanning and use the official refund process."),
                    Wrong("Scan it and enter your PIN quickly."),
                    Wrong("Scan it if the amount is small."),
                    Wrong("Share your payment PIN instead.")),
                Question("Before entering a payment PIN, what should you check?",
                    "Reading the confirmation screen helps identify wrong amounts and recipients.",
                    3,
                    Correct("Recipient, amount, and whether you are paying or receiving."),
                    Wrong("Only the color of the app screen."),
                    Wrong("Whether the other person sounds confident."),
                    Wrong("Only the last digit of the amount.")),
                Question("A buyer sends a payment screenshot. What should you do before handing over an item?",
                    "Your own account record is more reliable than a screenshot from someone else.",
                    4,
                    Correct("Check your own account or app transaction history."),
                    Wrong("Trust the screenshot immediately."),
                    Wrong("Delete your app notifications."),
                    Wrong("Send a refund first.")),
                Question("What is a safer response to rushed payment pressure?",
                    "Taking time to verify helps prevent mistakes.",
                    5,
                    Correct("Pause, read the app screen, and verify independently."),
                    Wrong("Approve first and check later."),
                    Wrong("Share your PIN with the caller."),
                    Wrong("Ignore the amount because the sender sounds helpful."))
            ]),
        new(
            FraudCategorySeed.FakeJobScamsName,
            "Fake Job Offer Safety Quiz",
            "Learn safer responses to advance-fee job offers and document requests.",
            [
                Question("A recruiter asks for a fee before an interview. What is safest?",
                    "Advance fees are a common warning sign in fake job offers.",
                    1,
                    Correct("Do not pay and verify the job through official company channels."),
                    Wrong("Pay quickly to reserve the job."),
                    Wrong("Send your payment PIN instead."),
                    Wrong("Borrow money if the salary sounds high.")),
                Question("Which is a warning sign in a job offer?",
                    "Pressure and payment demands before verification are risky.",
                    2,
                    Correct("Immediate hiring with a registration fee."),
                    Wrong("A written job description."),
                    Wrong("An interview scheduled in advance."),
                    Wrong("A company careers page listing roles.")),
                Question("When should sensitive documents be shared?",
                    "Documents should be shared only after reasonable verification and when necessary.",
                    3,
                    Correct("Only after verifying the employer and need for the document."),
                    Wrong("As soon as any recruiter asks."),
                    Wrong("Before knowing the company name."),
                    Wrong("In public chat groups.")),
                Question("How can you verify a recruiter?",
                    "Official company channels are safer than relying on chat claims.",
                    4,
                    Correct("Check the official company careers page or verified contact channel."),
                    Wrong("Trust copied logos in a message."),
                    Wrong("Trust an urgent voice note."),
                    Wrong("Pay a fee and wait for proof later.")),
                Question("What should you avoid during a job process?",
                    "Real hiring should not require payment PINs, OTPs, or remote access.",
                    5,
                    Correct("Sharing OTPs, payment PINs, or remote access."),
                    Wrong("Reading the job description."),
                    Wrong("Asking questions about the role."),
                    Wrong("Checking the interview schedule."))
            ]),
        new(
            FraudCategorySeed.OnlineShoppingScamsName,
            "Online Shopping Scam Safety Quiz",
            "Practice identifying fake discounts, unsafe payments, and suspicious marketplace behavior.",
            [
                Question("A seller offers a costly item at an unusually low price and wants direct payment. What should you do?",
                    "Unrealistic discounts and outside-platform payments are common warning signs.",
                    1,
                    Correct("Avoid rushing and use trusted platform payment protections."),
                    Wrong("Pay directly to get the deal first."),
                    Wrong("Send your card details in chat."),
                    Wrong("Ignore seller history.")),
                Question("A delivery message asks for a small fee through an unexpected link. What is safest?",
                    "Delivery status should be checked through official channels.",
                    2,
                    Correct("Check the order or courier status through the official app or website."),
                    Wrong("Pay through the link immediately."),
                    Wrong("Enter card details to test the link."),
                    Wrong("Forward the link to friends.")),
                Question("Why keep marketplace chats inside the platform?",
                    "Platform records and payment flows can provide protections and evidence.",
                    3,
                    Correct("It helps preserve platform protections and transaction records."),
                    Wrong("It guarantees every seller is honest."),
                    Wrong("It hides the transaction from support."),
                    Wrong("It lets sellers avoid rules.")),
                Question("Before trusting a payment screenshot from a buyer, what should you check?",
                    "A screenshot can be misleading. Your own account is the source of truth.",
                    4,
                    Correct("Confirm the credit in your own payment app or bank account."),
                    Wrong("Only check that the screenshot has your name."),
                    Wrong("Ship the item before checking."),
                    Wrong("Send a refund to confirm the buyer.")),
                Question("Which seller behavior is suspicious?",
                    "Pressure and avoidance of normal checkout can signal risk.",
                    5,
                    Correct("Insisting on immediate payment outside the platform."),
                    Wrong("Providing a clear return policy."),
                    Wrong("Answering product questions calmly."),
                    Wrong("Using the platform checkout."))
            ]),
        new(
            FraudCategorySeed.FakeWebsitesLinksName,
            "Fake Website and Link Safety Quiz",
            "Practice checking links, fake forms, and unsafe download requests.",
            [
                Question("What is safest when a message link asks you to log in?",
                    "Opening the official service yourself avoids trusting an unverified link.",
                    1,
                    Correct("Close it and open the official app or website yourself."),
                    Wrong("Enter your password to see if it works."),
                    Wrong("Share the link publicly."),
                    Wrong("Ask the sender for a shorter link.")),
                Question("Which URL should be treated as fictional training content, not a real service?",
                    "Fictional reserved domains are used for safe examples.",
                    2,
                    Correct("https://support-login.example.invalid"),
                    Wrong("The official app opened from your phone menu."),
                    Wrong("A bookmarked official website you verified earlier."),
                    Wrong("A printed website from an official statement.")),
                Question("A form asks for your password and OTP to claim a reward. What should you do?",
                    "Reward forms should not need private verification codes.",
                    3,
                    Correct("Do not submit it and verify through official channels."),
                    Wrong("Submit it if the reward is small."),
                    Wrong("Enter a wrong OTP to test it."),
                    Wrong("Ask the form owner for a bigger reward.")),
                Question("What is a warning sign in a look-alike website?",
                    "Small spelling changes and extra words can be used to mimic trusted names.",
                    4,
                    Correct("Misspellings or extra words in the website address."),
                    Wrong("A page that loads quickly."),
                    Wrong("A page with normal text size."),
                    Wrong("A website opened from your own saved bookmark.")),
                Question("How should unexpected app download links be handled?",
                    "Official app stores and provider websites are safer sources.",
                    5,
                    Correct("Avoid them and use official app stores or verified websites."),
                    Wrong("Install them if a caller says it is urgent."),
                    Wrong("Install them and share screen access."),
                    Wrong("Forward them to others first."))
            ]),
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Impersonation and Pressure Safety Quiz",
            "Learn how to slow down, verify identity, and resist secrecy pressure.",
            [
                Question("A message says a family member changed numbers and needs money urgently. What should you do?",
                    "Independent verification helps prevent impersonation scams.",
                    1,
                    Correct("Call the known number or verify through another trusted person."),
                    Wrong("Send money immediately because it sounds urgent."),
                    Wrong("Keep it secret as requested."),
                    Wrong("Share your banking PIN in chat.")),
                Question("What is a common social engineering tactic?",
                    "Scammers often use pressure to prevent careful checking.",
                    2,
                    Correct("Creating urgency and asking for secrecy."),
                    Wrong("Letting you take time to verify."),
                    Wrong("Encouraging you to contact official support."),
                    Wrong("Refusing any personal information.")),
                Question("A support caller asks you to install a remote access app. What is safest?",
                    "Unexpected remote access requests can expose your device and accounts.",
                    3,
                    Correct("Decline and contact official support yourself."),
                    Wrong("Install it so they can help faster."),
                    Wrong("Share your screen and payment PIN."),
                    Wrong("Give control only for a few minutes.")),
                Question("What should you do if someone refuses a callback?",
                    "A legitimate contact should allow reasonable verification.",
                    4,
                    Correct("Treat it as suspicious and verify through another channel."),
                    Wrong("Trust them more because they are busy."),
                    Wrong("Send money before the call ends."),
                    Wrong("Stop asking questions.")),
                Question("Which phrase should make you pause?",
                    "Secrecy and punishment threats are used to isolate people.",
                    5,
                    Correct("\"Do not tell anyone, and act now.\""),
                    Wrong("\"Please verify through our official website.\""),
                    Wrong("\"Take time to read the details.\""),
                    Wrong("\"Call back using the number on your card.\""))
            ]),
        new(
            FraudCategorySeed.InvestmentLoanScamsName,
            "Investment and Loan Scam Safety Quiz",
            "Practice recognizing guaranteed returns, advance fees, and pressure-based financial offers.",
            [
                Question("What is a warning sign in an investment offer?",
                    "Guaranteed high returns are unrealistic and should be verified carefully.",
                    1,
                    Correct("Guaranteed high profit with pressure to pay today."),
                    Wrong("Clear written risks and fees."),
                    Wrong("Time to review official documents."),
                    Wrong("A regulated provider's verified website.")),
                Question("A loan offer asks for processing fees before any verified approval. What should you do?",
                    "Advance-fee loan requests are a common scam pattern.",
                    2,
                    Correct("Pause and verify the lender through official sources."),
                    Wrong("Pay each requested fee quickly."),
                    Wrong("Share OTPs to speed up approval."),
                    Wrong("Send card details in chat.")),
                Question("Why are chat group profit screenshots risky?",
                    "Screenshots can be edited or used without context.",
                    3,
                    Correct("They can be misleading and are not proof of a safe investment."),
                    Wrong("They guarantee the same profit for everyone."),
                    Wrong("They replace official documents."),
                    Wrong("They prove the group is regulated.")),
                Question("What should you avoid before investing?",
                    "Borrowing or rushing into unclear schemes increases harm if the offer is false.",
                    4,
                    Correct("Borrowing money to join a scheme you do not understand."),
                    Wrong("Reading official risk information."),
                    Wrong("Asking questions about fees."),
                    Wrong("Taking time to verify registration.")),
                Question("What is a safer response to a sudden financial offer?",
                    "Slowing down and checking official sources reduces risk.",
                    5,
                    Correct("Verify independently and avoid sending money under pressure."),
                    Wrong("Pay first to avoid missing out."),
                    Wrong("Trust anyone who says profit is guaranteed."),
                    Wrong("Share payment PINs for approval."))
            ])
    ];

    private static QuizQuestionDefinition Question(
        string text,
        string explanation,
        int order,
        QuizOptionDefinition first,
        QuizOptionDefinition second,
        QuizOptionDefinition third,
        QuizOptionDefinition fourth)
    {
        return new QuizQuestionDefinition(text, explanation, order, [first, second, third, fourth]);
    }

    private static QuizOptionDefinition Correct(string text)
    {
        return new QuizOptionDefinition(text, true);
    }

    private static QuizOptionDefinition Wrong(string text)
    {
        return new QuizOptionDefinition(text, false);
    }
}

public sealed record QuizDefinition(
    string CategoryName,
    string Title,
    string Description,
    IReadOnlyList<QuizQuestionDefinition> Questions)
{
    public Quiz Create(FraudCategory category)
    {
        return new Quiz
        {
            Title = Title,
            Description = Description,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            FraudCategory = category,
            Questions = Questions.Select(question => question.Create()).ToList()
        };
    }
}

public sealed record QuizQuestionDefinition(
    string QuestionText,
    string Explanation,
    int Order,
    IReadOnlyList<QuizOptionDefinition> Options)
{
    public QuizQuestion Create()
    {
        return new QuizQuestion
        {
            QuestionText = QuestionText,
            Explanation = Explanation,
            Order = Order,
            Options = Options.Select((option, index) => option.Create(index + 1)).ToList()
        };
    }
}

public sealed record QuizOptionDefinition(string OptionText, bool IsCorrect)
{
    public QuizOption Create(int order)
    {
        return new QuizOption
        {
            OptionText = OptionText,
            IsCorrect = IsCorrect,
            Order = order
        };
    }
}
