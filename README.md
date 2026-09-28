# DigitalShield

DigitalShield is an interactive digital fraud awareness and risk-analysis platform.

## Project Status

Phase 2 Backend Development is complete from a code, build, API documentation, and automated test perspective. The backend compiles cleanly, exposes documented REST APIs, includes JWT authentication and role/ownership authorization, and has a security-focused automated test suite.

Local database-backed smoke testing requires a working SQL Server or LocalDB instance. In this workspace, LocalDB was detected but could not create/open the automatic instance, so database-backed endpoint flows could not be completed here.

## Backend Architecture

Backend solution:

```text
backend/
  DigitalShield.sln
  DigitalShield.API/
    Authentication/
    Authorization/
    Configuration/
    Controllers/
    Data/
    DTOs/
    Fraud/
      Analyzer/
      Recommendations/
      Rules/
      Scoring/
    Interfaces/
    Middleware/
    Models/
    Repositories/
    Services/
    Validators/
    Program.cs
```

The API follows a layered structure:

```text
REST API -> Controllers -> Services -> Repositories -> EF Core -> SQL Server
```

Fraud analysis is modular:

```text
Fraud request -> Analyzer -> Rules -> Scoring -> Recommendations -> Safe response
```

## Technology

- .NET SDK 8
- ASP.NET Core Web API
- Entity Framework Core SQL Server
- FluentValidation
- JWT bearer authentication
- Swashbuckle Swagger/OpenAPI
- xUnit backend tests

## Configuration

The API expects JWT settings and a SQL Server connection string from configuration.

Source-controlled settings intentionally do not include secrets. Provide sensitive values through User Secrets, environment variables, or a deployment secret manager.

Required JWT secret setting:

```text
Jwt__SecretKey
```

The secret must be at least 32 bytes long.

If `ConnectionStrings:DefaultConnection` is empty, development falls back to LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=DigitalShieldDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;
```

Optional development seed users are disabled by default. Configure their emails and passwords through .NET User Secrets or environment variables; never add them to `appsettings` files or source control. Both database seeding and development-user seeding must be explicitly enabled, and the accounts are blocked outside ASP.NET Core's `Development` environment. See [database seeding guidance](docs/database/README.md#development-user-seeding).

## Run Backend

From the repository root:

```powershell
dotnet restore backend\DigitalShield.sln
dotnet build backend\DigitalShield.sln
$env:Jwt__SecretKey = "development-only-key-with-at-least-32-bytes"
dotnet run --project backend\DigitalShield.API\DigitalShield.API.csproj
```

Swagger is available in Development:

```text
/swagger
/swagger/v1/swagger.json
```

Health check:

```text
GET /api/health
```

## Authentication And Authorization

Authentication uses JWT bearer tokens.

Primary auth endpoints:

```text
POST /api/auth/register
POST /api/auth/login
```

Protected endpoints require a valid bearer token. Admin-only endpoints require the `Admin` role. User profile, progress, and quiz-attempt operations derive ownership from authenticated identity rather than trusting client-supplied ownership fields.

## API Groups

- `/api/auth`
- `/api/users`
- `/api/fraud`
- `/api/fraud-categories`
- `/api/learning-modules`
- `/api/scenarios`
- `/api/quizzes`
- `/api/progress`
- `/api/quiz-attempts`
- `/api/badges`
- `/api/health`

See [docs/api/README.md](docs/api/README.md) for API usage notes.

## Fraud Engine

The fraud engine uses deterministic heuristic indicators for educational risk assessment. It does not fetch submitted URLs, execute submitted content, execute shell commands, or store raw fraud inputs as part of analysis.

The response includes score, risk level, indicators, recommendations, and a disclaimer. It is not proof that content is fraudulent or safe.

## Testing

Run all backend tests:

```powershell
dotnet test backend\DigitalShield.sln
```

Run security-focused tests:

```powershell
dotnet test backend\DigitalShield.sln --filter Category=Security
```

See [docs/testing/README.md](docs/testing/README.md) for testing strategy, database isolation, and known limitations.

## Security Notes

- Passwords are hashed and never returned.
- JWT secrets are not stored in source.
- Swagger examples use fictional data only.
- Protected ownership fields are server-controlled.
- Quiz scoring is calculated server-side.
- Error responses use safe Problem Details.
- Unexpected exception logs avoid logging exception messages from the application handler.
- CORS is not configured as unrestricted wildcard behavior.
- HTTPS redirection remains enabled.

## Known Deferred Work

The following are intentionally outside Phase 2:

- rate limiting
- refresh tokens
- password reset
- two-factor authentication
- production secret manager setup
- external monitoring
- production deployment
- advanced fraud intelligence feeds
