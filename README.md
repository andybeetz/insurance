# Insurance

A small ASP.NET Core (Minimal API) service for selling, retrieving, renewing, and cancelling insurance policies.

## Prerequisites

- **.NET SDK 10.0** (project targets `net10.0`)

## Running the API

### From the command line

```
dotnet run --project .\Insurance.Api\Insurance.Api.csproj
```
The API will start on the at http://localhost:5013 and is configured by ASP.NET Core (launchsettings.json).

The OpenAPI endpoint is http://localhost:5013/openapi/v1.json

### From Rider

- Select the **Insurance.Api** run configuration
- Click **Run** (or **Debug**)
- 
### Seeded sample policies (for manual renewal testing)

When the API starts, it seeds the in-memory policy store with a few sample policies and logs their **UniqueReference** IDs.

Look for log lines like:

```
Seeded sample Household policies: 04b85c45-ae24-4744-b8e1-55fba4b7db59, bd970234-411f-49a0-b4da-12256c9d709c
Seeded sample BuyToLet policies: 1dc5f655-4001-4480-9e94-1c9fbf1037be, 2e98e788-0aa3-447c-8562-30699c0fed06
```

Use those GUIDs with the existing endpoints:

- View policy details:
    - `GET /policies/v1/household/{uniqueReference:guid}`
    - `GET /policies/v1/buytolet/{uniqueReference:guid}`

- Renew a policy:
    - `PATCH /policies/v1/household`
    - `PATCH /policies/v1/buytolet`

Example (replace `<HOUSEHOLD_GUID_FROM_LOGS>` and `<PAYMENT_GUID>`):

```
GET http://localhost:5013/policies/v1/household/<HOUSEHOLD_GUID_FROM_LOGS>
```

```json
{ 
  "uniqueReference": "<HOUSEHOLD_GUID_FROM_LOGS>", 
  "startDate": "2024-01-01", 
  "endDate": "2025-01-01", 
  "amount": 77.00, 
  "hasClaims": false, 
  "autoRenew": true, 
  "policyHolders": [], 
  "property": { "addressLine1": "1 Sample Street", "addressLine2": null, "addressLine3": null, "postCode": "ZZ1 1ZZ" }, 
  "payments": []
}
```

### OpenAPI (Swagger)

OpenAPI is enabled in **Development**. When the app is running in Development, the OpenAPI endpoint is mapped via `app.MapOpenApi()`.

If you don’t see it:
- ensure `ASPNETCORE_ENVIRONMENT=Development`
- and rerun the API

Once the API is running, you can visit http://localhost:5013/swagger/index.html to explore the API.

## API Endpoints (v1)

Base paths:

- `POST /policies/v1/household` — sell a household policy
- `POST /policies/v1/buytolet` — sell a buy-to-let policy
- `GET /policies/v1/household/{uniqueReference:guid}` — retrieve a household policy
- `GET /policies/v1/buytolet/{uniqueReference:guid}` — retrieve a buy-to-let policy
- `PATCH /policies/v1/household` — renew a household policy
- `PATCH /policies/v1/buytolet` — renew a buy-to-let policy
- `DELETE /policies/v1/household/{uniqueReference:guid}` — cancel a household policy
- `DELETE /policies/v1/buytolet/{uniqueReference:guid}` — cancel a buy-to-let policy

## Making requests

There’s an HTTP scratch file at:

- `Insurance.Api/Insurance.Api.http`

You can run requests from Rider using the built-in HTTP client.
(You may need to adjust the base address/port to match the URL your app is running on.)

## Testing

This repo contains:

- **Unit tests**: `Insurance.Api.Tests.Unit`, `Insurance.Domain.Tests.Unit`
- **Integration tests**: `Insurance.Api.Tests.Integration`

### Run all tests

```
dotnet test
```

### Run a specific test project

```
dotnet test .\Insurance.Api.Tests.Unit\Insurance.Api.Tests.Unit.csproj
dotnet test .\Insurance.Api.Tests.Integration\Insurance.Api.Tests.Integration.csproj
dotnet test .\Insurance.Domain.Tests.Unit\Insurance.Domain.Tests.Unit.csproj
```

### Test framework + tooling

Tests use **NUnit** and `dotnet test` via `Microsoft.NET.Test.Sdk`.
Coverage collection is available via the `coverlet.collector` package (configure as needed for your workflow).