# DigitalShield Backend Testing

## Test Projects

Backend tests live in `backend/DigitalShield.Tests` and target `net8.0` with xUnit.

The suite covers:

- service behavior and DTO mapping
- validators and mass-assignment guardrails
- authentication, password hashing, and JWT validation
- authorization roles and ownership checks
- fraud rules, scoring, recommendations, and input security
- global error handling and safe logging
- controller route/security metadata

## How to Run

From the repository root:

```powershell
dotnet restore backend\DigitalShield.sln
dotnet build backend\DigitalShield.sln
dotnet test backend\DigitalShield.sln
```

Run only security-marked tests:

```powershell
dotnet test backend\DigitalShield.sln --filter Category=Security
```

## Database Strategy

Current automated tests use a unique EF Core InMemory database per test via a GUID database name. This keeps tests fast and isolated, and it prevents accidental writes to the developer's normal DigitalShield database.

Known limitation: EF Core InMemory is not a relational database and does not perfectly match SQL Server behavior for constraints and SQL translation. Repository tests that depend on relational behavior should be expanded later with LocalDB, SQLite compatibility mode, or a dedicated SQL Server test database. Do not point automated tests at a normal development or production database.

## Security Testing

Security regression tests are grouped with the xUnit trait `Category=Security`.

Covered scenarios include:

- anonymous/protected and user/admin authorization metadata
- IDOR-style ownership checks for profiles, progress, and quiz attempts
- mass-assignment attempts against role, user ID, password hash, score, and correctness fields
- quiz score manipulation attempts
- invalid, tampered, expired, and malformed JWT behavior
- oversized and malformed fraud inputs
- URL analysis without outbound network access
- SQL injection, XSS, path traversal, and Unicode payloads treated as data
- safe Problem Details responses
- logs avoiding password, token, path, and other sensitive exception details

## Test Data

Use fictional `.example.test` addresses and test-only passwords. Do not commit real user data, real access tokens, API keys, database credentials, OTPs, payment data, or private URLs.

## Known Limitations

The suite currently emphasizes unit, service, controller metadata, and in-process security boundary tests. Full HTTP pipeline integration tests with `WebApplicationFactory` are not enabled because the required ASP.NET Core integration testing packages were not already present in the local package cache during this phase. Add them deliberately in a future phase if network/package installation is approved.
