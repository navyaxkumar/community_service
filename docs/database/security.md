# Database Security And Data Protection Review

## Scope And Verification Status

This document records the Phase 3.8 source, EF Core model, migration, configuration, and automated-test review. It does not claim that SQL Server permissions, transport encryption, encryption at rest, backup configuration, restoration, or migration application were verified against a live database.

```text
Source and model review: Verified
Automated security tests: Verified
Live SQL Server verification: Blocked
```

The local LocalDB runtime cannot create its automatic instance, so connection-dependent verification remains unavailable.

## Data Classification

| Classification | Stored data | Protection and minimization decision |
| --- | --- | --- |
| Public/reference | Fraud categories, published learning modules, scenarios, quizzes, quiz questions/options, and badge metadata | Required educational content. Seed content is fictional and does not contain real personal, financial, or credential data. |
| User profile | User ID, name, normalized email, role, active state, and audit timestamps | Required for account identity and authorization. No phone number, address, payment information, or other unnecessary profile fields are modeled. |
| Authentication/security | `User.PasswordHash` | Required for password authentication. Plaintext password, tokens, JWT signing keys, authorization headers, and refresh tokens are not database properties. |
| User activity | Progress state, completion timestamps, quiz ID, score, total questions, and attempt timestamps | Required for learning progress and server-side quiz scoring. Quiz attempts do not store submitted answer text or raw fraud content. |
| Fraud-analysis input | None | Message/URL analysis is stateless. Request content, URLs, indicators, and response data are not mapped as entities or persisted. |

## Authentication And Secret Protection

- Passwords are hashed only through the existing ASP.NET Core Identity `PasswordHasher<User>` abstraction.
- User and login response mappings expose profile fields and an access token only where authentication requires it; `PasswordHash` is not returned by normal DTOs or Swagger response types.
- Login failures use one generic response for unknown email, inactive account, and invalid password.
- Registration assigns the fixed `User` role. Ordinary profile updates cannot accept role, active state, password hash, timestamps, or ID fields.
- JWT signing material is configuration-only, validates a minimum 32-byte secret, and is not persisted.
- Source-controlled settings contain no database password, JWT secret, or development seed password. User Secrets and environment variables are the supported secret sources.

## Fraud Input And SQL Injection Protection

- Fraud messages are limited to 10,000 characters and URLs to 2,048 characters. Validation and service-layer checks both enforce these bounds.
- URLs are parsed as HTTP/HTTPS values only. The analyzer does not fetch URLs, execute commands, access files, render HTML, or execute submitted scripts.
- Fraud input is processed as untrusted in-memory data and is not logged or persisted.
- Repository and service database access uses EF Core LINQ. The source review found no `FromSql`, `ExecuteSql`, `SqlQuery`, or raw migration SQL usage.
- No dynamic SQL identifiers, user-provided sort expressions, or string-built SQL were found.

## Authorization, Ownership, And Exposure

- User profile, progress, and quiz-attempt services use the authenticated identity rather than a client-supplied user ID.
- User-owned DTOs omit `UserId`; quiz submissions omit score and correctness fields; server-side code calculates score from stored quiz options.
- Public quiz detail DTOs omit `QuizOption.IsCorrect`; the management DTO that includes it is used only by admin management paths.
- Entity reads use response DTO mappings, avoiding accidental serialization of `PasswordHash`.
- EF Core relationships and constraints protect integrity: unique user email, unique `(UserId, LearningModuleId)` progress, required relationships, restrictive user deletion for progress/attempts, cascading quiz question/option deletion, and range/non-negative check constraints.

## Seed And Migration Security

- Reference seeds are idempotent and only add missing records. They do not contain real credentials, financial data, or operational attack material.
- Development user seeding requires Development environment plus explicit enablement and externally supplied credentials. It never runs in Production, uses normal password hashing, preserves existing accounts, and cannot promote an existing user.
- Seed failure logging records only the exception type, not provider exception details that may contain database topology or credential-related information.
- Migrations contain model-driven schema operations and no raw SQL, seed credentials, or secrets. The historical `AddInitialDomainModels` migration removes the obsolete `Users.Username` column; it was reviewed as an intentional historical schema evolution and is not modified by this phase.
- No Phase 3.8 migration is required because this review changes no persisted schema.

## Logging And Error Boundaries

- Application logs record application metadata, record counts, exception types, HTTP status, and trace IDs. They do not log passwords, hashes, JWTs, authorization headers, connection strings, or raw fraud submissions.
- Global API error responses are generic Problem Details with a trace ID. They do not return exception messages, stack traces, SQL statements, paths, or connection strings.
- OpenAPI password inputs use a non-secret placeholder, never a reusable or default credential.

## Connection Security And Least Privilege

- Production-like environments require an explicit `ConnectionStrings:DefaultConnection` value. The LocalDB fallback with `TrustServerCertificate=True` is restricted to the ASP.NET Core Development environment.
- Production connection strings and JWT secrets must be supplied through a deployment secret manager or equivalent protected configuration, never source control.
- Use a dedicated runtime database identity with only the permissions required by the application. Do not use `sa`, `sysadmin`, or `db_owner` for routine application runtime access.
- Source/configuration review cannot establish that a live server enforces encrypted transport or encryption at rest. Those properties require environment-specific verification.

## Retention, Deletion, Backup, And Recovery

- Retain user profile, progress, and quiz-attempt data only for the active product purpose and an approved retention period. Avoid adding storage for raw fraud submissions unless a documented feature and retention policy require it.
- User deletion is currently restricted by progress and quiz-attempt foreign keys. A future privacy/deletion feature must define approved deletion, anonymization, or retention behavior before changing that relationship.
- Logs should retain only operationally necessary metadata and follow an environment-specific retention policy.
- Production backups should be encrypted where supported, access-restricted, stored securely, and exercised through documented restoration testing. Backup scripts must not embed credentials, and export access should follow least privilege.

## Follow-Up Boundary

Live SQL Server checks remain outside this source-level review: applied migration state, database permissions, TLS/certificate behavior, encryption at rest, backup configuration, and restoration verification require an available controlled SQL Server environment.
