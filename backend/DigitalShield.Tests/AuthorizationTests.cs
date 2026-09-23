using System.Security.Claims;
using DigitalShield.API.Authorization;
using DigitalShield.API.Constants;
using DigitalShield.API.Data;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;
using DigitalShield.API.Repositories;
using DigitalShield.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalShield.Tests;

public class AuthorizationTests
{
    [Fact]
    public async Task AdminPolicy_AllowsAdminAndRejectsUserAndAnonymous()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole(Roles.Admin));
        });
        await using var provider = services.BuildServiceProvider();
        var authorization = provider.GetRequiredService<IAuthorizationService>();

        var admin = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, Roles.Admin) }, "TestAuth"));
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Role, Roles.User) }, "TestAuth"));
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.True((await authorization.AuthorizeAsync(admin, null, "Admin")).Succeeded);
        Assert.False((await authorization.AuthorizeAsync(user, null, "Admin")).Succeeded);
        Assert.False((await authorization.AuthorizeAsync(anonymous, null, "Admin")).Succeeded);
    }

    [Fact]
    public void CurrentUserService_ReadsIdentityAndRoleClaims()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "42"),
                new Claim(ClaimTypes.Email, "user@example.com"),
                new Claim(ClaimTypes.Role, Roles.Admin)
            }, "TestAuth"))
        };
        var service = new CurrentUserService(new HttpContextAccessor { HttpContext = context });

        Assert.True(service.IsAuthenticated);
        Assert.Equal(42, service.UserId);
        Assert.Equal("user@example.com", service.Email);
        Assert.Equal(Roles.Admin, service.Role);
        Assert.True(service.IsAdmin);
    }

    [Fact]
    public async Task UserProgressService_RejectsAccessToAnotherUsersProgress()
    {
        await using var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        var module = new LearningModule { Title = "Safety", Content = "Content" };
        context.LearningModules.Add(module);
        await context.SaveChangesAsync();

        var currentUser = new TestCurrentUserService(1, Roles.User);
        var service = new UserProgressService(
            new LearningModuleRepository(context),
            new UserProgressRepository(context),
            currentUser);
        await service.CreateAsync(1, new DigitalShield.API.DTOs.Progress.CreateUserProgressDto
        {
            LearningModuleId = module.Id,
            ProgressPercentage = 25
        });

        currentUser.SetUser(2, Roles.User);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.GetByUserAsync(1));
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public TestCurrentUserService(int userId, string role)
        {
            SetUser(userId, role);
        }

        public int? UserId { get; private set; }
        public string? Email => null;
        public string? Role { get; private set; }
        public bool IsAuthenticated => UserId.HasValue;
        public bool IsAdmin => Role == Roles.Admin;

        public void SetUser(int userId, string role)
        {
            UserId = userId;
            Role = role;
        }
    }
}
