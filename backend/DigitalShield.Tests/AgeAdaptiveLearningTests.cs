using DigitalShield.API.Constants;
using DigitalShield.API.Data;
using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.DTOs.Scenario;
using DigitalShield.API.Helpers;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using DigitalShield.API.Services;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.Tests;

public class AgeAdaptiveLearningTests
{
    [Fact]
    public void ContentMetadataParser_ParsesCommentJsonMetadata()
    {
        const string content = """
            <!--digitalshield-metadata:{"audience":"EarlyLearners","difficulty":"Beginner","duration":2,"objectives":"Learn password safety.","safetyTips":"Never share."}-->
            # Keep your passwords safe!
            Tell a trusted adult.
            """;

        var meta = ContentMetadataParser.Parse(content);

        Assert.Equal(AudienceConstants.EarlyLearners, meta.TargetAudience);
        Assert.Equal("Beginner", meta.DifficultyLevel);
        Assert.Equal(2, meta.EstimatedMinutes);
        Assert.Equal("Learn password safety.", meta.LearningObjectives);
        Assert.Equal("Never share.", meta.SafetyTips);

        var stripped = ContentMetadataParser.StripMetadata(content);
        Assert.DoesNotContain("digitalshield-metadata", stripped);
        Assert.Contains("Tell a trusted adult.", stripped);
    }

    [Fact]
    public void ContentMetadataParser_ParsesYamlFrontmatter()
    {
        const string content = """
            ---
            targetAudience: Professionals
            difficultyLevel: Advanced
            estimatedMinutes: 6
            learningObjectives: Detect BEC and wire redirection.
            safetyTips: Out-of-band verification.
            ---
            Executive impersonation poses severe enterprise threats.
            """;

        var meta = ContentMetadataParser.Parse(content);

        Assert.Equal(AudienceConstants.Professionals, meta.TargetAudience);
        Assert.Equal("Advanced", meta.DifficultyLevel);
        Assert.Equal(6, meta.EstimatedMinutes);
        Assert.Equal("Detect BEC and wire redirection.", meta.LearningObjectives);
        Assert.Equal("Out-of-band verification.", meta.SafetyTips);
    }

    [Fact]
    public void ContentMetadataParser_ParsesDescriptionBracketTags()
    {
        const string desc = "[Audience: Seniors | Difficulty: Beginner | Time: 4 mins] Calm telephone verification guidance.";

        var meta = ContentMetadataParser.Parse(string.Empty, desc);

        Assert.Equal(AudienceConstants.Seniors, meta.TargetAudience);
        Assert.Equal("Beginner", meta.DifficultyLevel);
        Assert.Equal(4, meta.EstimatedMinutes);

        var clean = ContentMetadataParser.CleanDescription(desc);
        Assert.Equal("Calm telephone verification guidance.", clean);
    }

    [Fact]
    public void ContentMetadataParser_InfersAudienceFromKeywords()
    {
        var earlyMeta = ContentMetadataParser.Parse("Play a fun game with smiling balloons and tell your mom or grown-up.");
        Assert.Equal(AudienceConstants.EarlyLearners, earlyMeta.TargetAudience);

        var childMeta = ContentMetadataParser.Parse("Click here for 10,000 free Robux game coins and skins.");
        Assert.Equal(AudienceConstants.Children, childMeta.TargetAudience);

        var teenMeta = ContentMetadataParser.Parse("Brand ambassador collab on Instagram DM.");
        Assert.Equal(AudienceConstants.Teens, teenMeta.TargetAudience);

        var youngAdultMeta = ContentMetadataParser.Parse("Work-from-home internship registration fee.");
        Assert.Equal(AudienceConstants.YoungAdults, youngAdultMeta.TargetAudience);

        var seniorMeta = ContentMetadataParser.Parse("Pension and electricity bill disconnection threat call.");
        Assert.Equal(AudienceConstants.Seniors, seniorMeta.TargetAudience);

        var proMeta = ContentMetadataParser.Parse("Business email compromise and MFA fatigue push bombing attack.");
        Assert.Equal(AudienceConstants.Professionals, proMeta.TargetAudience);
    }

    [Fact]
    public void AudienceConstants_MatchesAudienceCorrectly()
    {
        Assert.True(AudienceConstants.MatchesAudience(AudienceConstants.All, AudienceConstants.EarlyLearners));
        Assert.True(AudienceConstants.MatchesAudience(AudienceConstants.EarlyLearners, AudienceConstants.EarlyLearners));
        Assert.True(AudienceConstants.MatchesAudience(AudienceConstants.EarlyLearners, "kids57"));
        Assert.True(AudienceConstants.MatchesAudience(AudienceConstants.Seniors, "elderly"));
        Assert.True(AudienceConstants.MatchesAudience(AudienceConstants.Professionals, "enterprise"));
        Assert.True(AudienceConstants.MatchesAudience(AudienceConstants.Professionals, null));
        Assert.True(AudienceConstants.MatchesAudience(AudienceConstants.Professionals, AudienceConstants.All));

        Assert.False(AudienceConstants.MatchesAudience(AudienceConstants.EarlyLearners, AudienceConstants.Professionals));
        Assert.False(AudienceConstants.MatchesAudience(AudienceConstants.Seniors, AudienceConstants.Teens));
    }

    [Fact]
    public async Task LearningModules_AudienceFilter_ReturnsRelevantAndUniversalContent()
    {
        await using var context = CreateContext();
        var catRepo = new FraudCategoryRepository(context);
        var modRepo = new LearningModuleRepository(context);
        var service = new LearningModuleService(catRepo, modRepo);

        var cat = new FraudCategory { Name = "Adaptive Category", Description = "Testing" };
        context.FraudCategories.Add(cat);
        await context.SaveChangesAsync();

        // 1. Universal module
        await service.CreateAsync(new CreateLearningModuleDto
        {
            Title = "Universal Digital Basics",
            Content = "Everyone should know digital basics.",
            IsPublished = true,
            TargetAudience = AudienceConstants.All,
            FraudCategoryId = cat.Id
        });

        // 2. Early learners module
        await service.CreateAsync(new CreateLearningModuleDto
        {
            Title = "Fun Balloon Safety",
            Content = "Balloons and smiles for young kids.",
            IsPublished = true,
            TargetAudience = AudienceConstants.EarlyLearners,
            FraudCategoryId = cat.Id
        });

        // 3. Professional module
        await service.CreateAsync(new CreateLearningModuleDto
        {
            Title = "Enterprise Wire Controls",
            Content = "Dual-control ACH authorization procedures.",
            IsPublished = true,
            TargetAudience = AudienceConstants.Professionals,
            FraudCategoryId = cat.Id
        });

        // Query for EarlyLearners
        var earlyResults = await service.GetPublishedAsync(AudienceConstants.EarlyLearners);
        Assert.Equal(2, earlyResults.Count);
        Assert.Contains(earlyResults, m => m.Title == "Fun Balloon Safety");
        Assert.Contains(earlyResults, m => m.Title == "Universal Digital Basics");
        Assert.DoesNotContain(earlyResults, m => m.Title == "Enterprise Wire Controls");

        // Query for Professionals
        var proResults = await service.GetPublishedAsync(AudienceConstants.Professionals);
        Assert.Equal(2, proResults.Count);
        Assert.Contains(proResults, m => m.Title == "Enterprise Wire Controls");
        Assert.Contains(proResults, m => m.Title == "Universal Digital Basics");
        Assert.DoesNotContain(proResults, m => m.Title == "Fun Balloon Safety");

        // Query All / null
        var allResults = await service.GetPublishedAsync(null);
        Assert.Equal(3, allResults.Count);
    }

    [Fact]
    public async Task Scenarios_AudienceFilter_ProtectsAudienceSegmentation()
    {
        await using var context = CreateContext();
        var catRepo = new FraudCategoryRepository(context);
        var scRepo = new ScenarioRepository(context);
        var service = new ScenarioService(catRepo, scRepo);

        var cat = new FraudCategory { Name = "Scenario Category", Description = "Testing" };
        context.FraudCategories.Add(cat);
        await context.SaveChangesAsync();

        await service.CreateAsync(new CreateScenarioDto
        {
            Title = "Senior Bill Call",
            Situation = "Caller asks elderly person for power bill.",
            CorrectAction = "Hang up calmly.",
            IsPublished = true,
            TargetAudience = AudienceConstants.Seniors,
            FraudCategoryId = cat.Id
        });

        await service.CreateAsync(new CreateScenarioDto
        {
            Title = "Teen TikTok Collab",
            Situation = "DM asks teenager to click link.",
            CorrectAction = "Do not open link.",
            IsPublished = true,
            TargetAudience = AudienceConstants.Teens,
            FraudCategoryId = cat.Id
        });

        var seniorScenarios = await service.GetPublishedAsync(AudienceConstants.Seniors);
        Assert.Single(seniorScenarios);
        Assert.Equal("Senior Bill Call", seniorScenarios[0].Title);

        var teenScenarios = await service.GetPublishedAsync(AudienceConstants.Teens);
        Assert.Single(teenScenarios);
        Assert.Equal("Teen TikTok Collab", teenScenarios[0].Title);
    }

    [Fact]
    public async Task Quizzes_AudienceFilter_ReturnsMatchingAndUniversalQuizzes()
    {
        await using var context = CreateContext();
        var catRepo = new FraudCategoryRepository(context);
        var quizRepo = new QuizRepository(context);
        var service = new QuizService(catRepo, quizRepo);

        var cat = new FraudCategory { Name = "Quiz Category", Description = "Testing" };
        context.FraudCategories.Add(cat);
        await context.SaveChangesAsync();

        await service.CreateAsync(new CreateQuizDto
        {
            Title = "Kid Star Quiz",
            Description = "Fun safety questions for kids.",
            IsPublished = true,
            TargetAudience = AudienceConstants.Children,
            FraudCategoryId = cat.Id,
            Questions =
            [
                new CreateQuizQuestionDto
                {
                    QuestionText = "Is this safe?",
                    Order = 1,
                    Options =
                    [
                        new CreateQuizOptionDto { OptionText = "Yes", IsCorrect = true, Order = 1 },
                        new CreateQuizOptionDto { OptionText = "No", IsCorrect = false, Order = 2 }
                    ]
                }
            ]
        });

        await service.CreateAsync(new CreateQuizDto
        {
            Title = "Workplace Risk Quiz",
            Description = "Advanced corporate assessment.",
            IsPublished = true,
            TargetAudience = AudienceConstants.Professionals,
            FraudCategoryId = cat.Id,
            Questions =
            [
                new CreateQuizQuestionDto
                {
                    QuestionText = "Is wire redirect valid?",
                    Order = 1,
                    Options =
                    [
                        new CreateQuizOptionDto { OptionText = "No", IsCorrect = true, Order = 1 },
                        new CreateQuizOptionDto { OptionText = "Yes", IsCorrect = false, Order = 2 }
                    ]
                }
            ]
        });

        var childQuizzes = await service.GetPublishedAsync(AudienceConstants.Children);
        Assert.Single(childQuizzes);
        Assert.Equal("Kid Star Quiz", childQuizzes[0].Title);

        var proQuizzes = await service.GetPublishedAsync(AudienceConstants.Professionals);
        Assert.Single(proQuizzes);
        Assert.Equal("Workplace Risk Quiz", proQuizzes[0].Title);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
