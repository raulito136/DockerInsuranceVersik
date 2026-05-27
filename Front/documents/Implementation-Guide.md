# Implementation Guide — Policy Claims Management Microservices

This guide walks you through building three services from scratch. Read it fully before writing any code. Each section tells you **what** to do and **why**, in the right order.

---

## Before You Start

### Tools you need installed

- .NET 9 SDK
- Visual Studio 2022 or VS Code with the C# extension
- SQL Server or SQL Server Express (each service gets its own database)
- A REST client — Postman or the `.http` files feature in VS Code

### Understand the dependency order

The services depend on each other. Build them in this order so you always have a real service to call:

```
1. Reference Data Service   (no dependencies — start here)
2. Policies Service         (depends on Reference Data)
3. Claims Service           (depends on Policies and Reference Data)
```

> **Rule:** Never mock a downstream service by faking its behaviour in your own code. Run the real service locally.

---

## Phase 1 — Project Setup (do this for every service)

### 1. Create the solution and projects

Each service is a separate .NET solution. Use the following project structure inside the solution:

```
ServiceName.sln
├── ServiceName.Api          ← ASP.NET Core Web API (entry point)
├── ServiceName.Application  ← Business logic, interfaces, DTOs
├── ServiceName.Domain       ← Entities (plain C# classes)
└── ServiceName.Infrastructure ← EF Core DbContext, repositories, HTTP clients
```

Create each project as a class library, except `Api` which is a Web API project.

### 2. Add project references

- `Api` references `Application` and `Infrastructure`
- `Infrastructure` references `Application`
- `Application` references `Domain`
- `Domain` references nothing

This keeps your business logic free of framework concerns.

### 3. NuGet packages to add

| Package                                   | Where          | Purpose                                    |
| ----------------------------------------- | -------------- | ------------------------------------------ |
| `Microsoft.EntityFrameworkCore.SqlServer` | Infrastructure | Database access                            |
| `Microsoft.EntityFrameworkCore.Tools`     | Infrastructure | Migrations via CLI                         |
| `Refit`                                   | Infrastructure | Typed HTTP clients for inter-service calls |
| `Refit.HttpClientFactory`                 | Infrastructure | Integrates Refit with `IHttpClientFactory` |

---

## Phase 2 — Domain Layer

Define your entities as plain C# classes with no special attributes or base classes. Each entity maps directly to a database table. Use the field names and types exactly as listed in the requirements for your service.

Rules:

- `DateTime` fields use `DateTime` (UTC, set in the application layer — never in the entity itself)
- `DateOnly` fields use `DateOnly`
- `decimal` fields use `decimal`
- Do not put any business logic inside entity classes

---

## Phase 3 — Infrastructure Layer

### Database context

Create one `DbContext` per service. Register each entity as a `DbSet<T>`. Use EF Core Fluent API (an `IEntityTypeConfiguration<T>` class per entity) to configure column lengths, unique constraints, and default values. Do not use data annotation attributes on entities.

### Migrations

Once your `DbContext` and entity configurations are in place, create and apply your first migration. Run migrations every time you change your entity model. Each service has its own database — never share a database between services.

### Seed data (Reference Data service only)

The Reference Data service must seed its tables with the values listed in the requirements on first run. Use EF Core's `HasData` in your entity configurations or a startup migration to insert the seed records.

---

## Phase 4 — Application Layer

This layer contains:

- **DTOs (Data Transfer Objects):** Request and response models. Never return raw entity objects from your controllers.
- **Interfaces:** Define an interface for every repository and every HTTP client. This makes unit testing straightforward.
- **Service classes:** Contain the business logic. Each method validates input, calls repositories or HTTP clients, and returns a result. Return a typed result object — do not throw exceptions for business rule failures.

---

## Phase 5 — Inter-service HTTP Clients (Refit)

Services that need to call another service must use **Refit** to define typed HTTP clients.

### How Refit works (concept only)

Refit lets you define an interface where each method corresponds to one HTTP endpoint of the target service. You annotate the methods with HTTP verb attributes and the route. Refit generates the implementation automatically at runtime — you never write `HttpClient` request/response code by hand.

### Where to put Refit interfaces

Define the Refit interface in your `Application` layer (it's a contract/interface). Put the Refit registration and any configuration in your `Infrastructure` layer.

### Registration

Register Refit clients in `Program.cs` using `AddRefitClient<IYourClient>()` combined with `ConfigureHttpClient(...)` to set the base address from configuration. Read the base URL from `appsettings.json` under the `Services` key.

### Error handling

When a downstream service returns an error or is unreachable, your service must not let the exception propagate to the caller. Catch it, log it, and return an HTTP 503 with a descriptive message.

### Which services need Refit clients

| Service        | Refit clients to create                   |
| -------------- | ----------------------------------------- |
| Policies       | `IReferenceDataClient`                    |
| Claims         | `IPoliciesClient`, `IReferenceDataClient` |
| Reference Data | None                                      |

Define only the methods you actually call — not every endpoint the target service exposes.

---

## Phase 6 — API Layer

### Controllers

One controller per entity group (e.g., `PoliciesController`, `PolicyHoldersController`). Keep controllers thin — they receive the request, call the application service, and return the result. No business logic in controllers.

### Response envelope

Every response must follow the standard envelope defined in the requirements:

- Success: `{ "data": ... }`
- Error: `{ "errors": [{ "field": "...", "message": "..." }] }`
- Paginated: `{ "data": [...], "page": 1, "pageSize": 20, "total": 100 }`

Create shared response wrapper classes in your `Application` layer and use them consistently.

### API versioning

All routes must be prefixed with `/api/v1/`. Configure API versioning in `Program.cs`.

---

## Phase 7 — Build and Test Order

Follow this sequence to avoid being blocked:

### Step 1 — Reference Data Service (Raúl)

1. Set up the project structure
2. Create entities, DbContext, and seed data
3. Implement CRUD endpoints for all four entity types
4. Verify seed data loads on startup using Postman
5. Keep it running on port 5003

### Step 2 — Policies Service (Oscar)

1. Set up the project structure
2. Create entities and DbContext
3. Create Refit client for Reference Data
4. Implement policy holder and policy CRUD
5. Add validation calls to Reference Data before saving
6. Test by creating a policy holder and policy with Postman (Reference Data must be running)

### Step 3 — Claims Service (David)

1. Set up the project structure
2. Create entities and DbContext
3. Create Refit client for Policies and Reference Data
4. Implement claims CRUD
5. Implement the status workflow validation (only the defined transitions are valid)
6. Test the full flow end to end with Postman (Policies and Reference Data services must be running)

---

## Common Mistakes to Avoid

| Mistake                                            | What to do instead                                    |
| -------------------------------------------------- | ----------------------------------------------------- |
| Sharing a database between services                | Each service has its own database                     |
| Returning entity objects directly from controllers | Always map to a DTO                                   |
| Calling `HttpClient` directly without Refit        | Use a Refit interface                                 |
| Letting downstream HTTP errors crash your service  | Catch the Refit exception and return HTTP 503         |
| Hardcoding service URLs in code                    | Always read from `appsettings.json`                   |
| Skipping migrations and using `EnsureCreated`      | Use proper EF Core migrations                         |
| Putting business logic in controllers              | Business logic belongs in application service classes |
| Using `DateTime.Now`                               | Use UTC (`DateTime.UtcNow`) or inject a time provider |

---

## Team Coordination Checklist

Because some services depend on others, agree on these things as a team **before building**:

- [ ] Confirm everyone is using the same port numbers (5001–5004)
- [ ] Agree on the `appsettings.json` `Services` section format (copy exactly from the requirements)
- [ ] Raúl starts Reference Data first so Oscar and David can test against real data early
- [ ] Share Postman collections or `.http` test files so each person can test another's service
- [ ] Define the response envelope format together and create a shared NuGet package or copy the wrapper classes to each service
- [ ] Test inter-service flows together locally before the end of the week
