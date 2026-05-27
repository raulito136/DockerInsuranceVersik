# Reference Data Service

**Owner:** Raúl  
**Port:** 5003  
**Purpose:** Master data for lookups used by Claims and Policies services.

> Leaf service — does not call any other service.

---

## Entities

### ClaimStatus

| Field       | Type        | Constraints        |
| ----------- | ----------- | ------------------ |
| Id          | int         | PK, auto-increment |
| Code        | string(20)  | Required, Unique   |
| Name        | string(100) | Required           |
| Description | string(500) | Optional           |
| IsActive    | bool        | Default: true      |

### PolicyType

| Field       | Type        | Constraints        |
| ----------- | ----------- | ------------------ |
| Id          | int         | PK, auto-increment |
| Code        | string(20)  | Required, Unique   |
| Name        | string(100) | Required           |
| Description | string(500) | Optional           |
| IsActive    | bool        | Default: true      |

### CoverageType

| Field       | Type        | Constraints        |
| ----------- | ----------- | ------------------ |
| Id          | int         | PK, auto-increment |
| Code        | string(20)  | Required, Unique   |
| Name        | string(100) | Required           |
| Description | string(500) | Optional           |
| IsActive    | bool        | Default: true      |

### Region

| Field    | Type        | Constraints        |
| -------- | ----------- | ------------------ |
| Id       | int         | PK, auto-increment |
| Code     | string(10)  | Required, Unique   |
| Name     | string(100) | Required           |
| IsActive | bool        | Default: true      |

---

## Endpoints

### Claim Statuses

| Method | Route                                   | Description                                                   |
| ------ | --------------------------------------- | ------------------------------------------------------------- |
| GET    | `/api/v1/claim-statuses`                | List all (active by default, `?includeInactive=true` for all) |
| GET    | `/api/v1/claim-statuses/{id}`           | Get by ID                                                     |
| GET    | `/api/v1/claim-statuses/by-code/{code}` | Get by code (used by Claims service)                          |
| POST   | `/api/v1/claim-statuses`                | Create                                                        |
| PUT    | `/api/v1/claim-statuses/{id}`           | Update                                                        |
| DELETE | `/api/v1/claim-statuses/{id}`           | Soft delete (sets IsActive = false)                           |

### Policy Types

| Method | Route                                 | Description                            |
| ------ | ------------------------------------- | -------------------------------------- |
| GET    | `/api/v1/policy-types`                | List all                               |
| GET    | `/api/v1/policy-types/{id}`           | Get by ID                              |
| GET    | `/api/v1/policy-types/by-code/{code}` | Get by code (used by Policies service) |
| POST   | `/api/v1/policy-types`                | Create                                 |
| PUT    | `/api/v1/policy-types/{id}`           | Update                                 |
| DELETE | `/api/v1/policy-types/{id}`           | Soft delete                            |

### Coverage Types

| Method | Route                                   | Description                            |
| ------ | --------------------------------------- | -------------------------------------- |
| GET    | `/api/v1/coverage-types`                | List all                               |
| GET    | `/api/v1/coverage-types/{id}`           | Get by ID                              |
| GET    | `/api/v1/coverage-types/by-code/{code}` | Get by code (used by Policies service) |
| POST   | `/api/v1/coverage-types`                | Create                                 |
| PUT    | `/api/v1/coverage-types/{id}`           | Update                                 |
| DELETE | `/api/v1/coverage-types/{id}`           | Soft delete                            |

### Regions

| Method | Route                  | Description |
| ------ | ---------------------- | ----------- |
| GET    | `/api/v1/regions`      | List all    |
| GET    | `/api/v1/regions/{id}` | Get by ID   |
| POST   | `/api/v1/regions`      | Create      |
| PUT    | `/api/v1/regions/{id}` | Update      |
| DELETE | `/api/v1/regions/{id}` | Soft delete |

---

## Seed Data

| Entity         | Values                                                      |
| -------------- | ----------------------------------------------------------- |
| Claim Statuses | `SUBMITTED`, `UNDER_REVIEW`, `APPROVED`, `REJECTED`, `PAID` |
| Policy Types   | `AVIATION`, `ENERGY`, `MARINE`, `CYBER`, `LIABILITY`        |
| Coverage Types | `FULL`, `PARTIAL`, `THIRD_PARTY`, `CATASTROPHIC`            |
| Regions        | `NSW`, `VIC`, `QLD`, `WA`, `SA`, `TAS`, `ACT`, `NT`         |

---

## Inter-service HTTP Calls

_None — Reference Data is a leaf service; it does not call other services._

---

## Glossary

### Shared Fields (all Reference Data entities)

| Field         | Meaning                                                                                                                                                                                                                                                                 |
| ------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Id`          | Internal database identifier. Auto-assigned on creation. Never used in business logic or user-facing displays.                                                                                                                                                          |
| `Code`        | Short, uppercase string used as a stable key by other services (e.g. `SUBMITTED`, `AVIATION`, `NSW`). This is what the Claims and Policies services store and validate against — never the `Id`.                                                                        |
| `Name`        | Human-readable label shown in the UI (e.g. "Under Review", "Aviation").                                                                                                                                                                                                 |
| `Description` | Optional longer explanation of what the code means. Shown as a tooltip or help text in admin screens.                                                                                                                                                                   |
| `IsActive`    | Controls whether the code is available for use. Inactive codes still exist in the database but cannot be selected when creating or updating records. Deleting a reference data record performs a soft delete — it sets `IsActive = false` rather than removing the row. |

### ClaimStatus

Represents a stage in the claim lifecycle. The full set of valid statuses is seeded at startup. The Claims service validates every `StatusCode` against this table.

| Code           | Meaning                                                 |
| -------------- | ------------------------------------------------------- |
| `SUBMITTED`    | Claim has been filed and is awaiting review.            |
| `UNDER_REVIEW` | A reviewer is actively assessing the claim.             |
| `APPROVED`     | The claim has been accepted; payment will be processed. |
| `REJECTED`     | The claim has been denied; no payment will be made.     |
| `PAID`         | Payment has been issued to the policy holder.           |

### PolicyType

Describes the specialty insurance market a policy covers. The Policies service validates every `PolicyTypeCode` against this table. These types reflect the Lloyd's of London specialty market.

| Code        | Meaning                                                                                                                                                             |
| ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `AVIATION`  | Insurance for aircraft, airlines, airports, and aviation-related liability. A single policy can cover hundreds of millions of dollars.                              |
| `ENERGY`    | Insurance for oil rigs, wind farms, refineries, pipelines, and other energy infrastructure. Covers physical damage, pollution liability, and business interruption. |
| `MARINE`    | Insurance for ships, cargo, ports, and waterways. One of the oldest types of insurance in the Lloyd's market.                                                       |
| `CYBER`     | Insurance against data breaches, ransomware attacks, and digital infrastructure failures. A growing specialty line.                                                 |
| `LIABILITY` | General commercial liability insurance covering legal claims made against a business for injury or damage.                                                          |

### CoverageType

Describes the scope of coverage provided within a policy. The Policies service validates every `CoverageTypeCode` against this table.

| Code           | Meaning                                                                                                                         |
| -------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| `FULL`         | All-risk coverage. The insurer covers all causes of loss unless explicitly excluded. Highest premium.                           |
| `PARTIAL`      | Named-perils coverage. Only the specific risks listed in the policy are covered. Lower premium than FULL.                       |
| `THIRD_PARTY`  | Covers legal liability to third parties (e.g. other companies or individuals harmed). Does not cover the holder's own assets.   |
| `CATASTROPHIC` | Covers only extreme, large-scale loss events (e.g. total loss of an aircraft or oil platform). Lowest premium; high deductible. |

### Region

Represents an Australian state or territory. Used to record where a policy holder is located. The `Code` field uses standard Australian state abbreviations.

| Code  | Name                         |
| ----- | ---------------------------- |
| `NSW` | New South Wales              |
| `VIC` | Victoria                     |
| `QLD` | Queensland                   |
| `WA`  | Western Australia            |
| `SA`  | South Australia              |
| `TAS` | Tasmania                     |
| `ACT` | Australian Capital Territory |
| `NT`  | Northern Territory           |
