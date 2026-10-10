# DigitalShield Database Production Readiness Audit

## Scope and Status

This document captures the remaining production-readiness work for the DigitalShield database layer after the repository-level source, model, migration, and security review. It intentionally separates:

- controls that were verified from repository source and automated tests, and
- controls that require a human administrator with access to the target SQL Server environment, certificate configuration, backups, monitoring, and deployment tooling.

## Evidence reviewed

The following repository artifacts were reviewed as evidence for the database and security baseline:

- [backend/DigitalShield.API/Data/ApplicationDbContext.cs](../../backend/DigitalShield.API/Data/ApplicationDbContext.cs)
- [backend/DigitalShield.API/Program.cs](../../backend/DigitalShield.API/Program.cs)
- [backend/DigitalShield.API/Authentication/AuthenticationService.cs](../../backend/DigitalShield.API/Authentication/AuthenticationService.cs)
- [backend/DigitalShield.API/Authentication/PasswordHasherService.cs](../../backend/DigitalShield.API/Authentication/PasswordHasherService.cs)
- [backend/DigitalShield.Tests/DatabaseSecurityReviewTests.cs](../../backend/DigitalShield.Tests/DatabaseSecurityReviewTests.cs)
- [docs/database/security.md](./security.md)
- [docs/database/README.md](./README.md)
- [README.md](../../README.md)

The repository contains no committed production connection strings, no SQL credentials, and no JWT secret values. The app currently requires a secure external configuration source for both connection strings and signing secrets.

## Verified repository controls

The following are verified by source inspection and automated tests and are considered repository-level controls only:

1. Password hashing
   - User passwords are hashed with the ASP.NET Core Identity password hasher rather than stored in plaintext.
   - The repository test suite checks that stored values are not raw passwords and that verification succeeds with the configured password.

2. No sensitive database fields in persisted model
   - The persistence model does not include plaintext password, token, JWT secret, refresh token, or authorization header columns.
   - Security tests confirm these values are not mapped to EF entities.

3. No unsafe raw SQL usage
   - Repository and service code uses EF Core LINQ queries. No raw SQL string execution or dynamic SQL object names were identified in the source review.

4. Input and schema protection
   - Unique email enforcement, non-negative checks, range checks, and restrict/cascade delete rules are defined in the EF model.
   - The code does not persist raw fraud submissions or tokens as database content.

5. Safe defaults and environment separation
   - A LocalDB fallback exists only in the Development environment and is gated by the application environment check.
   - The app refuses to rely on a missing production connection string without explicit environment configuration.

## Operational gaps that require administrator verification

The following operational checks remain unverified because they require access to a real SQL Server environment, certificate configuration, backup automation, or deployment infrastructure.

### 1) Least-privilege SQL Server permissions

The repository does not include the live SQL Server role assignments. A production deployment must be approved by a database administrator with access to the server.

Recommended production pattern (proposal requiring owner approval):

- Runtime application identity
  - Connect to the application database only.
  - Grant schema-level permissions to the application tables and required views.
  - Allow SELECT, INSERT, UPDATE, DELETE as needed by the API.
  - Permit execution of required stored procedures or functions only if they are used intentionally.
  - Explicitly deny or avoid:
    - `db_owner`
    - `db_accessadmin`
    - `db_securityadmin`
    - `ALTER ANY DATABASE`
    - `CREATE DATABASE`
    - `ALTER ANY LOGIN`
    - `sysadmin` or equivalent server-level admin roles
  - This identity should never own schema migration tasks or admin-level operational maintenance.

- Migration and deployment identity
  - Use a separate identity specifically for EF Core migrations and schema changes.
  - Grant only the permissions required for table creation, index creation, check constraints, and schema updates.
  - Keep migration rights separate from runtime application rights.
  - The migration identity should only be used during controlled release windows.

- Minimum approved approach
  - Prefer schema-scoped grants over broad database roles when the environment supports it.
  - If the SQL Server team chooses broader roles for operational simplicity, that must be explicitly approved and documented with a risk acceptance.

This is a proposed least-privilege model and must be reviewed and approved by the database administrator before production use.

### 2) Production TLS certificate validation

The repository-level review confirms the app is expected to use a secure SQL Server connection string, but it does not verify a live SQL Server instance is configured to require TLS and validate the certificate.

Production requirements:

- `Encrypt=True` when supported by the server/client stack.
- `TrustServerCertificate=False` in production.
- Certificate validation must be enabled so the server certificate is checked against the expected trust chain and hostname.
- `Integrated Security` or managed identity patterns may be used where supported, but they must still enforce certificate validation and least privilege.
- A production connection string must never be committed to source control or embedded in documentation.

A typical production pattern is:

- secret manager / environment variable injection for SQL Server connection string
- `Encrypt=True; TrustServerCertificate=False;` in the connection string
- certificate flow validated by the SQL Server infrastructure team

This check remains unverified until the live SQL Server environment is inspected and approved by an administrator.

### 3) Secret injection, rotation, and logging

Repository control: secrets are not stored in source files. Production deployment must use a secret manager or equivalent protected configuration store.

Required production controls:

- Store database connection strings, JWT secret keys, and other sensitive configuration values in a secret manager such as Azure Key Vault, AWS Secrets Manager, or an equivalent approved system.
- Inject secrets via environment variables or application configuration at runtime only.
- Use rotation procedures for both database credentials and JWT secret values.
- Rotate credentials after operational changes and on a defined schedule.
- Do not log connection strings, JWT values, raw passwords, hash values, authorization headers, or SQL exception details that may expose topology information.
- Audit log access to secret stores and restrict permissions to operators who need them.

The codebase does not manage rotation automatically; this is a deployment and operations requirement.

### 4) Backup retention, RPO/RTO, and tested restore procedure

The repository does not contain evidence of a configured SQL Server backup job, retention schedule, or recovery exercise.

Proposed values requiring owner approval (not verified):

| Control | Proposed value | Approval required |
| --- | --- | --- |
| Full backup frequency | Weekly full backup | Yes |
| Differential backup frequency | Every 6 hours | Yes |
| Transaction log backup frequency | Every 5 to 15 minutes | Yes |
| Backup retention | 30 days online + 12 months archive | Yes |
| Recovery point objective (RPO) | 15 minutes | Yes |
| Recovery time objective (RTO) | 4 hours | Yes |
| Restore testing | Quarterly restore drill | Yes |

Required restore checklist:

1. Confirm backup chain integrity and encryption.
2. Restore to a non-production environment or isolated recovery environment.
3. Validate application connectivity, schema state, and data integrity.
4. Check authentication and authorization behavior after restore.
5. Record restore duration, data loss window, and any manual remediation steps.
6. Validate that no credentials or secrets were exposed during restore operations.

This area is not verified in the repository and must be validated by the database administrator or hosting platform owner before production release.

### 5) Reviewed production migration process

The repository includes EF Core migrations and model snapshot files, but the live migration execution process has not been verified against a production database.

Required migration controls:

- Use a separate migration identity with the minimum required schema permissions.
- Run during a defined maintenance window with approval from the operational owner.
- Ensure a valid backup exists before schema changes.
- Capture the migration plan and confirm no destructive changes are included.
- Avoid automatic migrations on application startup in production.
- Confirm the target database is the intended environment before applying migration scripts.
- Review the generated migration SQL prior to execution.
- Maintain an explicit rollback plan and recovery path.

A migration may be allowed only when all of the following are true:

- database admin approves the plan,
- backup validation has succeeded,
- the deployment environment is explicitly identified,
- the schema change is reviewed for data impact,
- rollback or recovery strategy exists.

### 6) Health monitoring, alerts, failure detection, and capacity monitoring

The repository does not provide evidence of production SQL Server monitoring or alerting configuration.

Minimum production monitoring for SQL Server and the application:

- CPU saturation and memory pressure
- database file growth and disk thresholds
- blocked sessions and deadlocks
- login failures and failed authentication patterns
- connection pool saturation or high latency
- failed backup events and backup age monitoring
- growth in transaction log and transaction backlog
- database availability alerts
- application error rate and 5xx rate

Recommended alerts:

- database not reachable for a defined threshold
- backup failure or backup age beyond retention window
- elevated deadlocks or blocking
- login failure spikes
- disk space below threshold
- storage growth trend beyond expected capacity
- extended transaction log growth

Monitoring should be reviewed and implemented by the hosting and operations team.

### 7) Verified repository controls vs. environment checks

Verified in repository:

- password hashing and no plaintext password storage
- no raw SQL execution in application code
- EF Core model constraints and delete behavior
- environment-gated LocalDB fallback
- source control excludes secrets
- security tests pass for repository-level protections

Requires human administrator or production infrastructure access:

- SQL Server least-privilege configuration
- TLS certificate validation and connection-string security
- backup automation, retention, and restore testing
- migration execution approval and rollback plan
- production monitoring and alerts
- capacity planning and load review
- operational change control

### 8) Evidence for the current state

The repository evidence supports the following conclusion:

- The application codebase is prepared for secure application-level database access patterns.
- The project includes security-focused testing and integrity checks.
- The database is not yet production-verified against a live SQL Server environment.

The live operational checks remain blocked by missing administrator access and a real production-like SQL Server environment.

## GO / NO-GO recommendation

Current recommendation: NO-GO for production deployment until the administrator-owned operational controls are reviewed and approved.

Reason:

- repository controls are in a good state, but
- live SQL Server permissions, TLS configuration, backup strategy, restore validation, and operational monitoring have not been verified with evidence,
- and the repository alone cannot establish production readiness.

The database is conditionally ready for a controlled pre-production or staging validation environment only after the following are confirmed by the responsible infrastructure owner:

- least-privilege runtime and migration identities,
- TLS and certificate validation,
- production secret injection and rotation,
- backup and restore execution evidence,
- migration plan and rollback rehearsal,
- production alerting and capacity monitoring.

## Final disposition

This audit does not claim production deployment readiness. It documents the repository-level controls that are in place and the environment-specific validation that still must be performed by authorized administrators before a production GO decision is made.
