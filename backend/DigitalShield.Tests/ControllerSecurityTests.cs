using DigitalShield.API.Constants;
using DigitalShield.API.Controllers;
using DigitalShield.API.DTOs.Progress;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace DigitalShield.Tests;

public class ControllerSecurityTests
{
    [Theory]
    [InlineData(typeof(AuthController), "api/auth")]
    [InlineData(typeof(UsersController), "api/users")]
    [InlineData(typeof(FraudCategoriesController), "api/fraud-categories")]
    [InlineData(typeof(LearningModulesController), "api/learning-modules")]
    [InlineData(typeof(ScenariosController), "api/scenarios")]
    [InlineData(typeof(QuizzesController), "api/quizzes")]
    [InlineData(typeof(ProgressController), "api/progress")]
    [InlineData(typeof(QuizAttemptsController), "api/quiz-attempts")]
    [InlineData(typeof(BadgesController), "api/badges")]
    [InlineData(typeof(HealthController), "api")]
    public void ControllersUseApiControllerAndExpectedRoute(Type controllerType, string route)
    {
        Assert.NotNull(controllerType.GetCustomAttribute<ApiControllerAttribute>());
        Assert.Equal(route, controllerType.GetCustomAttribute<RouteAttribute>()?.Template);
    }

    [Fact]
    public void AuthenticationAndHealthActionsAreExplicitlyAnonymous()
    {
        Assert.All(typeof(AuthController).GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(method => method.Name is "Login" or "Register"), method =>
            Assert.NotNull(method.GetCustomAttribute<AllowAnonymousAttribute>()));

        var healthMethod = typeof(HealthController).GetMethod(nameof(HealthController.GetHealth));
        Assert.NotNull(healthMethod?.GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Theory]
    [InlineData(typeof(FraudCategoriesController), "Create")]
    [InlineData(typeof(FraudCategoriesController), "Update")]
    [InlineData(typeof(LearningModulesController), "Create")]
    [InlineData(typeof(LearningModulesController), "Update")]
    [InlineData(typeof(ScenariosController), "Create")]
    [InlineData(typeof(ScenariosController), "Update")]
    [InlineData(typeof(QuizzesController), "Create")]
    [InlineData(typeof(QuizzesController), "Update")]
    [InlineData(typeof(BadgesController), "Create")]
    [InlineData(typeof(BadgesController), "Update")]
    public void ContentManagementActionsRequireAdmin(Type controllerType, string actionName)
    {
        var action = controllerType.GetMethod(actionName);
        var authorize = action?.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal(Roles.Admin, authorize!.Roles);
    }

    [Theory]
    [InlineData(typeof(UsersController))]
    [InlineData(typeof(ProgressController))]
    [InlineData(typeof(QuizAttemptsController))]
    [InlineData(typeof(BadgesController))]
    public void UserAndPrivateControllersRequireAuthentication(Type controllerType)
    {
        Assert.NotNull(controllerType.GetCustomAttribute<AuthorizeAttribute>());
    }

    [Fact]
    public void UserOwnedRoutesUseCurrentUserEndpoints()
    {
        Assert.NotNull(typeof(UsersController).GetMethod(nameof(UsersController.GetCurrentUser)));
        Assert.NotNull(typeof(ProgressController).GetMethod(nameof(ProgressController.GetCurrentUserProgress)));
        Assert.NotNull(typeof(QuizAttemptsController).GetMethod(nameof(QuizAttemptsController.GetCurrentUserAttempts)));
        Assert.Null(typeof(ProgressController).GetMethod("GetByUserId"));
        Assert.Null(typeof(QuizAttemptsController).GetMethod("GetByUserId"));
    }

    [Fact]
    public void UserOwnedRequestDtosDoNotAcceptClientUserIdOrTrustedResults()
    {
        Assert.Null(typeof(UpdateUserDto).GetProperty("Role"));
        Assert.Null(typeof(UpdateUserDto).GetProperty("UserId"));
        Assert.Null(typeof(CreateUserProgressDto).GetProperty("UserId"));
        Assert.Null(typeof(SubmitQuizAttemptDto).GetProperty("UserId"));
        Assert.Null(typeof(SubmitQuizAttemptDto).GetProperty("Score"));
        Assert.Null(typeof(SubmitQuizAttemptDto).GetProperty("TotalQuestions"));
    }
}
