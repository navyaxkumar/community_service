using DigitalShield.API.Data;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.Tests;

public class DatabaseIndexTests
{
    [Fact]
    public void ApplicationModel_HasIndexesForFrequentFilterAndOrderingPatterns()
    {
        using var context = CreateContext();

        AssertIndex<LearningModule>(
            context,
            nameof(LearningModule.FraudCategoryId),
            nameof(LearningModule.Order));
        AssertIndex<Scenario>(
            context,
            nameof(Scenario.FraudCategoryId),
            nameof(Scenario.Title));
        AssertIndex<Quiz>(
            context,
            nameof(Quiz.FraudCategoryId),
            nameof(Quiz.Title));
        AssertIndex<UserProgress>(
            context,
            nameof(UserProgress.UserId),
            nameof(UserProgress.StartedAt));
        AssertIndex<QuizAttempt>(
            context,
            nameof(QuizAttempt.UserId),
            nameof(QuizAttempt.StartedAt));
    }

    [Fact]
    public void ApplicationModel_PreservesExistingUniqueLookupIndexes()
    {
        using var context = CreateContext();

        AssertIndex<User>(context, true, nameof(User.Email));
        AssertIndex<FraudCategory>(context, true, nameof(FraudCategory.Name));
        AssertIndex<UserProgress>(
            context,
            true,
            nameof(UserProgress.UserId),
            nameof(UserProgress.LearningModuleId));
    }

    private static void AssertIndex<TEntity>(DbContext context, params string[] propertyNames)
        where TEntity : class
    {
        AssertIndex<TEntity>(context, false, propertyNames);
    }

    private static void AssertIndex<TEntity>(DbContext context, bool isUnique, params string[] propertyNames)
        where TEntity : class
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        Assert.NotNull(entityType);

        Assert.Contains(entityType.GetIndexes(), index =>
            index.IsUnique == isUnique &&
            index.Properties.Select(property => property.Name).SequenceEqual(propertyNames));
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
