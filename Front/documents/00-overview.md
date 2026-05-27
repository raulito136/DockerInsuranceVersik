# Microservices Requirements — Policy Claims Management

## Participants

| Intern | Service                |
| ------ | ---------------------- |
| Raúl   | Reference Data Service |
| Oscar  | Policies Service       |
| David  | Claims Service         |

## Architecture Overview

Three microservices communicating over HTTP. Each service owns its own database.
Inter-service calls are synchronous HTTP (Refit typed clients).

```
┌─────────────────┐     validates policy      ┌─────────────────┐
│  Claims Service │ ─────────────────────────▶ │ Policies Service│
│                 │                             │                 │
│                 │ ──┐ validates status type   │                 │ ──┐ validates
└─────────────────┘   │                         └─────────────────┘   │ policy type
        │             ▼                                 │              ▼
        │   ┌──────────────────────┐                   │   ┌──────────────────────┐
        └──▶│ Reference Data Svc   │ ◀─────────────────┘   │ Reference Data Svc   │
            └──────────────────────┘                       └──────────────────────┘
```

## Service Ports

| Service        | Port | Intern |
| -------------- | ---- | ------ |
| Claims         | 5001 | David  |
| Policies       | 5002 | Oscar  |
| Reference Data | 5003 | Raúl   |

---

## Inter-service Communication Summary

```
Reference Data  ◀── Policies  ◀── Claims
     ▲                        │
     └────────────────────────┘
```

| Caller   | Called         | When                                     |
| -------- | -------------- | ---------------------------------------- |
| Policies | Reference Data | On create/update policy or policy holder |
| Claims   | Policies       | On create/update claim                   |
| Claims   | Reference Data | On create/update claim status            |

### HTTP Client Configuration

Each service must configure typed `HttpClient` registrations in `Program.cs`:

```csharp
// Example in Policies service
builder.Services.AddHttpClient<IReferenceDataClient, ReferenceDataClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:ReferenceData"]));

// Example in Claims service
builder.Services.AddHttpClient<IPoliciesClient, PoliciesClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Policies"]));

builder.Services.AddHttpClient<IReferenceDataClient, ReferenceDataClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:ReferenceData"]));
```

### `appsettings.json` — Service URLs

```json
{
    "Services": {
        "Policies": "http://localhost:5002",
        "ReferenceData": "http://localhost:5003"
    }
}
```

---

## Common Standards Across All Services

- All endpoints return standard envelope: `{ "data": ..., "errors": [...] }`
- Pagination response includes: `{ "data": [...], "page": 1, "pageSize": 20, "total": 100 }`
- All 4xx errors return: `{ "errors": [{ "field": "PolicyNumber", "message": "Policy not found" }] }`
- Soft deletes preferred over hard deletes where noted
- All inter-service HTTP calls return a typed error if the downstream service is unavailable (HTTP 503 with descriptive message — do **not** let exceptions propagate)
