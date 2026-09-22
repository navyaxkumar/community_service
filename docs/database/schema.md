# DigitalShield Database Schema

This document describes the schema represented by the C# models, `ApplicationDbContext`, EF Core migrations, and model snapshot.

Live SQL Server verification is currently blocked by the local SQL Server environment. This inventory is source/model verified, not live-database verified.

## Timestamp Policy

Server-side timestamps are intended to be UTC.

- Database defaults use `GETUTCDATE()` for creation/start timestamps.
- Application services use `DateTime.UtcNow` for update/completion timestamps.
- `UpdatedAt` and `CompletedAt` are nullable where a record may not yet be updated or completed.

## Tables

### Users

Purpose: Stores password-authenticated user accounts and role state.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | User identifier |
| Name | nvarchar(120) | No |  |  | Display name |
| Email | nvarchar(255) | No | Unique index `IX_Users_Email` |  | Normalized login email |
| PasswordHash | nvarchar(max) | No |  |  | Hashed password only |
| Role | nvarchar(50) | No |  | `User` | Authorization role |
| CreatedAt | datetime2 | No |  | `GETUTCDATE()` | Account creation time |
| UpdatedAt | datetime2 | Yes |  |  | Last profile update time |
| IsActive | bit | No |  | `true` | Login eligibility flag |

Security notes:

- No plaintext password column exists.
- No JWT secret or access token column exists.
- Email normalization is also enforced in authentication/application code; the database unique index is not the only consistency control.

### FraudCategories

Purpose: Stores fraud education categories used to group learning content, scenarios, and quizzes.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Category identifier |
| Name | nvarchar(120) | No | Unique index `IX_FraudCategories_Name` |  | Category name |
| Description | nvarchar(500) | Yes |  |  | Category explanation |
| IsActive | bit | No |  | `true` | Soft visibility/management state |
| CreatedAt | datetime2 | No |  | `GETUTCDATE()` | Creation time |

Delete behavior:

- Child content uses nullable category foreign keys with `SetNull`, preserving educational records if a category is removed.

### LearningModules

Purpose: Stores published or draft educational modules.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Module identifier |
| Title | nvarchar(200) | No |  |  | Module title |
| Description | nvarchar(1000) | Yes |  |  | Short summary |
| Content | nvarchar(max) | No |  |  | Educational content |
| Order | int | No | Check `CK_LearningModules_Order_NonNegative` |  | Display order |
| IsPublished | bit | No |  |  | Publication state |
| CreatedAt | datetime2 | No |  | `GETUTCDATE()` | Creation time |
| UpdatedAt | datetime2 | Yes |  |  | Last update time |
| FraudCategoryId | int | Yes | FK to `FraudCategories.Id`; index `IX_LearningModules_FraudCategoryId` |  | Optional category |

Ordering note:

- `Order` is non-negative.
- Global uniqueness is not enforced because different categories may legitimately reuse the same order values.

### Scenarios

Purpose: Stores educational fraud scenarios, not user-submitted fraud messages.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Scenario identifier |
| Title | nvarchar(200) | No |  |  | Scenario title |
| Description | nvarchar(1000) | Yes |  |  | Short summary |
| Situation | nvarchar(max) | No |  |  | Scenario prompt |
| CorrectAction | nvarchar(max) | No |  |  | Recommended user action |
| IsPublished | bit | No |  |  | Publication state |
| CreatedAt | datetime2 | No |  | `GETUTCDATE()` | Creation time |
| FraudCategoryId | int | Yes | FK to `FraudCategories.Id`; index `IX_Scenarios_FraudCategoryId` |  | Optional category |

### Quizzes

Purpose: Stores quiz containers and publication/category state.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Quiz identifier |
| Title | nvarchar(200) | No |  |  | Quiz title |
| Description | nvarchar(1000) | Yes |  |  | Quiz summary |
| IsPublished | bit | No |  |  | Publication state |
| CreatedAt | datetime2 | No |  | `GETUTCDATE()` | Creation time |
| UpdatedAt | datetime2 | Yes |  |  | Last update time |
| FraudCategoryId | int | Yes | FK to `FraudCategories.Id`; index `IX_Quizzes_FraudCategoryId` |  | Optional category |

Delete behavior:

- Category deletion sets `FraudCategoryId` to null.
- Quiz attempt rows restrict quiz deletion.
- Quiz question rows cascade from quiz deletion.

### QuizQuestions

Purpose: Stores authored quiz questions.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Question identifier |
| QuestionText | nvarchar(max) | No |  |  | Question prompt |
| Explanation | nvarchar(1000) | Yes |  |  | Optional explanation |
| Order | int | No | Check `CK_QuizQuestions_Order_NonNegative` |  | Display order |
| QuizId | int | No | FK to `Quizzes.Id`; index `IX_QuizQuestions_QuizId` |  | Parent quiz |

Ordering note:

- `Order` is non-negative.
- Uniqueness within a quiz is not currently enforced by the database. This avoids adding a constraint until strict uniqueness is an explicit product rule.

### QuizOptions

Purpose: Stores authored answer options for quiz questions.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Option identifier |
| OptionText | nvarchar(max) | No |  |  | Answer text |
| IsCorrect | bit | No |  |  | Correctness for admin/content and server scoring |
| Order | int | No | Check `CK_QuizOptions_Order_NonNegative` |  | Display order |
| QuizQuestionId | int | No | FK to `QuizQuestions.Id`; index `IX_QuizOptions_QuizQuestionId` |  | Parent question |

Security note:

- `IsCorrect` is legitimate database and admin/content-management data.
- User-facing quiz DTOs must not expose `IsCorrect`.

### UserProgress

Purpose: Tracks each user's progress for a learning module.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Progress identifier |
| UserId | int | No | FK to `Users.Id`; unique composite index with `LearningModuleId` |  | Owner |
| LearningModuleId | int | No | FK to `LearningModules.Id`; index `IX_UserProgress_LearningModuleId`; unique composite index with `UserId` |  | Module |
| IsCompleted | bit | No |  |  | Completion state |
| CompletedAt | datetime2 | Yes |  |  | Completion time |
| ProgressPercentage | int | No | Check `CK_UserProgress_ProgressPercentage_Range` | `0` | Progress from 0 to 100 |
| StartedAt | datetime2 | No |  | `GETUTCDATE()` | Start time |

Integrity rules:

- Database-level: `ProgressPercentage` must be between 0 and 100.
- Application-level: when progress reaches 100, the service sets `IsCompleted = true` and populates `CompletedAt`; lowering progress clears completion.
- The composite unique index prevents duplicate progress rows for the same user and learning module.

### QuizAttempts

Purpose: Stores server-calculated quiz attempt results.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Attempt identifier |
| UserId | int | No | FK to `Users.Id`; index `IX_QuizAttempts_UserId` |  | Owner |
| QuizId | int | No | FK to `Quizzes.Id`; index `IX_QuizAttempts_QuizId` |  | Quiz attempted |
| Score | int | No | Check `CK_QuizAttempts_Score_NonNegative`; check `CK_QuizAttempts_Score_NotGreaterThanTotalQuestions` | `0` | Server-calculated score |
| TotalQuestions | int | No | Check `CK_QuizAttempts_TotalQuestions_NonNegative` | `0` | Server-calculated question count |
| StartedAt | datetime2 | No |  | `GETUTCDATE()` | Attempt start time |
| CompletedAt | datetime2 | Yes |  |  | Attempt completion time |

Security note:

- Clients submit answers only.
- The server calculates `Score` and `TotalQuestions`.
- The schema does not store client-provided `IsCorrect` values in quiz attempts.

### Badges

Purpose: Stores badge definitions for later badge retrieval/management.

| Column | Type | Nullable | Key / Constraint | Default | Purpose |
| --- | --- | --- | --- | --- | --- |
| Id | int | No | Primary key | Identity | Badge identifier |
| Name | nvarchar(120) | No |  |  | Badge name |
| Description | nvarchar(500) | Yes |  |  | Badge description |
| Icon | nvarchar(200) | Yes |  |  | Icon identifier/path/name |
| RequiredPoints | int | No | Check `CK_Badges_RequiredPoints_NonNegative` |  | Required points |
| IsActive | bit | No |  | `true` | Visibility/management state |
| CreatedAt | datetime2 | No |  | `GETUTCDATE()` | Creation time |

No user-badge join table exists yet; badge awarding is not part of the current schema.

## Relationship Delete Behavior Matrix

| Parent | Child | FK Nullable | Delete behavior | Rationale |
| --- | --- | --- | --- | --- |
| FraudCategory | LearningModule | Yes | SetNull | Preserve content if category changes/removes |
| FraudCategory | Scenario | Yes | SetNull | Preserve content if category changes/removes |
| FraudCategory | Quiz | Yes | SetNull | Preserve content if category changes/removes |
| Quiz | QuizQuestion | No | Cascade | Avoid orphaned questions after intentional quiz deletion |
| QuizQuestion | QuizOption | No | Cascade | Avoid orphaned options after intentional question deletion |
| User | UserProgress | No | Restrict | Preserve ownership/history and prevent accidental data loss |
| LearningModule | UserProgress | No | Cascade | Progress depends on its module |
| User | QuizAttempt | No | Restrict | Preserve quiz attempt history and prevent accidental data loss |
| Quiz | QuizAttempt | No | Restrict | Preserve attempt history and prevent accidental data loss |

## Index Inventory

| Index | Table | Columns | Unique | Purpose |
| --- | --- | --- | --- | --- |
| IX_Users_Email | Users | Email | Yes | Enforce unique login identity |
| IX_FraudCategories_Name | FraudCategories | Name | Yes | Avoid duplicate category names |
| IX_LearningModules_FraudCategoryId | LearningModules | FraudCategoryId | No | Category filtering |
| IX_Scenarios_FraudCategoryId | Scenarios | FraudCategoryId | No | Category filtering |
| IX_Quizzes_FraudCategoryId | Quizzes | FraudCategoryId | No | Category filtering |
| IX_QuizQuestions_QuizId | QuizQuestions | QuizId | No | Load questions for quiz |
| IX_QuizOptions_QuizQuestionId | QuizOptions | QuizQuestionId | No | Load options for question |
| IX_UserProgress_LearningModuleId | UserProgress | LearningModuleId | No | Module/progress joins |
| IX_UserProgress_UserId_LearningModuleId | UserProgress | UserId, LearningModuleId | Yes | Prevent duplicate user/module progress |
| IX_QuizAttempts_UserId | QuizAttempts | UserId | No | User attempt history |
| IX_QuizAttempts_QuizId | QuizAttempts | QuizId | No | Quiz attempt history |

No additional indexes were added in Phase 3.2 because the existing indexes already cover the current foreign-key and lookup patterns.

## Security And Data Minimization

- No plaintext passwords are stored.
- Password hashes are required for password-authenticated users.
- JWT secrets and access tokens are not stored in normal tables.
- OTP, PIN, CVV, card, address, phone, or government ID fields do not exist.
- Fraud analysis does not persist raw submitted messages or URLs.
- EF Core LINQ is used for data access; no raw SQL query usage was found.
- Foreign keys and unique constraints protect core relationships and duplicate identity/progress data.

## Migration Status

Phase 3.2 added `RefineDatabaseSchema`, an additive migration containing check constraints only.

Phase 3.3 and Phase 3.4 seed reference data at runtime through `IDatabaseSeeder`; seed data is not embedded in migrations.

Live SQL migration application remains blocked by the local SQL Server environment, as documented in `docs/database/README.md`.
