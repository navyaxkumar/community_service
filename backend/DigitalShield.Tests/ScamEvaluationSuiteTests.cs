using DigitalShield.API.DTOs.Fraud;
using DigitalShield.API.Fraud.Analyzer;
using DigitalShield.API.Fraud.Recommendations;
using DigitalShield.API.Fraud.Rules;
using DigitalShield.API.Fraud.Scoring;
using DigitalShield.API.Fraud.ThreatIntelligence;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Services;
using Xunit.Abstractions;

namespace DigitalShield.Tests;

public class ScamEvaluationSuiteTests
{
    private readonly ITestOutputHelper _output;
    private readonly IFraudAnalysisService _service;

    public ScamEvaluationSuiteTests(ITestOutputHelper output)
    {
        _output = output;

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

    public record LabeledCase(
        string Id,
        string Category,
        string InputType,
        string Content,
        string? Url,
        bool? SenderKnown,
        bool ExpectedIsFraud,
        string Description
    );

    public static readonly List<LabeledCase> Dataset = new()
    {
        // -------------------------------------------------------------
        // Category 1: Credential Theft & OTP Harvesting (Fraud)
        // -------------------------------------------------------------
        new("OTP-01", "CredentialTheft_OTP", "Message", "Dear customer, your SBI card is blocked. Please share the 6-digit OTP received on your mobile to unblock it immediately.", null, false, true, "Direct OTP request under threat of card blockage"),
        new("OTP-02", "CredentialTheft_OTP", "Message", "URGENT: Verify your netbanking password and OTP to stop unauthorized transaction of Rs 45,000.", null, false, true, "Urgent password and OTP request citing unauthorized transaction"),
        new("OTP-03", "CredentialTheft_OTP", "Message", "Enter your debit card PIN and CVV to claim your annual credit card reward points.", null, false, true, "Harvesting PIN and CVV for reward claim"),
        new("OTP-04", "CredentialTheft_OTP", "Message", "Please tell me the one time password sent to your phone so I can verify your delivery.", null, false, true, "Delivery agent asking for one time password"),
        new("OTP-05", "CredentialTheft_OTP", "Message", "Your net banking profile password has expired. Reply with your username, password, and security answer to reactivate.", null, false, true, "Profile reactivation password harvesting"),
        new("OTP-06", "CredentialTheft_OTP", "Message", "Customer verification: share your 6-digit verification code immediately or account will be suspended today.", null, false, true, "Verification code demand with account suspension threat"),

        // -------------------------------------------------------------
        // Category 2: Banking & KYC Account Threats (Fraud)
        // -------------------------------------------------------------
        new("BNK-01", "Banking_Threat", "Message", "Your HDFC Bank account KYC has expired. Your account will be suspended within 24 hours. Update KYC now at http://hdfc-kyc-portal.net", null, false, true, "Impersonation of HDFC with KYC expiry threat and phishing link"),
        new("BNK-02", "Banking_Threat", "Message", "State Bank of India alert: Your account has been locked due to suspicious activity. Visit http://sbi-login-verify.com immediately.", null, false, true, "SBI lock threat with lookalike link"),
        new("BNK-03", "Banking_Threat", "Message", "Income Tax department warning: Outstanding penalty of Rs 15,000 detected. Pay immediately to avoid asset arrest.", null, false, true, "Income tax authority impersonation with arrest threat and payment demand"),
        new("BNK-04", "Banking_Threat", "Message", "Electricity Board notice: Your power supply will be disconnected tonight at 9:30 PM due to unpaid bill. Call officer urgently.", null, false, true, "Power disconnection threat with urgent contact"),
        new("BNK-05", "Banking_Threat", "Message", "ICICI Bank notice: Your savings account has been deactivated. Complete KYC immediately to restore transactions.", null, false, true, "ICICI deactivation notice with KYC demand"),

        // -------------------------------------------------------------
        // Category 3: UPI & QR Code Payment Diversion (Fraud)
        // -------------------------------------------------------------
        new("UPI-01", "UPI_QRDiversion", "Message", "Congratulations! You won a lottery cashback of Rs 5,000. Scan this QR code and enter your UPI PIN to receive money.", null, false, true, "UPI PIN entry for receiving money via QR code"),
        new("UPI-02", "UPI_QRDiversion", "Message", "To accept your refund of Rs 1,200, please scan this QR code and authorize payment in your Google Pay app.", null, false, true, "QR scan authorization deceptive refund"),
        new("UPI-03", "UPI_QRDiversion", "Message", "Buyer sent payment via QR code. Scan QR code and enter UPI PIN to approve deposit into your account.", null, false, true, "Marketplace buyer QR payment diversion trap"),
        new("UPI-04", "UPI_QRDiversion", "Message", "Scan QR code to credit cashback reward of Rs 3,500 into your PhonePe wallet immediately.", null, false, true, "PhonePe cashback QR credit trap"),
        new("UPI-05", "UPI_QRDiversion", "Message", "Claim your scratch card prize of Rs 9,999. Scan the attached barcode and enter your secret MPIN to deposit funds.", null, false, true, "Scratch card prize MPIN entry claim"),

        // -------------------------------------------------------------
        // Category 4: Fake Jobs & Task Deposit Scams (Fraud)
        // -------------------------------------------------------------
        new("JOB-01", "Job_TaskDeposit", "Message", "Part-time job offer! Earn Rs 3000 to Rs 8000 daily by liking YouTube videos and rating hotels on Google. Pay Rs 500 registration fee to start today.", null, false, true, "YouTube rating job with upfront registration fee"),
        new("JOB-02", "Job_TaskDeposit", "Message", "Work from home online job: earn daily Rs 5000 with telegram tasks. Refundable security deposit of Rs 1000 required to unlock tasks.", null, false, true, "Telegram tasks with refundable security deposit"),
        new("JOB-03", "Job_TaskDeposit", "Message", "Exciting appointment offer at global firm! Pay Rs 1,500 training charge and processing fee to receive joining letter.", null, false, true, "Upfront training charge for appointment letter"),
        new("JOB-04", "Job_TaskDeposit", "Message", "Earn Rs 1000 per hour typing captchas online. Security deposit is required before work allocation.", null, false, true, "Typing job requiring security deposit"),
        new("JOB-05", "Job_TaskDeposit", "Message", "Amazon product rating job! Work 1 hour per day and earn Rs 4000 daily. Pay processing charge to activate vendor account.", null, false, true, "Product rating job with vendor processing charge"),

        // -------------------------------------------------------------
        // Category 5: Package Delivery & Customs Scams (Fraud)
        // -------------------------------------------------------------
        new("PKG-01", "Package_CustomsScam", "Message", "India Post alert: Your parcel could not be delivered due to incomplete address. Pay clearance fee of Rs 48 at http://indiapost-redelivery.com", null, false, true, "India post unpaid clearance fee with phishing link"),
        new("PKG-02", "Package_CustomsScam", "Message", "DHL Express: Your international package is held at customs department. Pay clearance fee immediately to prevent parcel seizure.", null, false, true, "DHL international package customs fee seizure threat"),
        new("PKG-03", "Package_CustomsScam", "Message", "FedEx notice: Unpaid parcel shipment fee required. Your shipment will be returned to sender unless paid within 12 hours.", null, false, true, "FedEx unpaid shipment fee under 12h urgency"),
        new("PKG-04", "Package_CustomsScam", "Message", "Customs department warning: Illegal package intercepted in your name. Pay fine immediately or warrant will be issued.", null, false, true, "Customs department illegal package arrest fine coercion"),
        new("PKG-05", "Package_CustomsScam", "Message", "Your parcel is locked at courier hub. Pay redelivery charge of Rs 99 at http://track-parcel-courier.net to avoid cancellation.", null, false, true, "Locked parcel redelivery charge on lookalike domain"),

        // -------------------------------------------------------------
        // Category 6: Lottery, Crypto & Prize Claims (Fraud)
        // -------------------------------------------------------------
        new("PRZ-01", "Lottery_PrizeScam", "Message", "Congratulations! Your mobile number was selected in KBC Lucky Draw 2026. You have won Rs 25,00,000 cash prize. Contact manager on WhatsApp.", null, false, true, "KBC lottery cash prize notification"),
        new("PRZ-02", "Lottery_PrizeScam", "Message", "You have won a free iPhone 15 Pro and a Rs 50,000 gift card! Claim your prize immediately at http://claim-free-iphone-reward.com", null, false, true, "Free gadget prize claim on phishing site"),
        new("PRZ-03", "Lottery_PrizeScam", "Message", "Dear winner, you have been selected for an exclusive cash prize of Rs 1,00,000. Transfer processing fee of Rs 2,000 to release funds.", null, false, true, "Advance fee lottery prize transfer requirement"),
        new("PRZ-04", "Lottery_PrizeScam", "Message", "Instant crypto investment return: Double your Bitcoin in 24 hours. Send payment to our secure wallet now.", null, false, true, "Crypto doubling investment promise"),
        new("PRZ-05", "Lottery_PrizeScam", "Message", "Congratulations! Your email won Rs 10 Lakhs in international lottery. Claim your reward before deadline expires today.", null, false, true, "International lottery urgent reward claim"),

        // -------------------------------------------------------------
        // Category 7: Remote Access Hijacking (Fraud)
        // -------------------------------------------------------------
        new("RMT-01", "RemoteAccess_Hijack", "Message", "Bank technical support: Please install AnyDesk on your mobile phone and share the 9-digit code to stop unauthorized transactions.", null, false, true, "AnyDesk installation requested to stop transactions"),
        new("RMT-02", "RemoteAccess_Hijack", "Message", "Customer refund desk: Download TeamViewer QuickSupport so our executive can process your pending airline ticket refund.", null, false, true, "TeamViewer QuickSupport download for refund processing"),
        new("RMT-03", "RemoteAccess_Hijack", "Message", "Telecom KYC officer: Install RustDesk app and share screen to complete mandatory SIM card re-verification.", null, false, true, "RustDesk screen sharing for SIM verification"),
        new("RMT-04", "RemoteAccess_Hijack", "Message", "Your payment failed. Open AnyDesk and grant remote access to our support representative to unblock your money.", null, false, true, "Open AnyDesk and grant remote access to unblock funds"),
        new("RMT-05", "RemoteAccess_Hijack", "Message", "Security alert: Trojan detected on your laptop. Download UltraViewer immediately to allow Microsoft certified technician to fix.", null, false, true, "Tech support scam demanding UltraViewer download"),

        // -------------------------------------------------------------
        // Category 8: Phishing URLs & Lookalikes (Fraud)
        // -------------------------------------------------------------
        new("URL-01", "Phishing_URL", "Url", "", "http://sbi-kyc-update.com/netbanking/login", false, true, "SBI brand lookalike phishing link"),
        new("URL-02", "Phishing_URL", "Url", "", "https://google.com@phishing-target.com/secure-login", false, true, "Embedded user info credentials in authority"),
        new("URL-03", "Phishing_URL", "Url", "", "http://192.168.1.100/banking/login.html", false, true, "Raw IP address host for banking login"),
        new("URL-04", "Phishing_URL", "Url", "", "https://xn--pple-43d.example.com", false, true, "Punycode internationalized domain homograph attack"),
        new("URL-05", "Phishing_URL", "Url", "", "https://paypa1.com/cgi-bin/webscr", false, true, "Typo-squatting brand impersonation (paypa1)"),
        new("URL-06", "Phishing_URL", "Url", "", "http://login.secure.verification.portal.account.services.badsite.com", false, true, "Excessive subdomain hierarchy phishing"),

        // =============================================================
        // BENIGN / LEGITIMATE EXAMPLES (Should be classified as Non-Fraud / Low)
        // =============================================================

        // Category 9: Educational Security Warnings & Advisories (Benign)
        new("EDU-01", "Educational_Security", "Message", "Remember that your bank will never ask for your password or OTP. Stay vigilant against phishing.", null, true, false, "Consumer protection reminder regarding bank OTPs"),
        new("EDU-02", "Educational_Security", "Message", "Cybercrime awareness advisory: Never share your passwords or credentials with anyone claiming to be customer support.", null, true, false, "Advisory warning never to share credentials"),
        new("EDU-03", "Educational_Security", "Message", "How to spot fake jobs: Legitimate employers never ask candidates to pay an upfront registration fee or security deposit.", null, true, false, "Educational guide on identifying job scams"),
        new("EDU-04", "Educational_Security", "Message", "Security tip: Do not install AnyDesk or TeamViewer on instructions from strangers claiming to provide refunds.", null, true, false, "Advisory against installing remote access tools"),
        new("EDU-05", "Educational_Security", "Message", "RBI consumer education: Scanning a QR code is only for making payments, never for receiving money into your bank.", null, true, false, "Official RBI educational guidance on QR codes"),
        new("EDU-06", "Educational_Security", "Message", "Public security notice: Beware of messages promising unrealistic lottery prizes or cashback rewards.", null, true, false, "General security notice on lottery scams"),

        // Category 10: Workplace & Calendar Urgency (Benign)
        new("WRK-01", "Workplace_Urgent", "Message", "Meeting rescheduled urgently to 4 PM in room 302.", null, true, false, "Workplace meeting rescheduling notice"),
        new("WRK-02", "Workplace_Urgent", "Message", "Urgent reminder: Please submit your quarterly budget report before the end of the day today.", null, true, false, "Routine deadline reminder for quarterly budget"),
        new("WRK-03", "Workplace_Urgent", "Message", "Team lunch venue has been moved to the cafeteria due to rain.", null, true, false, "Casual internal workplace update"),
        new("WRK-04", "Workplace_Urgent", "Message", "Can you please review the attached slide deck when you get a chance? No rush.", null, true, false, "Workplace document review request"),
        new("WRK-05", "Workplace_Urgent", "Message", "Sprint retrospective starts in 10 minutes on Zoom.", null, true, false, "Agile sprint meeting announcement"),
        new("WRK-06", "Workplace_Urgent", "Message", "Please review the pull request before our deployment window tonight.", null, true, false, "Software engineer peer review request"),

        // Category 11: Legitimate Transactional Bank Alerts (Benign)
        new("TXN-01", "Legitimate_BankAlert", "Message", "Your account XX1234 was credited with INR 2,500.00 on 12-Oct-26. Clear balance: INR 45,320.00. - HDFC Bank", null, true, false, "Informational bank credit alert"),
        new("TXN-02", "Legitimate_BankAlert", "Message", "INR 450.00 spent on your ICICI Card ending 9876 at COFFEE SHOP on 10-Oct. Available limit: INR 95,000.", null, true, false, "Informational credit card purchase alert"),
        new("TXN-03", "Legitimate_BankAlert", "Message", "Dear customer, your monthly e-statement for September is ready. Download it securely from the official netbanking app.", null, true, false, "Monthly bank statement availability notification"),
        new("TXN-04", "Legitimate_BankAlert", "Message", "Your fixed deposit of INR 50,000 has been renewed for 1 year at 7.1% interest. - SBI", null, true, false, "SBI fixed deposit renewal confirmation"),
        new("TXN-05", "Legitimate_BankAlert", "Message", "Salary for October 2026 credited to your salary account. Total credited: Rs 85,000.", null, true, false, "Corporate salary credit notification"),

        // Category 12: Legitimate Utility & Delivery Notifications (Benign)
        new("DEL-01", "Legitimate_UtilityDelivery", "Message", "Your Amazon order #402-1234567-8901234 has been dispatched and will arrive tomorrow by 8 PM.", null, true, false, "Standard ecommerce package dispatch update"),
        new("DEL-02", "Legitimate_UtilityDelivery", "Message", "Your Uber ride has arrived. Driver: Ramesh in white Swift Dzire (DL 01 AB 1234).", null, true, false, "Ride-hailing arrival alert"),
        new("DEL-03", "Legitimate_UtilityDelivery", "Message", "Your flight 6E-204 from Delhi to Mumbai is on schedule. Boarding gate opens at 14:15 at Terminal 3.", null, true, false, "Flight schedule and boarding gate alert"),
        new("DEL-04", "Legitimate_UtilityDelivery", "Message", "Swiggy delivery partner is at your gate with your order from Pizza Palace.", null, true, false, "Food delivery arrival message"),
        new("DEL-05", "Legitimate_UtilityDelivery", "Message", "Thank you for paying your broadband bill of Rs 999. Transaction ID: BB123456789.", null, true, false, "Broadband bill payment receipt"),

        // Category 13: Legitimate URLs & Multi-Part Domains (Benign)
        new("URL-07", "Legitimate_URL", "Url", "", "https://www.google.com/search?q=cybersecurity+best+practices", true, false, "Official Google search URL"),
        new("URL-08", "Legitimate_URL", "Url", "", "https://portal.library.ox.ac.uk/catalog/search", true, false, "Legitimate university library portal with multi-part TLD"),
        new("URL-09", "Legitimate_URL", "Url", "", "https://github.com/dotnet/aspnetcore", true, false, "Legitimate GitHub repository URL"),
        new("URL-10", "Legitimate_URL", "Url", "", "https://developer.mozilla.org/en-US/docs/Web/HTTP", true, false, "Legitimate Mozilla Developer Network documentation"),
        new("URL-11", "Legitimate_URL", "Url", "", "https://en.wikipedia.org/wiki/Phishing", true, false, "Educational Wikipedia page on phishing"),
        new("URL-12", "Legitimate_URL", "Url", "", "https://www.onlinesbi.sbi/", true, false, "Official SBI banking portal domain"),

        // Category 14: Ambiguous / Empty / Minimal Content (Benign or Inconclusive)
        new("AMB-01", "Ambiguous_Content", "Message", "Hello, how are you doing today?", null, true, false, "Everyday polite greeting with no fraud intent"),
        new("AMB-02", "Ambiguous_Content", "Message", "Thanks for your help yesterday!", null, true, false, "Polite message of gratitude"),
        new("AMB-03", "Ambiguous_Content", "Message", "Ok sounds good, see you tomorrow.", null, true, false, "Routine benign confirmation")
    };

    [Fact]
    public void RunCompleteEvaluationSuite_MeetsHighPrecisionRecallThresholds()
    {
        int truePositives = 0;
        int falsePositives = 0;
        int trueNegatives = 0;
        int falseNegatives = 0;

        var categoryStats = new Dictionary<string, (int Total, int Correct)>();
        var misclassifications = new List<string>();

        foreach (var item in Dataset)
        {
            var request = new FraudCheckRequestDto
            {
                Type = item.InputType,
                Content = item.Content,
                Url = item.Url,
                SenderKnown = item.SenderKnown
            };

            var result = _service.Analyze(request);

            // A prediction is considered Fraud if RiskLevel is Suspicious (Medium), High, or Critical
            bool predictedFraud = result.RiskLevel is "Suspicious" or "High" or "Critical";

            bool isCorrect = predictedFraud == item.ExpectedIsFraud;

            if (item.ExpectedIsFraud && predictedFraud) truePositives++;
            else if (!item.ExpectedIsFraud && predictedFraud) falsePositives++;
            else if (!item.ExpectedIsFraud && !predictedFraud) trueNegatives++;
            else if (item.ExpectedIsFraud && !predictedFraud) falseNegatives++;

            if (!categoryStats.ContainsKey(item.Category))
                categoryStats[item.Category] = (0, 0);

            var current = categoryStats[item.Category];
            categoryStats[item.Category] = (current.Total + 1, current.Correct + (isCorrect ? 1 : 0));

            if (!isCorrect)
            {
                misclassifications.Add($"[{item.Id} - {item.Category}] Expected: {(item.ExpectedIsFraud ? "FRAUD" : "BENIGN")}, " +
                                       $"Actual: {result.RiskLevel} (Score: {result.Score}). Description: {item.Description}. Reason: {result.RiskReason}");
            }
        }

        int totalEvaluated = Dataset.Count;
        double precision = (truePositives + falsePositives) > 0
            ? (double)truePositives / (truePositives + falsePositives)
            : 0.0;
        double recall = (truePositives + falseNegatives) > 0
            ? (double)truePositives / (truePositives + falseNegatives)
            : 0.0;
        double f1Score = (precision + recall) > 0
            ? 2.0 * (precision * recall) / (precision + recall)
            : 0.0;
        double accuracy = (double)(truePositives + trueNegatives) / totalEvaluated;
        double specificity = (trueNegatives + falsePositives) > 0
            ? (double)trueNegatives / (trueNegatives + falsePositives)
            : 0.0;

        // Output detailed report
        _output.WriteLine("===============================================================================");
        _output.WriteLine("           DIGITALSHIELD SCAM DETECTION EVALUATION SUITE REPORT                ");
        _output.WriteLine("===============================================================================");
        _output.WriteLine($"Total Labeled Cases Evaluated: {totalEvaluated}");
        _output.WriteLine($"Fraud Ground Truth:            {truePositives + falseNegatives}");
        _output.WriteLine($"Benign Ground Truth:           {trueNegatives + falsePositives}");
        _output.WriteLine("-------------------------------------------------------------------------------");
        _output.WriteLine("CONFUSION MATRIX:");
        _output.WriteLine($"  [True Positives  (TP)]: {truePositives,3}  |  [False Positives (FP)]: {falsePositives,3}");
        _output.WriteLine($"  [False Negatives (FN)]: {falseNegatives,3}  |  [True Negatives  (TN)]: {trueNegatives,3}");
        _output.WriteLine("-------------------------------------------------------------------------------");
        _output.WriteLine("PERFORMANCE METRICS:");
        _output.WriteLine($"  Precision:   {precision,6:P2}  (Target: >= 90.0%)");
        _output.WriteLine($"  Recall:      {recall,6:P2}  (Target: >= 90.0%)");
        _output.WriteLine($"  F1 Score:    {f1Score,6:P2}  (Target: >= 90.0%)");
        _output.WriteLine($"  Accuracy:    {accuracy,6:P2}");
        _output.WriteLine($"  Specificity: {specificity,6:P2}");
        _output.WriteLine("-------------------------------------------------------------------------------");
        _output.WriteLine("CATEGORY BREAKDOWN:");
        foreach (var (category, stats) in categoryStats.OrderBy(k => k.Key))
        {
            double catPct = (double)stats.Correct / stats.Total * 100.0;
            _output.WriteLine($"  {category,-28}: {stats.Correct}/{stats.Total} ({catPct:F1}%)");
        }

        if (misclassifications.Count > 0)
        {
            _output.WriteLine("-------------------------------------------------------------------------------");
            _output.WriteLine("MISCLASSIFICATIONS:");
            foreach (var err in misclassifications)
            {
                _output.WriteLine("  * " + err);
            }
        }
        _output.WriteLine("===============================================================================");

        // Quality Gate Assertions
        Assert.True(totalEvaluated >= 50, "Evaluation suite must contain at least 50 labeled cases.");
        Assert.True(precision >= 0.90, $"Precision must meet or exceed 90.0%. Actual: {precision:P2}");
        Assert.True(recall >= 0.90, $"Recall must meet or exceed 90.0%. Actual: {recall:P2}");
        Assert.True(f1Score >= 0.90, $"F1 Score must meet or exceed 90.0%. Actual: {f1Score:P2}");
    }
}