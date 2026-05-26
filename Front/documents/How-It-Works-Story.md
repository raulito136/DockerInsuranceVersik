# How the Application Works — A Story

> This document tells the story of the application.
> It uses simple English and a real example so you can understand **what** the system does and **why** each service exists.
> Read this before you read the technical requirements.

---

## The People in Our Story

| Name | Role |
|---|---|
| **Ana** | A customer. She is the owner of an airline company. She has an aviation insurance policy. |
| **Carlos** | An insurance reviewer. He works at the company and reviews claims. |
| **The App** | The Policy Claims Management application that you will build. |

---

## Part 1 — Ana Gets Insurance

Ana owns an airline company. She buys aviation insurance from the company.

A staff member opens the app and creates a **Policy Holder** record for Ana:

- Name: Ana García
- Email: ana@email.com
- Region: NSW

Then they create a **Policy** for Ana:

- Policy Number: `POL-2026-00001` *(the app generates this number automatically)*
- Type: `AVIATION` *(aviation insurance — for her aircraft)*
- Coverage Type: `FULL` *(all-risk coverage)*
- Coverage Amount: $50,000,000 *(the maximum the company will pay — aviation policies are very large)*
- Start Date: 1 January 2026
- End Date: 31 December 2026
- Status: `ACTIVE`

> **This is the Policies Service** — it stores information about policy holders and their policies.
> **Oscar** builds this service.

When the staff member saves the policy, the app needs to check two things:
- Is `AVIATION` a valid policy type? *(it asks the Reference Data Service)*
- Is `FULL` a valid coverage type? *(it asks the Reference Data Service again)*

Only if both answers are "yes" does the app save the policy.

> **This is the Reference Data Service** — it is a dictionary of valid codes used by the whole system.
> **Raúl** builds this service.

Ana now has an active aviation insurance policy. She goes back to her airline happy.

---

## Part 2 — Ana Has an Incident

Three months later, one of Ana's aircraft is damaged during a storm at the airport.

She opens the app and submits a **claim**.

She fills in:
- Policy Number: `POL-2026-00001` *(her policy)*
- Claim Date: 15 March 2026 *(the day of the incident)*
- Amount: $1,200,000 *(how much the repair costs)*
- Description: "Aircraft tail section damaged by severe storm while parked at Sydney Airport. Structural inspection required before return to service."

She clicks **Submit**.

> **This is the Claims Service** — it manages all claims in the system.
> **David** builds this service.

---

## Part 3 — The App Checks Everything

When Ana clicks Submit, the app does not save the claim immediately.
It checks several things first.

### Check 1 — Does the policy exist?

The Claims Service sends a request to the Policies Service:

> *"Is there a policy with number `POL-2026-00001`? And is it ACTIVE?"*

The Policies Service replies:

> *"Yes. The policy exists. It is ACTIVE. The coverage amount is $20,000."*

✅ Check passed.

---

### Check 2 — Is the amount valid?

Ana is asking for $1,200,000. The policy covers up to $50,000,000.

$1,200,000 is less than $50,000,000.

✅ Check passed.

---

### Check 3 — Is the initial status valid?

Every new claim starts with the status `SUBMITTED`.

The Claims Service asks the Reference Data Service:

> *"Is `SUBMITTED` a valid and active claim status?"*

The Reference Data Service replies:

> *"Yes."*

✅ Check passed.

---

### All checks passed — the claim is saved

The app creates the claim with:

- Claim Number: `CLM-2026-00001` *(the app generates this number automatically)*
- Status: `SUBMITTED`

Ana can now see her claim in the app. It says **Submitted**.

---

## Part 4 — Carlos Reviews the Claim

The next day, Carlos the reviewer opens the app. He sees Ana's claim waiting.

He reads the description. He decides to review it.

He changes the status from `SUBMITTED` to `UNDER_REVIEW`.

The app checks:
- Is this transition allowed? `SUBMITTED → UNDER_REVIEW` ✅ Yes, this is a valid step.

The claim is saved with the new status. Ana can see the updated status when she logs back in.

---

## Part 5 — Carlos Approves the Claim

Carlos looks at the photos of the damage. He checks the repair quote. Everything looks correct.

He changes the status from `UNDER_REVIEW` to `APPROVED`.

The app checks:
- Is this transition allowed? `UNDER_REVIEW → APPROVED` ✅ Yes.

The claim is saved with the new status.

---

## Part 6 — Payment is Made

The finance team processes the payment. They update the claim status from `APPROVED` to `PAID`.

The app checks:
- Is this transition allowed? `APPROVED → PAID` ✅ Yes.

The claim is updated to PAID status. Ana can see this in the app whenever she logs in.

The story is complete.

---

## What Happens if Something Goes Wrong?

### Wrong policy number

Ana makes a typo and writes `POL-9999-99999`.

The Claims Service asks the Policies Service about this number.
The Policies Service replies: *"This policy does not exist."*

The app returns an error to Ana:
```json
{ "errors": [{ "field": "PolicyNumber", "message": "Policy not found" }] }
```

Ana corrects the number and tries again.

---

### Amount too high

Ana writes $60,000,000. But her policy only covers up to $50,000,000.

The app returns an error:
```json
{ "errors": [{ "field": "Amount", "message": "Amount exceeds the policy coverage amount of $50,000,000" }] }
```

---

### Wrong status transition

Someone tries to change the status directly from `SUBMITTED` to `PAID`.

That is not allowed. The only valid path is:
```
SUBMITTED → UNDER_REVIEW → APPROVED → PAID
```

The app returns HTTP 422 (Unprocessable Entity).

---

### A service is not running

If the Policies Service is turned off, and Ana tries to submit a claim, the Claims Service cannot check the policy.

The Claims Service does not crash. It returns HTTP 503:
```json
{ "errors": [{ "field": "", "message": "Policies service is unavailable. Please try again later." }] }
```

---

## Summary — Each Service in One Sentence

| Service | What it does | Built by |
|---|---|---|
| **Reference Data** | Stores the valid codes for the system: status names, policy types, coverage types, and regions. It is like a dictionary. | Raúl |
| **Policies** | Stores policy holders and their insurance policies. It knows if a policy is active and how much it covers. | Oscar |
| **Claims** | Stores claims submitted by policy holders. It validates everything before saving. It controls the status workflow. | David |

---

## The Order Services Talk to Each Other

```
1. Ana submits a claim
      ↓
2. Claims Service asks Policies Service → "Is this policy valid?"
      ↓
3. Claims Service asks Reference Data Service → "Is this status code valid?"
      ↓
4. Claim is saved
      ↓
5. (When status changes) Claims Service tells Notifications Service → "Send a notification"
```

That is the whole system.

> **Remember:** Each service has its own database. They do not share data directly.
> They only talk to each other by sending HTTP requests — like sending a question and waiting for an answer.
