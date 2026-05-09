# Waves API

Sample ASP.NET Core 9 API for **waves** of store pick orders: list waves, get a wave by id, and upsert a wave. The solution uses **versioned controllers**, **Entity Framework Core** (SQLite), **FluentValidation**, **MassTransit** (in-memory bus by default, optional **Azure Service Bus**), **Swagger/OpenAPI**, and **dummy Bearer authentication** for local demos.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)

## Run the API

From the repository root:

```bash
dotnet run --project ApiRefactor
```

Or open `ApiRefactor.sln` in Visual Studio / Rider and start the **ApiRefactor** project (profile **https** or **http**).

- **Swagger UI** (Development only): `https://localhost:7038/swagger` or `http://localhost:5057/swagger` (see [`ApiRefactor/Properties/launchSettings.json`](ApiRefactor/Properties/launchSettings.json) for your machine’s ports).

### Authentication (required for all wave endpoints)

The API expects a **Bearer token** that matches configuration.

1. Default token in [`ApiRefactor/appsettings.json`](ApiRefactor/appsettings.json):

   ```json
   "DummyAuth": {
     "BearerToken": "dev-dummy-token"
   }
   ```

2. **Swagger:** click **Authorize**, choose **Bearer**, enter `dev-dummy-token` (Swagger sends `Authorization: Bearer dev-dummy-token`).

3. **curl / HTTP clients:**

   ```bash
   curl -s -H "Authorization: Bearer dev-dummy-token" https://localhost:7038/api/v1/waves
   ```

   Use `-k` if you hit certificate warnings on HTTPS in development.

### API routes (v1)

| Method | Route | Description |
|--------|--------|----------------|
| `GET` | `/api/v1/waves` | List all waves |
| `GET` | `/api/v1/waves/{id}` | Get one wave (404 if missing) |
| `POST` | `/api/v1/waves` | Upsert wave (201 if inserted, 200 if updated) |

Request body for `POST` is JSON matching `UpsertWaveRequest` (`id` optional, `name`, `waveDate`). Successful upserts publish a **`WaveUpserted`** integration event via MassTransit.

## Configuration

| Section | Purpose |
|---------|---------|
| `WaveDatabase:ConnectionString` | SQLite file (default `App_Data/waves.db`; folder created on first run) |
| `DummyAuth:BearerToken` | Shared secret for demo Bearer auth |
| `MassTransit:Transport` | Empty = in-memory bus; `AzureServiceBus` + connection string for Azure Service Bus |

## Run tests

From the repository root:

```bash
dotnet test
```

Or:

```bash
dotnet test ApiRefactor.sln
```

Tests use **WebApplicationFactory**, an isolated temp SQLite database, a test-only bearer token (`test-dummy-token` in test configuration), and assert HTTP status codes, validation, and integration-event recording.

## Solution layout

- `ApiRefactor` — Web API project
- `ApiRefactor.Tests` — xUnit integration tests

---

*Original brief: fictional Coles-style waves API exercise focused on readable, maintainable, testable code.*
