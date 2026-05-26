# Policies Service

**Owner:** Oscar  
**Port:** 5002  
**Purpose:** Manages insurance policies and policy holders.

---

## Entities

### PolicyHolder

| Field       | Type        | Constraints                                 |
| ----------- | ----------- | ------------------------------------------- |
| Id          | int         | PK, auto-increment                          |
| FirstName   | string(100) | Required                                    |
| LastName    | string(100) | Required                                    |
| Email       | string(255) | Required, Unique                            |
| Phone       | string(20)  | Optional                                    |
| DateOfBirth | DateOnly    | Required                                    |
| RegionCode  | string(10)  | Required — validated against Reference Data |
| CreatedAt   | DateTime    | Set on create                               |
| UpdatedAt   | DateTime    | Set on update                               |

### Policy

| Field            | Type          | Constraints                                            |
| ---------------- | ------------- | ------------------------------------------------------ |
| Id               | int           | PK, auto-increment                                     |
| PolicyNumber     | string(20)    | Required, Unique, auto-generated                       |
| PolicyHolderId   | int           | FK → PolicyHolder                                      |
| PolicyTypeCode   | string(20)    | Required — validated against Reference Data            |
| CoverageTypeCode | string(20)    | Required — validated against Reference Data            |
| CoverageAmount   | decimal(18,2) | Required, > 0                                          |
| PremiumAmount    | decimal(18,2) | Required, > 0                                          |
| StartDate        | DateOnly      | Required                                               |
| EndDate          | DateOnly      | Required, > StartDate                                  |
| Status           | string(20)    | Required: `ACTIVE`, `INACTIVE`, `EXPIRED`, `CANCELLED` |
| CreatedAt        | DateTime      | Set on create                                          |
| UpdatedAt        | DateTime      | Set on update                                          |

---

## Endpoints

### Policy Holders

| Method | Route                         | Description                                 |
| ------ | ----------------------------- | ------------------------------------------- |
| GET    | `/api/v1/policy-holders`      | List all (paginated: `?page=1&pageSize=20`) |
| GET    | `/api/v1/policy-holders/{id}` | Get by ID (includes policies)               |
| POST   | `/api/v1/policy-holders`      | Create                                      |
| PUT    | `/api/v1/policy-holders/{id}` | Update                                      |
| DELETE | `/api/v1/policy-holders/{id}` | Delete (only if no active policies)         |

### Policies

| Method | Route                                       | Description                                                                |
| ------ | ------------------------------------------- | -------------------------------------------------------------------------- |
| GET    | `/api/v1/policies`                          | List all (paginated, filterable: `?status=ACTIVE&policyTypeCode=AVIATION`) |
| GET    | `/api/v1/policies/{id}`                     | Get by ID                                                                  |
| GET    | `/api/v1/policies/by-number/{policyNumber}` | Get by policy number (used by Claims service)                              |
| POST   | `/api/v1/policies`                          | Create                                                                     |
| PUT    | `/api/v1/policies/{id}`                     | Update                                                                     |
| DELETE | `/api/v1/policies/{id}`                     | Delete (only if no associated claims)                                      |

---

## Validation Rules

- `PolicyTypeCode` must exist and be active → HTTP GET Reference Data `/api/v1/policy-types/by-code/{code}`
- `CoverageTypeCode` must exist and be active → HTTP GET Reference Data `/api/v1/coverage-types/by-code/{code}`
- `RegionCode` on PolicyHolder must exist → HTTP GET Reference Data `/api/v1/regions`
- `EndDate` must be after `StartDate`
- Cannot delete a PolicyHolder who has policies

---

## Inter-service HTTP Calls

| Trigger                    | Target Service        | Endpoint                                    | Purpose                          |
| -------------------------- | --------------------- | ------------------------------------------- | -------------------------------- |
| Create/Update Policy       | Reference Data (Raúl) | `GET /api/v1/policy-types/by-code/{code}`   | Validate PolicyTypeCode exists   |
| Create/Update Policy       | Reference Data (Raúl) | `GET /api/v1/coverage-types/by-code/{code}` | Validate CoverageTypeCode exists |
| Create/Update PolicyHolder | Reference Data (Raúl) | `GET /api/v1/regions`                       | Validate RegionCode exists       |

### `appsettings.json`

```json
{
    "Services": {
        "ReferenceData": "http://localhost:5003"
    }
}
```

---

## Glossary

### PolicyHolder Fields

| Field         | Meaning                                                                                                                                             |
| ------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Id`          | Internal database identifier. Auto-assigned on creation.                                                                                            |
| `FirstName`   | The policy holder's given name. Up to 100 characters.                                                                                               |
| `LastName`    | The policy holder's family name. Up to 100 characters.                                                                                              |
| `Email`       | The policy holder's email address. Must be unique across all policy holders. Used as the key for notification preferences and claim correspondence. |
| `Phone`       | Optional contact phone number. Up to 20 characters.                                                                                                 |
| `DateOfBirth` | The policy holder's date of birth. Date only — no time component. Used for age verification and eligibility checks.                                 |
| `RegionCode`  | The Australian state or territory where the policy holder resides (e.g. `NSW`, `VIC`). Must match an active code in the Reference Data service.     |
| `CreatedAt`   | Timestamp (UTC) when the record was first created. Set automatically by the server.                                                                 |
| `UpdatedAt`   | Timestamp (UTC) of the most recent modification. Updated automatically on every write.                                                              |

### Policy Fields

| Field              | Meaning                                                                                                                                                                 |
| ------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Id`               | Internal database identifier. Auto-assigned on creation.                                                                                                                |
| `PolicyNumber`     | Human-readable unique identifier for the policy. Auto-generated by the server (e.g. `POL-2024-00001`). Used when referencing the policy from a claim.                   |
| `PolicyHolderId`   | The `Id` of the `PolicyHolder` who owns this policy. Links the policy to its owner.                                                                                     |
| `PolicyTypeCode`   | The specialty insurance category (e.g. `AVIATION`, `ENERGY`). Must match an active code in the Reference Data service.                                                  |
| `CoverageTypeCode` | The scope of coverage for this policy (e.g. `FULL`, `CATASTROPHIC`). Must match an active code in the Reference Data service.                                           |
| `CoverageAmount`   | The maximum dollar amount the insurer will pay out across all claims on this policy. Claims cannot exceed this value. Stored with up to 18 digits and 2 decimal places. |
| `PremiumAmount`    | The recurring dollar amount the policy holder pays for this policy (e.g. monthly or annual premium).                                                                    |
| `StartDate`        | The date the policy coverage begins. Date only.                                                                                                                         |
| `EndDate`          | The date the policy coverage ends. Must be after `StartDate`.                                                                                                           |
| `Status`           | The current state of the policy. Only `ACTIVE` policies can have new claims filed against them. See _Policy Status Codes_ below.                                        |
| `CreatedAt`        | Timestamp (UTC) when the record was first created. Set automatically by the server.                                                                                     |
| `UpdatedAt`        | Timestamp (UTC) of the most recent modification. Updated automatically on every write.                                                                                  |

### Policy Status Codes

| Code        | Meaning                                                                         |
| ----------- | ------------------------------------------------------------------------------- |
| `ACTIVE`    | The policy is current and in force. Claims can be filed against it.             |
| `INACTIVE`  | The policy exists but has been temporarily suspended. Claims cannot be filed.   |
| `EXPIRED`   | The policy's `EndDate` has passed. No further claims are accepted.              |
| `CANCELLED` | The policy was terminated before its `EndDate`. No further claims are accepted. |

### Policy Type Codes

For reference — defined and owned by the Reference Data service.

| Code        | Meaning                                                      |
| ----------- | ------------------------------------------------------------ |
| `AVIATION`  | Aircraft, airlines, airports, and aviation liability.        |
| `ENERGY`    | Oil rigs, wind farms, refineries, and energy infrastructure. |
| `MARINE`    | Ships, cargo, ports, and waterways.                          |
| `CYBER`     | Data breaches, ransomware, and digital infrastructure.       |
| `LIABILITY` | General commercial liability against legal claims.           |

### Coverage Type Codes

For reference — defined and owned by the Reference Data service.

| Code           | Meaning                                                    |
| -------------- | ---------------------------------------------------------- |
| `FULL`         | All-risk coverage. Highest premium.                        |
| `PARTIAL`      | Named-perils only. Lower premium.                          |
| `THIRD_PARTY`  | Liability to others only; does not cover own assets.       |
| `CATASTROPHIC` | Extreme loss events only. Lowest premium; high deductible. |
