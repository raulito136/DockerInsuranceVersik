# Claims Service

**Owner:** David  
**Port:** 5001  
**Purpose:** Manages insurance claims submitted against policies.

---

## Entities

### Claim

| Field        | Type          | Constraints                                                               |
| ------------ | ------------- | ------------------------------------------------------------------------- |
| Id           | int           | PK, auto-increment                                                        |
| ClaimNumber  | string(20)    | Required, Unique, auto-generated (`CLM-{year}-{sequence}`)                |
| PolicyNumber | string(20)    | Required — validated against Policies service                             |
| ClaimDate    | DateOnly      | Required                                                                  |
| StatusCode   | string(20)    | Required — validated against Reference Data service, default: `SUBMITTED` |
| Amount       | decimal(18,2) | Required, > 0                                                             |
| Description  | string(1000)  | Required                                                                  |
| CreatedAt    | DateTime      | Set on create                                                             |
| UpdatedAt    | DateTime      | Set on update                                                             |

### ClaimComment

| Field      | Type         | Constraints        |
| ---------- | ------------ | ------------------ |
| Id         | int          | PK, auto-increment |
| ClaimId    | int          | FK → Claim         |
| AuthorName | string(100)  | Required           |
| Comment    | string(2000) | Required           |
| CreatedAt  | DateTime     | Set on create      |

### ClaimAudit

| Field        | Type        | Constraints        |
| ------------ | ----------- | ------------------ |
| Id           | int         | PK, auto-increment |
| ClaimId      | int         | FK → Claim         |
| ChangedBy    | string(100) | Required           |
| FieldChanged | string(100) | Required           |
| OldValue     | string(500) | Nullable           |
| NewValue     | string(500) | Nullable           |
| ChangedAt    | DateTime    | Set on create      |

---

## Endpoints

### Claims

| Method | Route                                    | Description                                                                                           |
| ------ | ---------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| GET    | `/api/v1/claims`                         | List all (paginated: `?page=1&pageSize=20`, filterable: `?statusCode=SUBMITTED&policyNumber=POL-001`) |
| GET    | `/api/v1/claims/{id}`                    | Get by ID (includes comments and audit trail)                                                         |
| GET    | `/api/v1/claims/by-number/{claimNumber}` | Get by claim number                                                                                   |
| POST   | `/api/v1/claims`                         | Create                                                                                                |
| PUT    | `/api/v1/claims/{id}`                    | Update                                                                                                |
| PATCH  | `/api/v1/claims/{id}/status`             | Update status only (triggers notification)                                                            |
| DELETE | `/api/v1/claims/{id}`                    | Delete (only allowed when status is `SUBMITTED`)                                                      |

### Claim Comments

| Method | Route                                      | Description                   |
| ------ | ------------------------------------------ | ----------------------------- |
| GET    | `/api/v1/claims/{id}/comments`             | List all comments for a claim |
| POST   | `/api/v1/claims/{id}/comments`             | Add a comment                 |
| DELETE | `/api/v1/claims/{id}/comments/{commentId}` | Delete a comment              |

### Claim Audit

| Method | Route                       | Description                      |
| ------ | --------------------------- | -------------------------------- |
| GET    | `/api/v1/claims/{id}/audit` | Get full audit trail for a claim |

---

## Status Workflow

```
SUBMITTED → UNDER_REVIEW → APPROVED → PAID
                         ↘ REJECTED
```

Only the above transitions are valid. Any other transition must return HTTP 422.

---

## Validation Rules

- `PolicyNumber` must exist and have status `ACTIVE` → HTTP GET Policies `/api/v1/policies/by-number/{policyNumber}`
- `StatusCode` must exist and be active → HTTP GET Reference Data `/api/v1/claim-statuses/by-code/{code}`
- `Amount` must not exceed the policy's `CoverageAmount` (retrieved from Policies service)
- Cannot delete a claim that is not in `SUBMITTED` status

---

## Inter-service HTTP Calls

| Trigger             | Target Service        | Endpoint                                        | Purpose                                          |
| ------------------- | --------------------- | ----------------------------------------------- | ------------------------------------------------ |
| Create/Update Claim | Policies (Oscar)      | `GET /api/v1/policies/by-number/{policyNumber}` | Validate policy is ACTIVE and get CoverageAmount |
| Create/Update Claim | Reference Data (Raúl) | `GET /api/v1/claim-statuses/by-code/{code}`     | Validate status code exists                      |
| PATCH status        | Notifications (All)   | `POST /api/v1/notifications`                    | Notify policy holder of status change            |

### `appsettings.json`

```json
{
    "Services": {
        "Policies": "http://localhost:5002",
        "ReferenceData": "http://localhost:5003",
        "Notifications": "http://localhost:5004"
    }
}
```

---

## Glossary

### Claim Fields

| Field          | Meaning                                                                                                                                                                                     |
| -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Id`           | Internal database identifier. Auto-assigned by the database on creation. Never exposed in business logic or user-facing displays.                                                           |
| `ClaimNumber`  | Human-readable unique identifier for a claim. Auto-generated in the format `CLM-{year}-{sequence}` (e.g. `CLM-2026-00042`). Used when referencing a claim in communications or the UI.      |
| `PolicyNumber` | The number of the insurance policy this claim is filed against. Must match an existing active policy in the Policies service (e.g. `POL-2024-00001`).                                       |
| `ClaimDate`    | The date the insured event occurred (not the date the claim was submitted). Date only — no time component.                                                                                  |
| `StatusCode`   | The current stage of the claim in the workflow. Must be a valid code from the Reference Data service. Starts at `SUBMITTED` and follows a defined progression. See _Status Workflow_ above. |
| `Amount`       | The monetary amount being claimed, in dollars. Must be greater than zero and cannot exceed the policy's `CoverageAmount`. Stored with up to 18 digits and 2 decimal places.                 |
| `Description`  | A free-text explanation of what happened and why the claim is being filed. Up to 1,000 characters.                                                                                          |
| `CreatedAt`    | Timestamp (UTC) when the claim record was first created. Set automatically by the server — never accepted from the client.                                                                  |
| `UpdatedAt`    | Timestamp (UTC) of the most recent modification to the claim. Updated automatically on every write.                                                                                         |

### ClaimComment Fields

| Field        | Meaning                                                                                                                |
| ------------ | ---------------------------------------------------------------------------------------------------------------------- |
| `Id`         | Internal identifier for the comment. Auto-assigned.                                                                    |
| `ClaimId`    | The `Id` of the claim this comment belongs to. Links the comment back to its parent claim.                             |
| `AuthorName` | Display name of the person who wrote the comment (e.g. the logged-in user's name). Free text, up to 100 characters.    |
| `Comment`    | The body of the comment. Up to 2,000 characters. Used to communicate updates, questions, or decisions about the claim. |
| `CreatedAt`  | Timestamp (UTC) when the comment was posted. Set automatically. Comments cannot be edited — only deleted.              |

### ClaimAudit Fields

| Field          | Meaning                                                                                                       |
| -------------- | ------------------------------------------------------------------------------------------------------------- |
| `Id`           | Internal identifier for the audit record. Auto-assigned.                                                      |
| `ClaimId`      | The `Id` of the claim this audit entry belongs to.                                                            |
| `ChangedBy`    | Name or identifier of the user or system that made the change.                                                |
| `FieldChanged` | The name of the field that was modified (e.g. `StatusCode`, `Amount`).                                        |
| `OldValue`     | The value of the field before the change. Stored as a string. Nullable — will be empty on first-time sets.    |
| `NewValue`     | The value of the field after the change. Stored as a string. Nullable — will be empty if a field was cleared. |
| `ChangedAt`    | Timestamp (UTC) when the change occurred. Set automatically.                                                  |

### Status Codes

| Code           | Meaning                                                                                         |
| -------------- | ----------------------------------------------------------------------------------------------- |
| `SUBMITTED`    | The claim has been filed and is awaiting review. This is the initial state of every new claim.  |
| `UNDER_REVIEW` | A reviewer has picked up the claim and is actively assessing it.                                |
| `APPROVED`     | The claim has been accepted. The insurer will proceed with processing payment.                  |
| `REJECTED`     | The claim has been denied. No payment will be made. The reason should be recorded in a comment. |
| `PAID`         | Payment has been issued to the policy holder. This is the terminal state for approved claims.   |
