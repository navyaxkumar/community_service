# DigitalShield Staging Deployment Guide

**Target Architecture:**
- **Frontend:** Vercel (Vite + React SPA)
- **Backend:** Azure App Service (.NET 8 ASP.NET Core Web API)
- **Database:** Azure SQL Database (Entity Framework Core 8)
- **Repository:** `C:\Users\krish\Desktop\DigitalShield`

---

## 1. Architecture Overview & Origin Design

Because the frontend is hosted on **Vercel** (`https://<project-name>.vercel.app`) and the backend is hosted on **Azure App Service** (`https://<app-name>.azurewebsites.net`), this deployment operates across **separate origins**.

```text
┌──────────────────────────────────────────────┐
│                  USER BROWSER                │
└──────────────┬───────────────────────────────┘
               │
      HTTPS GET / Navigation
               ▼
┌──────────────────────────────────────────────┐
│             VERCEL EDGE NETWORK              │
│  - Static React SPA assets                   │
│  - SPA rewrite to index.html (vercel.json)   │
│  - Origin: https://digitalshield-staging.    │
│            vercel.app                        │
└──────────────────────────────────────────────┘
               │
      Cross-Origin HTTPS API Calls (/api/*)
      with Bearer JWT Authorization Header
               ▼
┌──────────────────────────────────────────────┐
│           AZURE APP SERVICE (.NET 8)         │
│  - ASP.NET Core Web API                      │
│  - Restrictive CORS: allows only Vercel URL  │
│  - Stateless Heuristic Fraud Analyzer        │
│  - JWT Bearer Authentication                 │
└──────────────────────┬───────────────────────┘
                       │
              Encrypted TDS / Port 1433
              TLS 1.2+ Validation
                       ▼
┌──────────────────────────────────────────────┐
│             AZURE SQL DATABASE               │
│  - General Purpose Serverless                │
│  - EF Core Migrations                        │
│  - Schema Constraints & Foreign Keys         │
└──────────────────────────────────────────────┘
```

---

## 2. Phase 1 — Vercel Frontend Setup

### Project Configuration
1. In the Vercel Dashboard, select **Add New Project** and link your Git repository.
2. Configure project build settings:
   - **Framework Preset**: `Vite`
   - **Root Directory**: `frontend`
   - **Build Command**: `npm run build`
   - **Output Directory**: `dist`
   - **Install Command**: `npm install`

### SPA Routing & Deep Link Refresh (`vercel.json`)
Client-side routers (React Router) require all route paths (e.g. `/scam-checker`, `/learn/1`, `/quizzes`) to resolve to `/index.html` on refresh. This is handled by [`frontend/vercel.json`](../../frontend/vercel.json):

```json
{
  "$schema": "https://openapi.vercel.sh/vercel.json",
  "rewrites": [
    {
      "source": "/(.*)",
      "destination": "/index.html"
    }
  ]
}
```

### Environment Variables on Vercel
In **Project Settings &rarr; Environment Variables**:

| Variable Name | Environments | Value Example | Description |
|:---|:---:|:---|:---|
| `VITE_API_URL` | Staging / Preview | `https://digitalshield-api-staging.azurewebsites.net/api` | Base URL of the staging App Service API. |
| `VITE_API_URL` | Production | `https://digitalshield-api.azurewebsites.net/api` | Base URL of the production App Service API. |

> [!IMPORTANT]
> - Never store secrets, database passwords, or JWT keys in Vercel environment variables. Any variable prefixed with `VITE_` is baked into the public client JavaScript bundle.
> - The frontend [`apiClient.ts`](../../frontend/src/services/apiClient.ts) automatically strips trailing slashes to prevent malformed double-slash URLs.

---

## 3. Phase 2 — Azure App Service Backend Setup

### Resource Provisioning
1. **Operating System**: Linux (recommended) or Windows.
2. **Runtime Stack**: `.NET 8 (LTS)`.
3. **App Service Plan**: `B1` (Basic) for staging; `S1` (Standard) or higher for production.
4. **Networking**: Enforce **HTTPS Only** and set **Minimum TLS Version** to `1.2`.

### Application Settings (Environment Variables)
In Azure Portal under **App Service &rarr; Configuration &rarr; Application Settings**:

| Setting Name | Required | Secret? | Example / Format |
|:---|:---:|:---:|:---|
| `ASPNETCORE_ENVIRONMENT` | Yes | No | `Staging` |
| `ConnectionStrings__DefaultConnection` | Yes | **YES** | `Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<staging-db>;Persist Security Info=False;User ID=<app-user>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;` |
| `Jwt__SecretKey` | Yes | **YES** | &ge; 32 bytes (256-bit cryptographically secure string from Key Vault) |
| `Jwt__Issuer` | No | No | `DigitalShield` |
| `Jwt__Audience` | No | No | `DigitalShieldUsers` |
| `Jwt__ExpiryMinutes` | No | No | `60` |
| `Cors__AllowedOrigins__0` | Yes | No | `https://digitalshield-staging.vercel.app` (exact Vercel URL, no trailing slash, no wildcard) |
| `DatabaseSeeding__Enabled` | Optional | No | `true` on initial deployment to seed fraud categories/quizzes; revert to `false` once populated. |
| `DevelopmentUserSeeding__Enabled` | No | No | `false` (hard-blocked in application code when environment is not `Development`). |

### Restrictive CORS Configuration
ASP.NET Core [`Program.cs`](../../backend/DigitalShield.API/Program.cs) reads `Cors:AllowedOrigins` and configures restrictive origin validation:
- Disallows wildcards (`*`).
- Binds only to specified origins.
- Allows required API headers (`Content-Type`, `Authorization`, `Accept`) and methods (`GET`, `POST`, `PUT`, `DELETE`, `OPTIONS`).

---

## 4. Phase 3 — Azure SQL Database Setup

### Provisioning & Network Security
1. Create an Azure SQL Database:
   - **Compute Tier**: General Purpose Serverless (auto-pauses when inactive to optimize staging costs).
   - **Collation**: `SQL_Latin1_General_CP1_CI_AS` (default).
2. **Firewall & Virtual Networks**:
   - Enable **"Allow Azure services and resources to access this server"** so Azure App Service can connect.
   - For CI/CD runners (GitHub Actions / Azure DevOps), add the runner IP address to the firewall rules during deployment, or use an Azure Virtual Network Integration with Private Endpoints.
3. **Transport Encryption**:
   - Connection strings **must** specify `Encrypt=True;TrustServerCertificate=False;`.
   - LocalDB fallbacks and `TrustServerCertificate=True` are restricted to the local Development environment.

### Database Users & Least Privilege Roles
To follow least privilege:
- **Migration Identity (CI/CD Pipeline)**:
  - Grants: `db_ddladmin`, `db_datareader`, `db_datawriter`.
  - Used only during deployment to execute schema migrations.
- **Runtime Identity (App Service)**:
  - Grants: `db_datareader`, `db_datawriter` on application schema only.
  - Deny: `db_owner`, `ALTER ANY DATABASE`, `sysadmin`.

---

## 5. Phase 4 — Database Migration Procedure

The repository contains 4 EF Core migrations:
1. `20260920130618_InitialCreate`
2. `20260920131521_AddInitialDomainModels`
3. `20260920150244_RefineDatabaseSchema`
4. `20260928181716_AddDatabasePerformanceIndexes`

### Recommended Deployment Method: Idempotent SQL Script
Generate an idempotent migration script before deployment:
```powershell
dotnet ef migrations script --idempotent `
  --project backend\DigitalShield.API\DigitalShield.API.csproj `
  --startup-project backend\DigitalShield.API\DigitalShield.API.csproj `
  -o staging-migration.sql
```
- Safe for repeated execution: checks `__EFMigrationsHistory` before executing each migration block.
- Can be reviewed by database administrators before execution.
- Can be executed via Azure CLI:
  ```bash
  az sql db execute --server <server-name> --database <db-name> --file staging-migration.sql
  ```

### Alternative Method: CI/CD Pipeline CLI Update
In an Azure DevOps or GitHub Actions pipeline:
```powershell
$env:ConnectionStrings__DefaultConnection = "<staging-connection-string>"
$env:Jwt__SecretKey = "<staging-secret-key>"
dotnet ef database update `
  --project backend\DigitalShield.API\DigitalShield.API.csproj `
  --startup-project backend\DigitalShield.API\DigitalShield.API.csproj
```

---

## 6. Phase 5 — Staging Smoke-Test Checklist

Once Vercel and Azure App Service deployments are live:

- [ ] **1. Public Health Check**:
  - `GET https://<app-service-domain>/api/health`
  - Expected: `200 OK` with `{"status":"ok","service":"DigitalShield API"}`.
- [ ] **2. CORS Verification**:
  - `OPTIONS https://<app-service-domain>/api/fraud/check` with `Origin: https://<vercel-domain>`.
  - Expected: `Access-Control-Allow-Origin: https://<vercel-domain>`.
- [ ] **3. User Registration & JWT Issuance**:
  - Open `https://<vercel-domain>/register`.
  - Register a test account. Verify redirection to `/dashboard` and JWT storage.
- [ ] **4. Scam Checker Analysis**:
  - Open `https://<vercel-domain>/scam-checker`.
  - Analyze a test phishing message. Verify server-side score and risk indicators.
- [ ] **5. Learning Modules & Progress**:
  - Open `https://<vercel-domain>/learn`.
  - Browse modules; click through to detail page; mark complete. Verify progress updates on Dashboard.
- [ ] **6. Step-by-Step Scenario Quiz**:
  - Open `https://<vercel-domain>/quizzes`.
  - Take a quiz through all questions. Submit answers. Verify results page displays server-calculated score.
- [ ] **7. Profile Update**:
  - Open `https://<vercel-domain>/profile`.
  - Change display name and save. Refresh page to confirm persistence in Azure SQL.
- [ ] **8. Security / Access Denied**:
  - Log in as regular user; navigate to `/admin`. Verify redirection to `/access-denied`.

---

## 7. Rollback & Troubleshooting Procedures

### Frontend Rollback (Vercel)
1. In the Vercel project dashboard, navigate to **Deployments**.
2. Locate the previous working deployment.
3. Click the menu `...` &rarr; **Promote to Production** (or Staging). Rollback is instantaneous.

### Backend Rollback (Azure App Service)
1. **Deployment Slots (Recommended)**: Use a `staging` slot on App Service. After smoke tests pass, swap slots (`az webapp deployment slot swap`). If an issue occurs, perform a reverse slot swap.
2. **Container / Package Rollback**: Re-deploy the previous release artifact or container image tag from your artifact registry.

### Database Rollback (Azure SQL)
1. Azure SQL maintains automated point-in-time backups.
2. In Azure Portal &rarr; Azure SQL Database &rarr; **Restore**, specify a restore point immediately prior to migration execution to restore to a new database.
3. Update App Service `ConnectionStrings__DefaultConnection` to point to the restored database.

---

## 8. Cost Management & Resource Cleanup

To prevent unnecessary cloud spend during staging:
1. **Azure SQL Serverless**:
   - Set **Auto-Pause Delay** to 1 hour. When idle, compute pauses and costs drop to storage-only (~$0.15/GB/month).
   - Set minimum vCore to `0.5` and maximum to `1`.
2. **App Service Plan**:
   - Scale staging to `B1` tier (~$13/month) instead of production multi-core tiers.
   - Stop or pause the App Service when staging testing cycles are inactive.
3. **Resource Group Teardown**:
   - Group all staging resources in a single resource group (`rg-digitalshield-staging`) so the entire environment can be deleted cleanly with a single command if discontinued:
     ```bash
     az group delete --name rg-digitalshield-staging --yes --no-wait
     ```
