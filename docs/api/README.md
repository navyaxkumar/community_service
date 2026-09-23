# DigitalShield API

## Run the API

From `backend/`:

```powershell
dotnet run --project DigitalShield.API
```

In Development, Swagger UI is available at `/swagger` and the OpenAPI document is available at `/swagger/v1/swagger.json`.

## Authentication

Register with `POST /api/auth/register`, then log in with `POST /api/auth/login`. Login returns a JWT access token and safe user information. In Swagger UI, use **Authorize** and enter the token as a bearer token.

Passwords are hashed and JWT secrets are supplied through User Secrets or deployment environment configuration. Secrets are not stored in source control.

## Authorization

Protected endpoints require a valid JWT. Content-management operations require the `Admin` role. User profile, progress, and quiz-attempt operations derive identity from the authenticated token and enforce ownership.

## Main API groups

- `/api/auth` - registration and login
- `/api/users` - authenticated user profile
- `/api/fraud` - authenticated message and URL risk analysis
- `/api/fraud-categories` - category retrieval and Admin management
- `/api/learning-modules` - published learning and Admin management
- `/api/scenarios` - published scenarios and Admin management
- `/api/quizzes` - published quizzes and Admin management
- `/api/progress` - authenticated user progress
- `/api/quiz-attempts` - authenticated quiz attempts
- `/api/badges` - badge retrieval and Admin management
- `/api/health` - public health status

## Errors

Errors use HTTP Problem Details. Validation errors return field-level errors. Unexpected failures return a generic message and a trace ID for server-side investigation. Responses never include stack traces, SQL details, passwords, hashes, tokens, or secrets.

## Fraud analysis

`POST /api/fraud/check` accepts a bounded message or HTTP(S) URL. The engine uses deterministic, predefined heuristic indicators and does not fetch submitted URLs. The result is an educational risk assessment, not proof that content is fraudulent or safe.

## Security expectations

Use HTTPS for authenticated traffic. Treat all request data as untrusted. Do not submit passwords, OTPs, payment credentials, access tokens, or other real sensitive information in development examples.
