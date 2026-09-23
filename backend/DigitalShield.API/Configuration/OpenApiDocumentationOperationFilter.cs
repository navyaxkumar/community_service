using Microsoft.AspNetCore.Authorization;
using DigitalShield.API.DTOs.Badge;
using DigitalShield.API.DTOs.Fraud;
using DigitalShield.API.DTOs.FraudCategory;
using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.DTOs.Progress;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.DTOs.Scenario;
using DigitalShield.API.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DigitalShield.API.Configuration;

public sealed class OpenApiDocumentationOperationFilter : IOperationFilter
{
    private static readonly OpenApiSecurityScheme BearerReference = new()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    private static readonly Dictionary<string, string> Summaries = new(StringComparer.Ordinal)
    {
        ["AuthController.Login"] = "Authenticates a user and returns a JWT access token.",
        ["AuthController.Register"] = "Registers a user with a safe default User role.",
        ["HealthController.GetHealth"] = "Returns the public API health status.",
        ["ConfigHealthController.GetConfigHealth"] = "Returns non-sensitive application configuration health information.",
        ["UsersController.GetCurrentUser"] = "Returns the authenticated user's profile.",
        ["UsersController.UpdateCurrentUser"] = "Updates allowed fields on the authenticated user's profile.",
        ["FraudController.Check"] = "Analyzes a message or URL using explainable fraud-risk indicators.",
        ["FraudCategoriesController.GetAll"] = "Returns active fraud categories.",
        ["FraudCategoriesController.GetById"] = "Returns an active or inactive fraud category by identifier.",
        ["FraudCategoriesController.Create"] = "Creates a fraud category. Requires the Admin role.",
        ["FraudCategoriesController.Update"] = "Updates a fraud category. Requires the Admin role.",
        ["LearningModulesController.GetPublished"] = "Returns published learning modules available to authenticated users.",
        ["LearningModulesController.GetByCategory"] = "Returns published learning modules for a fraud category.",
        ["LearningModulesController.GetById"] = "Returns a published learning module by identifier.",
        ["LearningModulesController.Create"] = "Creates a learning module. Requires the Admin role.",
        ["LearningModulesController.Update"] = "Updates a learning module. Requires the Admin role.",
        ["ScenariosController.GetPublished"] = "Returns published scenarios available to authenticated users.",
        ["ScenariosController.GetByCategory"] = "Returns published scenarios for a fraud category.",
        ["ScenariosController.GetById"] = "Returns a published scenario by identifier.",
        ["ScenariosController.Create"] = "Creates a scenario. Requires the Admin role.",
        ["ScenariosController.Update"] = "Updates a scenario. Requires the Admin role.",
        ["QuizzesController.GetPublished"] = "Returns published quizzes available to authenticated users.",
        ["QuizzesController.GetByCategory"] = "Returns published quizzes for a fraud category.",
        ["QuizzesController.GetById"] = "Returns a quiz without exposing answer correctness.",
        ["QuizzesController.Create"] = "Creates a quiz. Requires the Admin role.",
        ["QuizzesController.Update"] = "Updates a quiz. Requires the Admin role.",
        ["ProgressController.GetCurrentUserProgress"] = "Returns progress records for the authenticated user.",
        ["ProgressController.GetCurrentModuleProgress"] = "Returns the authenticated user's progress for a learning module.",
        ["ProgressController.Create"] = "Creates progress for the authenticated user.",
        ["ProgressController.Update"] = "Updates progress owned by the authenticated user.",
        ["QuizAttemptsController.GetCurrentUserAttempts"] = "Returns quiz attempts for the authenticated user.",
        ["QuizAttemptsController.GetById"] = "Returns a quiz attempt owned by the authenticated user.",
        ["QuizAttemptsController.Submit"] = "Submits answers and calculates the quiz result server-side.",
        ["BadgesController.GetActive"] = "Returns active badges.",
        ["BadgesController.GetById"] = "Returns a badge by identifier.",
        ["BadgesController.Create"] = "Creates a badge. Requires the Admin role.",
        ["BadgesController.Update"] = "Updates a badge. Requires the Admin role."
    };

    private static readonly Dictionary<string, Type> ResponseTypes = new(StringComparer.Ordinal)
    {
        ["AuthController.Login"] = typeof(LoginResponseDto),
        ["AuthController.Register"] = typeof(UserResponseDto),
        ["FraudController.Check"] = typeof(FraudAnalysisResponseDto),
        ["UsersController.GetCurrentUser"] = typeof(UserResponseDto),
        ["UsersController.UpdateCurrentUser"] = typeof(UserResponseDto),
        ["FraudCategoriesController.GetAll"] = typeof(List<FraudCategoryResponseDto>),
        ["FraudCategoriesController.GetById"] = typeof(FraudCategoryResponseDto),
        ["FraudCategoriesController.Create"] = typeof(FraudCategoryResponseDto),
        ["FraudCategoriesController.Update"] = typeof(FraudCategoryResponseDto),
        ["LearningModulesController.GetPublished"] = typeof(List<LearningModuleResponseDto>),
        ["LearningModulesController.GetByCategory"] = typeof(List<LearningModuleResponseDto>),
        ["LearningModulesController.GetById"] = typeof(LearningModuleResponseDto),
        ["LearningModulesController.Create"] = typeof(LearningModuleResponseDto),
        ["LearningModulesController.Update"] = typeof(LearningModuleResponseDto),
        ["ScenariosController.GetPublished"] = typeof(List<ScenarioResponseDto>),
        ["ScenariosController.GetByCategory"] = typeof(List<ScenarioResponseDto>),
        ["ScenariosController.GetById"] = typeof(ScenarioResponseDto),
        ["ScenariosController.Create"] = typeof(ScenarioResponseDto),
        ["ScenariosController.Update"] = typeof(ScenarioResponseDto),
        ["QuizzesController.GetPublished"] = typeof(List<QuizResponseDto>),
        ["QuizzesController.GetByCategory"] = typeof(List<QuizResponseDto>),
        ["QuizzesController.GetById"] = typeof(QuizDetailDto),
        ["QuizzesController.Create"] = typeof(QuizResponseDto),
        ["QuizzesController.Update"] = typeof(QuizResponseDto),
        ["ProgressController.GetCurrentUserProgress"] = typeof(List<UserProgressResponseDto>),
        ["ProgressController.GetCurrentModuleProgress"] = typeof(UserProgressResponseDto),
        ["ProgressController.Create"] = typeof(UserProgressResponseDto),
        ["ProgressController.Update"] = typeof(UserProgressResponseDto),
        ["QuizAttemptsController.GetCurrentUserAttempts"] = typeof(List<QuizAttemptResponseDto>),
        ["QuizAttemptsController.GetById"] = typeof(QuizAttemptResponseDto),
        ["QuizAttemptsController.Submit"] = typeof(QuizAttemptResponseDto),
        ["BadgesController.GetActive"] = typeof(List<BadgeResponseDto>),
        ["BadgesController.GetById"] = typeof(BadgeResponseDto),
        ["BadgesController.Create"] = typeof(BadgeResponseDto),
        ["BadgesController.Update"] = typeof(BadgeResponseDto)
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var controllerName = context.MethodInfo.DeclaringType?.Name ?? "API";
        var actionName = context.MethodInfo.Name;
        var key = $"{controllerName}.{actionName}";
        operation.Tags = new List<OpenApiTag> { new() { Name = controllerName.Replace("Controller", string.Empty) } };
        operation.Summary ??= Summaries.GetValueOrDefault(key, actionName);

        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var isAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();
        var adminOnly = metadata.OfType<AuthorizeAttribute>().Any(attribute =>
            string.Equals(attribute.Roles, Constants.Roles.Admin, StringComparison.Ordinal));

        operation.Security = isAnonymous
            ? new List<OpenApiSecurityRequirement>()
            : new List<OpenApiSecurityRequirement>
            {
                new() { [BearerReference] = Array.Empty<string>() }
            };

        if (adminOnly && !(operation.Description?.Contains("Admin role", StringComparison.OrdinalIgnoreCase) ?? false))
        {
            operation.Description = $"Admin role required. {operation.Description}";
        }

        if (key == "FraudController.Check")
        {
            operation.Description = "The assessment uses predefined heuristic indicators and is not proof that content is fraudulent or safe.";
        }

        AddExamples(operation, key);

        if (context.ApiDescription.HttpMethod is "POST" or "PUT")
        {
            AddResponse(operation, "400", "Invalid request or validation failure.");
        }
        if (!isAnonymous)
        {
            AddResponse(operation, "401", "Authentication is missing or invalid.");
        }
        if (adminOnly)
        {
            AddResponse(operation, "403", "The authenticated user does not have the required role.");
        }
        if (actionName.Contains("GetById", StringComparison.Ordinal) ||
            actionName.Contains("GetCurrentModuleProgress", StringComparison.Ordinal) ||
            actionName.Contains("Update", StringComparison.Ordinal))
        {
            AddResponse(operation, "404", "The requested resource was not found.");
        }
        if (actionName is "Register" or "Create" or "Update" or "Submit")
        {
            AddResponse(operation, "409", "The request conflicts with the current resource state.");
        }
        if (actionName is "Register" or "Create" or "Submit")
        {
            operation.Responses.Remove("200");
            AddResponse(operation, "201", "The resource was created successfully.");
        }
        if (key == "AuthController.Login")
        {
            AddResponse(operation, "401", "The email or password is invalid.");
        }
        AddResponse(operation, "500", "An unexpected server error occurred. Use the trace ID to contact support.");

        if (ResponseTypes.TryGetValue(key, out var responseType))
        {
            var responseCode = actionName is "Register" or "Create" or "Submit" ? "201" : "200";
            if (operation.Responses.TryGetValue(responseCode, out var response))
            {
                response.Content["application/json"] = new OpenApiMediaType
                {
                    Schema = context.SchemaGenerator.GenerateSchema(responseType, context.SchemaRepository)
                };
            }
        }

        AddInlineResponseSchemas(operation, key);
        AddProblemDetailsSchemas(operation, context);
    }

    private static void AddResponse(OpenApiOperation operation, string statusCode, string description)
    {
        if (!operation.Responses.ContainsKey(statusCode))
        {
            operation.Responses.Add(statusCode, new OpenApiResponse { Description = description });
        }
    }

    private static void AddExamples(OpenApiOperation operation, string key)
    {
        if (operation.RequestBody is not null)
        {
            var example = key switch
            {
                "AuthController.Register" => new OpenApiObject
                {
                    ["name"] = new OpenApiString("Development User"),
                    ["email"] = new OpenApiString("user@example.test"),
                    ["password"] = new OpenApiString("Use-a-strong-development-password-123")
                },
                "AuthController.Login" => new OpenApiObject
                {
                    ["email"] = new OpenApiString("user@example.test"),
                    ["password"] = new OpenApiString("Use-a-strong-development-password-123")
                },
                "FraudController.Check" => new OpenApiObject
                {
                    ["type"] = new OpenApiString("message"),
                    ["content"] = new OpenApiString("Your account requires verification. Please verify through the official application."),
                    ["senderKnown"] = new OpenApiBoolean(false)
                },
                "QuizAttemptsController.Submit" => new OpenApiObject
                {
                    ["quizId"] = new OpenApiInteger(1),
                    ["answers"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["questionId"] = new OpenApiInteger(1),
                            ["optionId"] = new OpenApiInteger(2)
                        }
                    }
                },
                _ => null
            };

            if (example is not null)
            {
                foreach (var content in operation.RequestBody.Content.Values)
                {
                    content.Example = example;
                }
            }
        }

        if (key == "FraudController.Check" && operation.Responses.TryGetValue("200", out var fraudResponse))
        {
            foreach (var content in fraudResponse.Content.Values)
            {
                content.Example = new OpenApiObject
                {
                    ["score"] = new OpenApiInteger(40),
                    ["riskLevel"] = new OpenApiString("Suspicious"),
                    ["indicators"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["code"] = new OpenApiString("ACCOUNT_SUSPENSION"),
                            ["title"] = new OpenApiString("Account warning or verification request"),
                            ["score"] = new OpenApiInteger(15),
                            ["explanation"] = new OpenApiString("The content asks the user to verify an account status."),
                            ["category"] = new OpenApiString("Account")
                        },
                        new OpenApiObject
                        {
                            ["code"] = new OpenApiString("URGENCY"),
                            ["title"] = new OpenApiString("Urgency or pressure indicator"),
                            ["score"] = new OpenApiInteger(10),
                            ["explanation"] = new OpenApiString("The content encourages fast action."),
                            ["category"] = new OpenApiString("Behavior")
                        }
                    },
                    ["recommendations"] = new OpenApiArray
                    {
                        new OpenApiObject
                        {
                            ["code"] = new OpenApiString("VERIFY_OFFICIAL_CHANNEL"),
                            ["message"] = new OpenApiString("Verify the request through an official channel.")
                        }
                    },
                    ["disclaimer"] = new OpenApiString("This assessment is based on predefined risk indicators and is not proof that the content is fraudulent or safe.")
                };
            }
        }
    }

    private static void AddInlineResponseSchemas(OpenApiOperation operation, string key)
    {
        if (key == "HealthController.GetHealth" && operation.Responses.TryGetValue("200", out var healthResponse))
        {
            healthResponse.Content["application/json"] = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                    {
                        ["status"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("ok") },
                        ["service"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("DigitalShield API") }
                    },
                    AdditionalPropertiesAllowed = false
                },
                Example = new OpenApiObject
                {
                    ["status"] = new OpenApiString("ok"),
                    ["service"] = new OpenApiString("DigitalShield API")
                }
            };
        }

        if (key == "ConfigHealthController.GetConfigHealth" && operation.Responses.TryGetValue("200", out var configResponse))
        {
            configResponse.Content["application/json"] = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                    {
                        ["application"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("DigitalShield API") },
                        ["version"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("1.0.0") },
                        ["environment"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Development") },
                        ["status"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("ok") }
                    },
                    AdditionalPropertiesAllowed = false
                }
            };
        }
    }

    private static void AddProblemDetailsSchemas(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var (statusCode, response) in operation.Responses)
        {
            if (!int.TryParse(statusCode, out var status) || status < 400)
            {
                continue;
            }

            var schemaType = status == StatusCodes.Status400BadRequest
                ? typeof(ValidationProblemDetails)
                : typeof(ProblemDetails);
            response.Content["application/problem+json"] = new OpenApiMediaType
            {
                Schema = context.SchemaGenerator.GenerateSchema(schemaType, context.SchemaRepository),
                Example = new OpenApiObject
                {
                    ["type"] = new OpenApiString($"https://httpstatuses.com/{status}"),
                    ["title"] = new OpenApiString(response.Description),
                    ["status"] = new OpenApiInteger(status),
                    ["detail"] = new OpenApiString("The request could not be completed."),
                    ["traceId"] = new OpenApiString("00-example-trace-id")
                }
            };
        }
    }
}
