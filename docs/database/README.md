# DigitalShield Database

## Supported SQL Server Options

DigitalShield uses Entity Framework Core with the SQL Server provider.

Supported development targets include:

- SQL Server Developer Edition
- SQL Server Express
- SQL Server LocalDB, when available
- Docker SQL Server, when Docker is installed
- A dedicated remote development SQL Server

Do not use a production database for development or migration verification.

## Project Configuration

Database context:

```text
backend/DigitalShield.API/Data/ApplicationDbContext.cs
```

Provider registration:

```text
Program.cs -> AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(...))
```

Connection string key:

```text
ConnectionStrings:DefaultConnection
```

The source-controlled `appsettings.json` and `appsettings.Development.json` intentionally leave this value empty. When it is empty, development falls back to a LocalDB connection string for `DigitalShieldDb`.

## User Secrets

For local development, store the real development database connection string outside source control:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<development-sql-server-connection-string>" --project backend\DigitalShield.API\DigitalShield.API.csproj
```

Never commit SQL usernames, passwords, API keys, JWT secrets, or production connection strings.

## EF Core CLI

This repository uses a local `dotnet-ef` tool so migration commands are reproducible.

Install/restore local tools:

```powershell
dotnet tool restore
```

Verify the EF CLI:

```powershell
dotnet ef --version
```

Expected major version: `8.x`, matching the EF Core packages used by the API.

## Migration Commands

List migrations:

```powershell
dotnet ef migrations list `
  --project backend\DigitalShield.API\DigitalShield.API.csproj `
  --startup-project backend\DigitalShield.API\DigitalShield.API.csproj
```

Apply migrations to the configured development database:

```powershell
dotnet ef database update `
  --project backend\DigitalShield.API\DigitalShield.API.csproj `
  --startup-project backend\DigitalShield.API\DigitalShield.API.csproj
```

Do not create a new migration unless the application model intentionally changes.

## Seed Data Foundation

Phase 3.3 adds runtime seed infrastructure for small, repeatable application/reference data.

Seeder files:

```text
backend/DigitalShield.API/Data/IDatabaseSeeder.cs
backend/DigitalShield.API/Data/DatabaseSeeder.cs
backend/DigitalShield.API/Data/Seed/FraudCategorySeed.cs
backend/DigitalShield.API/Data/Seed/BadgeSeed.cs
```

DI registration:

```text
IDatabaseSeeder -> DatabaseSeeder
```

Startup integration is controlled by configuration:

```json
{
  "DatabaseSeeding": {
    "Enabled": false
  }
}
```

The default is `false` so a developer machine without a reachable SQL Server can still start the API for non-database checks. When `DatabaseSeeding:Enabled` is `true`, startup creates a scoped service provider, resolves `IDatabaseSeeder`, and runs the seed operation before the API begins serving requests. If the database is unavailable, the seed operation logs a safe diagnostic and rethrows the database error.

Run seeding during application startup:

```powershell
$env:DatabaseSeeding__Enabled = "true"
dotnet run --project backend\DigitalShield.API\DigitalShield.API.csproj
```

Seed detection uses stable natural keys:

- `FraudCategories.Name`
- `LearningModules.Title`
- `Badges.Name`

The seeder checks whether each required seed record already exists before inserting it. Running the seeder repeatedly must not create duplicate fraud categories, learning modules, or badges.

Current reference seed records:

- Fraud categories:
  - `Phishing & OTP Scams`
  - `UPI & Payment Fraud`
  - `Fake Job Scams`
  - `Online Shopping Scams`
  - `Fake Websites & Links`
  - `Impersonation & Social Engineering`
  - `Investment & Loan Scams`
- Learning modules: 21 published modules, 3 under each seeded fraud category.
- Scenarios: 8 published fictional fraud-awareness scenarios across the seeded categories.
- Quizzes: 7 published quizzes, one per seeded category.
- Quiz questions: 35 total, 5 per seeded quiz.
- Quiz options: 140 total, 4 per seeded question.
- Badge: `Awareness Starter`

The Phase 3.4 learning content and Phase 3.5 quiz/scenario content are safe reference education data for general fraud awareness. They use fictional examples only, avoid real personal/financial information, and do not include operational attack instructions.

Development/test data:

- No users are seeded.
- No development admin account is seeded.
- No plaintext password or password hash is seeded.
- No predictable credentials such as `admin@example.com` are present.

Production behavior:

- Production does not receive development/test users.
- The current foundation reference records are safe application data, but they are only inserted when `DatabaseSeeding:Enabled` is explicitly enabled.
- Do not enable startup seeding in production without a controlled deployment decision.

Security and preservation:

- The seeder only adds missing records.
- It does not delete records.
- It does not overwrite users, progress, quiz attempts, authored categories, or authored learning modules.
- It does not call `EnsureDeleted()` or `EnsureCreated()`.
- It does not log secrets, passwords, password hashes, JWTs, tokens, OTPs, PINs, CVVs, or connection-string contents.

Published content decision:

- Phase 3.4 learning modules are seeded as `IsPublished = true` because they are reviewed application reference content and are intended to be visible in development/demo learning flows.
- If an administrator edits an existing seeded module, later seed runs detect the title and leave the record unchanged.
- Phase 3.5 scenarios and quizzes are also seeded as `IsPublished = true` because they are reviewed reference content.
- If an administrator edits an existing seeded scenario or quiz, later seed runs detect the title and leave the record unchanged.

Quiz answer protection:

- Seeded quiz options include `IsCorrect` for server-side scoring and admin/management views.
- User-facing quiz responses use DTOs that omit `IsCorrect`.
- Quiz submissions accept only quiz/question/option answer IDs.
- The server calculates score from stored quiz options and does not trust client-provided score, correctness, or user identity.

## Existing Migrations

Current migrations:

```text
20260920130618_InitialCreate
20260920131521_AddInitialDomainModels
20260920150244_RefineDatabaseSchema
```

The model snapshot includes:

- Users
- FraudCategories
- LearningModules
- Scenarios
- Quizzes
- QuizQuestions
- QuizOptions
- UserProgress
- QuizAttempts
- Badges

## Schema Expectations

Detailed source-verified schema documentation is maintained in:

```text
docs/database/schema.md
```

Important primary keys:

- `Id` on all entity tables

Important foreign keys:

- `LearningModules.FraudCategoryId`
- `Scenarios.FraudCategoryId`
- `Quizzes.FraudCategoryId`
- `QuizQuestions.QuizId`
- `QuizOptions.QuizQuestionId`
- `UserProgress.UserId`
- `UserProgress.LearningModuleId`
- `QuizAttempts.UserId`
- `QuizAttempts.QuizId`

Important uniqueness rules:

- `Users.Email`
- `FraudCategories.Name`
- `UserProgress(UserId, LearningModuleId)`

Important delete behavior:

- Fraud category relationships use `SetNull` for learning modules, scenarios, and quizzes.
- Quiz questions/options cascade under their quiz/question parent.
- User-owned progress and quiz attempts restrict user deletion.
- Quiz attempts restrict quiz deletion.

Phase 3.2 added database check constraints for simple numeric invariants:

- Non-negative display ordering on learning modules, quiz questions, and quiz options.
- User progress percentage must stay between 0 and 100.
- Quiz attempt score and total question counts must be non-negative.
- Quiz attempt score cannot exceed total questions.
- Badge required points must be non-negative.

The Phase 3.2 migration is additive and contains check constraints only.

## Phase 3.1 Environment Verification

Observed on this machine:

- `.NET SDK`: `8.0.425`
- EF Core packages: `8.0.8`
- `dotnet-ef`: installed as local tool version `8.0.8`
- LocalDB instance name appears in `sqllocaldb info`, but `sqllocaldb info MSSQLLocalDB` fails with a LocalDB registry/runtime access error.
- SQL Server Express service `MSSQL$SQLEXPRESS` is running.
- SQL Express named pipes and TCP are disabled in the SQL Server registry configuration.
- `dotnet ef database update` against `.\SQLEXPRESS` builds successfully but cannot connect because the SQL Server/client encryption path is not usable in this environment.

Result:

```text
Database environment: Blocked
Migration files: Verified
Live schema: Not verified
Application-to-SQL integration: Not verified
```

## Troubleshooting

If LocalDB is used:

```powershell
sqllocaldb info
sqllocaldb info MSSQLLocalDB
```

If SQL Express is used:

- Verify the `SQL Server (SQLEXPRESS)` service is running.
- Ensure a supported local connection protocol is enabled.
- Use a dedicated development database such as `DigitalShield_Dev` or `DigitalShield_Test`.
- Keep real connection strings in User Secrets or environment variables.

If encryption errors occur, verify the SQL Server instance certificate/encryption configuration and client support. Do not bypass production encryption requirements casually; for local development, document the chosen setting and keep it environment-specific.

## Security Notes

- Passwords are stored only as hashes.
- JWT secrets are not stored in the database.
- Authentication tokens are not persisted by the current model.
- Fraud analysis is stateless and does not store submitted raw messages or URLs.
- Repositories use EF Core LINQ; no raw SQL queries are currently used.
- No destructive database operation was performed during Phase 3.1 verification.

## Phase 3.2 Schema Review

Source/model verification passed for the current schema inventory, migration chain, indexes, foreign keys, delete behaviors, and security-sensitive storage review.

Live SQL Server verification remains blocked by the local SQL Server environment described above, so `RefineDatabaseSchema` has not been applied to a live local database from this machine.

## Phase 3.4 Seed Data Review

Source/test verification passed for the fraud category and learning module seed dataset.

No schema migration was required for Phase 3.4 because `FraudCategories.Name` already provides a stable unique natural key and learning modules can be idempotently detected by stable seed titles.

Live SQL Server seed execution remains blocked by the local SQL Server environment described above, so the seed dataset has not been verified against a live local SQL Server database from this machine.

## Phase 3.5 Seed Data Review

Source/test verification passed for the scenario, quiz, quiz question, and quiz option seed dataset.

No schema migration was required for Phase 3.5 because the existing schema already supports scenarios, quizzes, nested questions, nested options, publication state, category relationships, deterministic ordering, and server-side scoring.

Live SQL Server seed execution remains blocked by the local SQL Server environment described above, so the Phase 3.5 seed dataset has not been verified against a live local SQL Server database from this machine.
