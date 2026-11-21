# Render PostgreSQL Deployment

The API uses PostgreSQL through Entity Framework Core and the Npgsql provider.
The root `render.yaml` Blueprint provisions a Render PostgreSQL database and
connects it to the Docker-based API service.

## Deployment

1. Push `render.yaml` and `backend/DigitalShield.API/Dockerfile` to the Git
   repository.
2. In Render, create a Blueprint from that repository and select the branch.
3. During Blueprint setup, provide `Cors__AllowedOrigins` as the exact
   production Vercel origin, without a trailing slash.
4. Deploy. The Blueprint generates a JWT signing secret and wires the
   PostgreSQL connection URL to the API.
5. On startup, the API applies its migrations and seeds reference content.
   After the first successful seed, set `DatabaseSeeding__Enabled` to `false`.
6. Verify `https://<service>.onrender.com/api/health`, then set the frontend's
   `VITE_API_URL` to `https://<service>.onrender.com/api` and redeploy it.

The service is configured on Render's free plan. Review the current limits and
retention for free services and databases before relying on this setup for
production data.

## Local Development

`docker-compose.yml` starts a local PostgreSQL database with the development
credentials expected by the API's Development fallback:

```powershell
docker compose up -d postgres
dotnet run --project backend\DigitalShield.API\DigitalShield.API.csproj
```

To apply migrations to another database, set
`ConnectionStrings__DefaultConnection` to an Npgsql connection string. The API
only applies migrations automatically when
`DatabaseMigrations__ApplyOnStartup` is `true`.

Never commit production database credentials or JWT secrets.
