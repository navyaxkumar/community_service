using DigitalShield.API.Models;

namespace DigitalShield.API.Data.Seed;

public static class LearningModuleSeed
{
    public static IReadOnlyList<LearningModuleDefinition> All =>
    [
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Recognizing OTP Pressure Messages",
            "Learn why urgent OTP requests are risky and how to respond safely.",
            """
            Scammers often send messages that sound urgent, such as "Your account will be blocked today. Share your OTP to continue." The goal is to make you react quickly before checking whether the request is real.

            A safe response is to stop, avoid sharing the OTP, and independently contact the bank, wallet, or service using its official app or website. Real support staff should not ask for your OTP, password, PIN, or full card details.

            Warning signs include threats of account closure, requests to forward codes, links that ask you to log in again, and callers who become impatient when you ask questions.
            """,
            1),
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Checking Sender Claims Before You Act",
            "Practice verifying suspicious messages without using the sender's link.",
            """
            Fraud messages may claim to come from a bank, delivery company, government office, or payment app. They may use familiar names, logos, or formal language, but that does not prove the message is real.

            Instead of tapping the link in the message, open the official app yourself or type the known official website address manually. If you need help, use a phone number from your card, statement, or the official website.

            Avoid replying with personal details. A fictional example is: "Your wallet KYC expires tonight. Update at example-bank-login.test." Treat messages like this as suspicious until verified independently.
            """,
            2),
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "What To Do After Sharing Sensitive Information",
            "Learn calm first steps if you think you shared an OTP or password.",
            """
            If you accidentally share an OTP, password, or account detail, act quickly but calmly. Change the password from the official app or website, sign out of other sessions if available, and contact the real service provider through verified support channels.

            For payment or bank-related incidents, report the issue to your bank or payment provider immediately. Keep screenshots and message details for reporting, but do not forward the suspicious link to others.

            Do not blame yourself. Scams are designed to create pressure and confusion. The safer response is to limit further sharing and get help through trusted channels.
            """,
            3),

        new(
            FraudCategorySeed.UpiPaymentFraudName,
            "Understanding Collect Requests",
            "Learn why receiving money should not require approving a payment request.",
            """
            A common payment trick is to send a collect request and claim it is needed to receive money. In many payment apps, approving a collect request sends money out of your account.

            If someone says they are paying you, check whether the app screen says you are receiving or paying. Read every confirmation screen slowly before entering a PIN.

            A safer response is to ask the sender to use a normal payment method and wait for the credit confirmation in your own app. Do not approve requests because a caller is rushing you.
            """,
            1),
        new(
            FraudCategorySeed.UpiPaymentFraudName,
            "QR Codes Are For Paying, Not Receiving",
            "Understand how QR code confusion can lead to accidental payments.",
            """
            Scammers may say, "Scan this QR code to receive your refund." In many everyday payment flows, scanning a QR code starts a payment from you to someone else.

            Before scanning any code, ask what action the app is showing. If the screen asks for a PIN, amount confirmation, or payment approval, you may be sending money.

            Use official refund processes from trusted apps or stores. Do not scan a QR code sent by a stranger or an unverified seller to receive money.
            """,
            2),
        new(
            FraudCategorySeed.UpiPaymentFraudName,
            "Safe Payment Verification Habits",
            "Build simple habits before approving digital payments.",
            """
            Good payment habits reduce mistakes. Check the recipient name, amount, note, and payment direction before entering a PIN. If anything looks different from what you expected, cancel and verify first.

            Be careful with callers who say a transaction must be completed immediately. Urgency is often used to stop people from reading the screen.

            For large or unusual payments, confirm through a separate trusted channel. Do not rely only on screenshots from the other person because screenshots can be misleading.
            """,
            3),

        new(
            FraudCategorySeed.FakeJobScamsName,
            "Spotting Advance-Fee Job Offers",
            "Learn why real hiring should not begin with pressure to pay.",
            """
            Fake job offers may promise quick hiring, remote work, or unusually high salaries. The scam often begins when the applicant is asked to pay for registration, training, equipment, or document verification.

            Before paying anything, check the employer through official company channels. Search for the role on the company's real careers page and be cautious if the interviewer only uses personal chat accounts.

            A safer response is to ask for a written offer from a verified company email and avoid sending money or sensitive documents until the employer is confirmed.
            """,
            1),
        new(
            FraudCategorySeed.FakeJobScamsName,
            "Protecting Documents During Job Searches",
            "Understand safer ways to handle ID, resume, and bank details.",
            """
            Job seekers may be asked for documents such as identity proof, address proof, certificates, or bank details. Some of this may be needed later in a legitimate process, but early requests from unverified recruiters can be risky.

            Share only what is necessary, only after verifying the employer, and avoid sending full documents to unknown people. Consider watermarking copies for the specific purpose when appropriate.

            Do not share passwords, OTPs, card details, or remote access to your phone or computer during a job process.
            """,
            2),
        new(
            FraudCategorySeed.FakeJobScamsName,
            "Checking Recruiter Identity",
            "Learn simple checks before trusting a job contact.",
            """
            Scammers may pretend to be recruiters from well-known companies. They may use copied logos, edited offer letters, or names of real employees.

            Verify the recruiter's email domain, compare the job with the company's official careers page, and contact the company through its official website if unsure. Be cautious if the process happens only through chat and payment links.

            A real opportunity should allow reasonable time for verification. Pressure to pay immediately or keep the offer secret is a warning sign.
            """,
            3),

        new(
            FraudCategorySeed.OnlineShoppingScamsName,
            "Evaluating Too-Good-To-Be-True Deals",
            "Learn how unrealistic discounts can hide fake sellers.",
            """
            Online shopping scams often use very low prices, limited-time claims, and professional-looking product photos. The goal is to make the buyer pay before checking seller trust.

            Look for seller history, return policy, secure payment options, and independent reviews. Be careful if the seller avoids the platform's normal checkout process or asks for direct payment to a personal account.

            A safer response is to use trusted platforms, avoid rushed advance payments, and keep communication inside the official marketplace when possible.
            """,
            1),
        new(
            FraudCategorySeed.OnlineShoppingScamsName,
            "Avoiding Delivery And Refund Traps",
            "Understand fake courier fees, refund links, and parcel messages.",
            """
            Fake delivery messages may say that a parcel is held and a small fee is required. Fake refund messages may ask you to enter card, PIN, or OTP details to receive money.

            Check delivery status only through the store or courier's official app or website. Do not trust links from unexpected messages, especially if they ask for payment details.

            If you are expecting a refund, use the seller's official process. Receiving a refund should not require sharing OTPs or passwords.
            """,
            2),
        new(
            FraudCategorySeed.OnlineShoppingScamsName,
            "Safer Marketplace Conversations",
            "Learn how to keep buying and selling conversations safer.",
            """
            Fraudsters may move conversations away from the marketplace to avoid platform protections. They may send outside payment links, fake payment screenshots, or urgent pickup requests.

            Keep chats and payments inside trusted platforms when possible. Read app screens carefully and verify payment credits in your own account before handing over an item.

            If a buyer or seller becomes aggressive, secretive, or unusually rushed, pause the transaction and verify independently.
            """,
            3),

        new(
            FraudCategorySeed.FakeWebsitesLinksName,
            "Reading Links Carefully",
            "Learn simple link checks without needing technical knowledge.",
            """
            Fake links may look similar to trusted websites but include extra words, misspellings, or unfamiliar endings. A message may hide the real destination behind a button or shortened link.

            Before entering information, check whether the address matches the official service. When unsure, do not use the message link. Open the official app or type the known website yourself.

            Avoid entering passwords, OTPs, card details, or identity information on pages opened from unexpected messages.
            """,
            1),
        new(
            FraudCategorySeed.FakeWebsitesLinksName,
            "Recognizing Fake Forms",
            "Understand why forms asking for sensitive details can be risky.",
            """
            Fake forms may ask for information such as account numbers, passwords, OTPs, card details, or document photos. They may claim to be for rewards, refunds, account updates, or verification.

            Stop if a form asks for more information than seems necessary. Verify through official channels before submitting sensitive details.

            A fictional example is: "Claim your reward at rewards-support.example.test." Treat such links as unverified unless you reached them from the official service yourself.
            """,
            2),
        new(
            FraudCategorySeed.FakeWebsitesLinksName,
            "Downloading Only From Trusted Sources",
            "Learn why unexpected app or file downloads should be avoided.",
            """
            Some scams ask users to install an app, update a service, or download a file from an unknown link. This can expose personal information or give strangers access to the device.

            Use official app stores and the service provider's real website. Do not install apps sent through random messages, especially if the sender says it is required for support, refund, or verification.

            If you already installed something suspicious, disconnect from the conversation and seek help from a trusted technical support source.
            """,
            3),

        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "When Someone Pretends To Be Family Or Staff",
            "Learn how trust and urgency are used in impersonation scams.",
            """
            Scammers may pretend to be a family member, friend, bank employee, delivery worker, police officer, or company staff. They may use urgency, fear, or emotional pressure to make you act quickly.

            Pause and verify through a separate trusted channel. For example, call the person using a number already saved in your contacts, not a number from the suspicious message.

            Real verification should allow questions. Be cautious if the person refuses a callback, asks for secrecy, or demands immediate payment.
            """,
            1),
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Verifying Support Calls",
            "Learn what legitimate support should and should not ask for.",
            """
            A caller may claim they are from customer support and ask you to share your screen, install an app, reveal an OTP, or read out a password. These are serious warning signs.

            End the call and contact support through the official app or website. Do not use a phone number supplied by the suspicious caller.

            Support teams may verify identity in limited ways, but they should not need your OTP, full password, payment PIN, or remote control of your device.
            """,
            2),
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Resisting Secrecy And Pressure",
            "Understand why secrecy is often part of social engineering.",
            """
            Fraudsters may say, "Do not tell anyone," "This is an emergency," or "You will be in trouble if you delay." These lines are used to isolate the person from advice.

            A safer response is to slow down and involve a trusted family member, friend, bank, or official support channel. Taking a few minutes to verify can reduce risk.

            You are allowed to ask questions, decline, and end a conversation that feels unsafe.
            """,
            3),

        new(
            FraudCategorySeed.InvestmentLoanScamsName,
            "Questioning Guaranteed High Returns",
            "Learn why guaranteed profit claims deserve extra caution.",
            """
            Investment scams often promise quick, high, or guaranteed returns. They may show edited profit screenshots or claim that only a few seats are left.

            Before investing, check whether the provider is registered with the appropriate official authority and read independent information. Do not invest because a chat group or stranger says everyone is earning.

            No investment is risk-free. A safer response is to avoid rushing, verify independently, and never borrow money to join a scheme you do not understand.
            """,
            1),
        new(
            FraudCategorySeed.InvestmentLoanScamsName,
            "Avoiding Advance-Fee Loan Traps",
            "Understand loan offers that demand money before disbursal.",
            """
            Fake loan offers may promise instant approval but ask for processing fees, insurance charges, or verification payments before releasing funds.

            Verify the lender through official channels and read the terms carefully. Be cautious if the lender contacts you only through chat, avoids written documents, or demands repeated small payments.

            Do not share OTPs, payment PINs, or full card details to receive a loan. If a loan offer feels rushed or unclear, pause and seek advice.
            """,
            2),
        new(
            FraudCategorySeed.InvestmentLoanScamsName,
            "Checking Financial Advice Sources",
            "Learn how to treat tips from strangers, groups, and influencers.",
            """
            Fraudsters may use social media groups, fake experts, or copied screenshots to build trust. They may claim insider knowledge or say a decision must be made today.

            Check whether the advice comes from a qualified, accountable source. Compare claims with official documents and avoid sending money to personal accounts for investment access.

            A safer response is to take time, ask questions, and avoid any financial decision based only on pressure or excitement.
            """,
            3),

        // --- Age-Adaptive Curriculum: Early Learners (Ages 5–7) ---
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Secret Words and Trusted Grown-Ups",
            "[Audience: EarlyLearners | Difficulty: Beginner | Time: 2 mins] Friendly lessons on secret passwords and asking a grown-up for help.",
            """
            <!--digitalshield-metadata:{"audience":"EarlyLearners","difficulty":"Beginner","duration":2,"objectives":"Understand that passwords are like secret keys and should only be shared with trusted parents.","safetyTips":"Never tell anyone your secret word in an online game."}-->
            Never tell anyone your secret password. Passwords are like secret house keys for your games.

            If a message makes you feel worried or surprised, stop playing. Do not click any buttons.

            Always show the screen to a trusted grown-up like your mom, dad, or teacher. They will keep you safe!
            """,
            4),
        new(
            FraudCategorySeed.FakeWebsitesLinksName,
            "When a Game Asks for Your Parent's Phone",
            "[Audience: EarlyLearners | Difficulty: Beginner | Time: 2 mins] Simple rules about free prizes and popups in children's games.",
            """
            <!--digitalshield-metadata:{"audience":"EarlyLearners","difficulty":"Beginner","duration":2,"objectives":"Learn to stop and ask an adult before entering numbers or clicking popup boxes.","safetyTips":"Ask a trusted adult whenever a game promises free gifts or prizes."}-->
            Sometimes a game screen says you won a giant prize or shiny gems. Then it asks for your mom's phone number.

            Stop and pause! Real prizes do not ask for secret phone numbers.

            Turn around and ask your trusted adult before touching the screen. Good job being a safe digital explorer!
            """,
            4),

        // --- Age-Adaptive Curriculum: Children (Ages 8–12) ---
        new(
            FraudCategorySeed.OnlineShoppingScamsName,
            "Game Reward Traps & Free Coin Scams",
            "[Audience: Children | Difficulty: Beginner | Time: 3 mins] Spot fake giveaways and currency tricks in Roblox and Fortnite.",
            """
            <!--digitalshield-metadata:{"audience":"Children","difficulty":"Beginner","duration":3,"objectives":"Spot fake giveaways and game currency traps in Roblox, Fortnite, and Minecraft.","safetyTips":"Never download extra apps to get free in-game currency."}-->
            You might see videos or chat messages promising "Free 10,000 Robux or V-Bucks if you click this link."

            These are tricks called giveaway scams. Scammers want your account login, or they want you to download files that can harm your computer.

            Real game companies do not give away paid coins on random websites. If an offer sounds too good to be true, it is always a scam. Tell an adult if someone in a game pressures you.
            """,
            4),
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Secret Passwords and OTP Safety for Kids",
            "[Audience: Children | Difficulty: Beginner | Time: 3 mins] Learn why verification codes on your tablet must stay secret.",
            """
            <!--digitalshield-metadata:{"audience":"Children","difficulty":"Beginner","duration":3,"objectives":"Understand why one-time codes sent to a phone or tablet should never be forwarded.","safetyTips":"Keep your two-factor codes private from all online friends."}-->
            An OTP is a secret 4-digit or 6-digit number sent to your phone or tablet. It proves that you are the real owner.

            If an online friend says, "Send me the code you just got so I can add you to my game team," do not send it! They are trying to take over your account.

            No game moderator, friend, or support team will ever need your OTP. Keep it strictly private.
            """,
            5),

        // --- Age-Adaptive Curriculum: Teenagers (Ages 13–17) ---
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Social Media Impersonation & Fake DM Traps",
            "[Audience: Teens | Difficulty: Intermediate | Time: 4 mins] Defend against clone accounts, brand collab scams, and phishing DMs.",
            """
            <!--digitalshield-metadata:{"audience":"Teens","difficulty":"Intermediate","duration":4,"objectives":"Identify clone profiles, brand sponsorship DMs, and phishing links on Instagram, Discord, and TikTok.","safetyTips":"Never log into your social account through a link sent in a DM."}-->
            Scammers frequently copy public profiles of creators or friends, using almost identical usernames (like adding an underscore or swapping letters).

            They might send you a DM claiming: "Vote for my photography portfolio" or "You have been selected for a brand clothing sponsorship." The link opens a fake login screen designed to steal your credentials.

            Always verify suspicious requests by asking your friend on a different channel or in real life. Never enter your password on a page opened directly through a social media direct message.
            """,
            4),
        new(
            FraudCategorySeed.FakeWebsitesLinksName,
            "Gaming Account Trades & Recovery Traps",
            "[Audience: Teens | Difficulty: Intermediate | Time: 4 mins] Protect Steam and Epic accounts from trade bots and middleman tricks.",
            """
            <!--digitalshield-metadata:{"audience":"Teens","difficulty":"Intermediate","duration":4,"objectives":"Protect Steam, Epic, and Discord accounts from recovery code theft and middleman escrow tricks.","safetyTips":"Use official platform trade windows only; avoid third-party verification bots."}-->
            Account theft often happens when someone offers to buy your account or trade rare skins. They might send a link to a "trusted middleman site" or ask you to change your account email address.

            Once you change your account email or share a recovery link, the attacker locks you out permanently and sells your account inventory.

            If you ever feel pressured, threatened, or tricked, talk to a trusted adult or school counselor immediately. Do not feel ashamed—scammers exploit human trust.
            """,
            4),

        // --- Age-Adaptive Curriculum: Students & Young Adults (Ages 18–24) ---
        new(
            FraudCategorySeed.FakeJobScamsName,
            "Spotting Fake Work-From-Home & Internship Traps",
            "[Audience: YoungAdults | Difficulty: Intermediate | Time: 5 mins] Recognize advance-fee job postings and video-liking scams.",
            """
            <!--digitalshield-metadata:{"audience":"YoungAdults","difficulty":"Intermediate","duration":5,"objectives":"Evaluate student remote job offers, freelance task scams, and upfront equipment fee deception.","safetyTips":"Legitimate employers never charge application, training, or equipment verification fees."}-->
            College students and recent graduates are prime targets for employment fraud. Common patterns include unsolicited messages on WhatsApp or Telegram offering $100/day for liking YouTube videos or completing app reviews.

            The scam usually begins with a small "payout" to build confidence, followed by demanding a "refundable security deposit" to unlock higher-tier tasks. Once money is sent, the scammers disappear.

            Always cross-reference job openings on official corporate career portals. Legitimate employers will never ask you to pay money to receive a salary.
            """,
            4),
        new(
            FraudCategorySeed.UpiPaymentFraudName,
            "UPI & QR Code Traps: Receiving vs Paying",
            "[Audience: YoungAdults | Difficulty: Intermediate | Time: 5 mins] Master payment app mechanics: QR codes and PINs never receive money.",
            """
            <!--digitalshield-metadata:{"audience":"YoungAdults","difficulty":"Intermediate","duration":5,"objectives":"Understand payment app mechanics and debunk the 'scan QR to receive scholarship/refund' myth.","safetyTips":"Your UPI PIN is required ONLY to send money, never to receive funds."}-->
            Many young adults using peer-to-peer payment apps (such as Google Pay, PhonePe, or Paytm) fall for the "Collect Request" deception. A seller or buyer sends a QR code claiming, "Scan this to claim your university refund or scholarship money."

            The fundamental technical rule of digital payments: Scanning a QR code or entering a PIN is ALWAYS an outgoing transaction authorizing money to leave your account.

            To receive money, you only need to provide your UPI ID or phone number. You never need to enter your PIN, approve a notification, or scan an image.
            """,
            4),

        // --- Age-Adaptive Curriculum: Senior Citizens & First-Time Digital Users ---
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Recognizing Fake Bank & Pension Callers",
            "[Audience: Seniors | Difficulty: Beginner | Time: 4 mins] Calm, respectful guidance on telephone banking and pension scams.",
            """
            <!--digitalshield-metadata:{"audience":"Seniors","difficulty":"Beginner","duration":4,"objectives":"Learn calm, confident responses to callers claiming your bank account or pension will be suspended.","safetyTips":"Hang up and dial the official bank phone number printed on the back of your debit card."}-->
            Scammers often target seniors by pretending to be helpful bank managers, government officials, or electricity board agents. They use serious, rushed voices, claiming your account will be blocked within hours unless you verify immediately.

            Please remember: Real banks and government pension offices will NEVER call and demand your password, ATM PIN, or the SMS verification code (OTP).

            It is always safe to say, "I will check this myself," and hang up the phone. You are in complete control. Call your bank using the phone number printed clearly on the back of your debit card, or ask a trusted family member for help.
            """,
            6),
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Never Install Remote Control Apps (AnyDesk/TeamViewer)",
            "[Audience: Seniors | Difficulty: Beginner | Time: 4 mins] Understand why screen-sharing apps should never be installed for callers.",
            """
            <!--digitalshield-metadata:{"audience":"Seniors","difficulty":"Beginner","duration":4,"objectives":"Understand why callers asking you to install remote support apps on your phone or tablet are dangerous.","safetyTips":"Never download screen-sharing apps like AnyDesk, TeamViewer, or QuickSupport on instruction from a caller."}-->
            A caller may say they need to "update your digital banking app" or "fix a refund error," and ask you to install an application such as AnyDesk, TeamViewer, or RustDesk from the app store.

            These applications allow someone in another location to see everything on your phone screen and control your device, including reading your banking passwords and text messages.

            Legitimate customer support staff will never ask you to install screen-sharing software on your personal phone. If anyone asks you to do this, immediately hang up and do not open the app.
            """,
            4),

        // --- Age-Adaptive Curriculum: Professionals & Organizations ---
        new(
            FraudCategorySeed.ImpersonationSocialEngineeringName,
            "Business Email Compromise (BEC) & Vendor Wire Redirection",
            "[Audience: Professionals | Difficulty: Advanced | Time: 6 mins] Detect lookalike corporate domains, executive impersonation, and invoice tampering.",
            """
            <!--digitalshield-metadata:{"audience":"Professionals","difficulty":"Advanced","duration":6,"objectives":"Detect executive impersonation, lookalike domain spoofing, and unauthorized vendor invoice payment changes.","safetyTips":"Mandate out-of-band phone verification to a pre-established number for all banking coordinate changes."}-->
            Business Email Compromise (BEC) accounts for billions in global enterprise losses. Attackers register lookalike domains (e.g. acmne-corp.com instead of acme-corp.com) or compromise vendor mailboxes to insert fraudulent banking instructions into pending invoice threads.

            The deceptive email often cites a sudden bank audit, supplier transition, or end-of-quarter rush to circumvent normal dual-authorization protocols.

            Enterprise mitigation: Strictly enforce secondary out-of-band verification via voice call to a known, established contact before updating vendor payment details. Never use contact numbers listed solely on the updated invoice.
            """,
            5),
        new(
            FraudCategorySeed.PhishingOtpScamsName,
            "Executive Impersonation and MFA Fatigue Attacks",
            "[Audience: Professionals | Difficulty: Advanced | Time: 6 mins] Thwart push bombing, C-suite impersonation, and unauthorized credential abuse.",
            """
            <!--digitalshield-metadata:{"audience":"Professionals","difficulty":"Advanced","duration":6,"objectives":"Recognize multi-factor authentication push bombing and executive urgency coercion in enterprise communication.","safetyTips":"Report unexpected MFA push prompts immediately to your corporate Information Security Operations Center (SOC)."}-->
            Adversaries with compromised corporate passwords increasingly deploy MFA Fatigue (Push Bombing)—sending dozens of repeated login authorization prompts in the middle of the night until the employee taps "Approve" out of frustration or confusion.

            Simultaneously, threat actors impersonate C-level leadership on SMS, WhatsApp, or Slack, demanding urgent action (such as purchasing executive gift cards or routing confidential employee payroll files) while claiming to be in an unrecorded board meeting.

            Standard operational defense: Treat repeated unexpected MFA alerts as active credential compromise. Escalate immediately to IT Security and never bypass documented corporate payment authorization chains.
            """,
            7)
    ];
}

public sealed record LearningModuleDefinition(
    string CategoryName,
    string Title,
    string Description,
    string Content,
    int Order)
{
    public LearningModule Create(FraudCategory category)
    {
        return new LearningModule
        {
            Title = Title,
            Description = Description,
            Content = Content,
            Order = Order,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            FraudCategory = category
        };
    }
}
