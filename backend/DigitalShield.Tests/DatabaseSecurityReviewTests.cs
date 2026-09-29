using DigitalShield.API.Data;
using DigitalShield.API.DTOs.Fraud;
using DigitalShield.API.DTOs.User;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.Tests;

public class DatabaseSecurityReviewTests
{
    [Fact]
    [Trait("Category", "Security")]
    public void PersistenceModel_DoesNotPersistTokensOrFraudSubmissions()
    {
        using var context = CreateContext();
        var persistedTypes = context.Model.GetEntityTypes()
            .Select(entityType => entityType.ClrType)
            .ToList();

        Assert.DoesNotContain(typeof(FraudCheckRequestDto), persistedTypes);
        Assert.DoesNotContain(typeof(FraudAnalysisResponseDto), persistedTypes);
        Assert.DoesNotContain(typeof(LoginResponseDto), persistedTypes);
    }

    [Fact]
    [Trait("Category", "Security")]
    public void PersistenceModel_StoresPasswordHashesWithoutPlainPasswordOrTokenColumns()
    {
        using var context = CreateContext();
        var propertyNames = context.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(nameof(User.PasswordHash), propertyNames);
        Assert.DoesNotContain("Password", propertyNames);
        Assert.DoesNotContain("AccessToken", propertyNames);
        Assert.DoesNotContain("RefreshToken", propertyNames);
        Assert.DoesNotContain("AuthorizationHeader", propertyNames);
        Assert.DoesNotContain("SecretKey", propertyNames);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
