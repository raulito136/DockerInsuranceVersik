# Frontend Microfrontend Plan — Policy Claims Management System

## 1. Overview

We are extending the intern project with a microfrontend (MFE) layer on top of the three backend services already being built.

**What we're building:** Four Angular applications — one shell that hosts the UI, and three remote microfrontends (one per backend service). Each MFE is an independently deployable Angular app that the shell lazy-loads at runtime via Module Federation.

**Why microfrontends?**

The three backend services are intentionally decoupled by domain. The frontend mirrors this boundary: one MFE per backend service. The shell just provides the frame.

This architecture reflects patterns used in enterprise frontend development. The learning goal is not just Angular — it is understanding how to decompose a UI by domain boundary, align frontend ownership with backend ownership, and connect independent apps at runtime without a build-time dependency between them.

This is NOT about over-engineering a small project. It is about practising skills used at scale.

---

## 2. Architecture Diagram

```text
┌─────────────────────────────────────────────────────────────────────────┐
│                         mfe-shell  (:4200)                              │
│          App layout · Navigation · Routing · Lazy-loads remotes         │
│                                                                         │
│   ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐       │
│   │ mfe-reference-  │   │  mfe-policies   │   │   mfe-claims    │       │
│   │ data  (:4201)   │   │    (:4202)      │   │    (:4203)      │       │
│   │                 │   │                 │   │                 │       │
│   └────────┬────────┘   └────────┬────────┘   └────────┬────────┘       │
└────────────┼─────────────────────┼─────────────────────┼────────────────┘
             │ HTTP                │ HTTP                │ HTTP
             ▼                     ▼                     ▼
    ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
    │ Reference Data  │  │    Policies     │  │     Claims      │
    │  Service :5003  │  │  Service :5002  │  │  Service :5001  │
    └─────────────────┘  └───────┬─────────┘  └─────────┬───────┘
                                 │ (validates)          │ (validates)
                                 └──────────────────────┤
                                                        │
                                             ┌──────────▼──────────┐
                                             │  Reference Data     │
                                             │   Service :5003     │
                                             └─────────────────────┘
```

**Key rule:** Each MFE calls only its own backend service. The MFEs do NOT call each other. Cross-service validation is the backend's responsibility. If the claims MFE needs to display a policy number, it gets it from the Claims API response — the Claims service already resolved it from the Policies service.

---

## 3. Technology Decisions

### 3.1 Module Federation Tooling

**Use `@angular-architects/module-federation`** — this is the Webpack-based implementation of Module Federation. Angular 19 uses esbuild by default, so this package requires configuring the custom Webpack builder (`@angular-builders/custom-webpack`). This is standard practice for teams using Module Federation with Angular.

The Nx `@nx/angular` plugin has built-in generators for Module Federation that handle the Webpack config — use those, do not configure Webpack manually.

Learn about this: [`@angular-architects/module-federation` docs](https://github.com/angular-architects/module-federation-plugin)  
Learn about this: [Nx Module Federation generators](https://nx.dev/nx-api/angular/generators/setup-mf)  
Learn about this: [Module Federation with Angular (Manfred Steyer)](https://www.angulararchitects.io/blog/micro-frontends-with-modern-angular-part-1-standalone-and-esbuild/)

### 3.2 Monorepo

**Use Nx monorepo.** One workspace, four Angular apps (`mfe-shell`, `mfe-reference-data`, `mfe-policies`, `mfe-claims`), one shared library (`shared-models`). Period.

Research this: [Nx Angular Monorepo Tutorial](https://nx.dev/getting-started/tutorials/angular-monorepo-tutorial)  
Research this: [Nx + Module Federation](https://nx.dev/concepts/module-federation/micro-frontend-architecture)

### 3.3 Angular Version

Use **Angular 19**. Use **standalone components** throughout — no NgModules. This is the modern Angular pattern.

Research this: [Angular Standalone Components overview](https://v19.angular.dev/guide/components)

---

## 4. Timeline Integration

| Week | Dates        | Phase                            | Focus                                                                                                                                    | Deliverable                                                       |
| ---- | ------------ | -------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- |
| 1–2  | Apr 16–27    | Backend (FIXED)                  | Microservices foundation                                                                                                                 | Three working services with databases                             |
| 3    | Apr 30–May 4 | Backend (FIXED)                  | Testing & polish                                                                                                                         | Unit tests (70%), error handling, validation                      |
| 4    | May 7–11     | **Frontend — Setup**             | Nx workspace init; Module Federation configured; all 4 apps scaffold; standalone routing confirmed; each MFE runs standalone on its port | All 4 Angular apps boot and display a placeholder screen          |
| 5    | May 14–18    | **Frontend — Features**          | MFE feature pages built; Angular services wired to their backend API; forms, lists, and detail views working                             | All CRUD pages working when MFEs run standalone                   |
| 6    | May 21–29    | **Frontend — Integration & E2E** | Shell loads all three remotes via routing; Playwright E2E tests (10+); loading/error states; final demo                                  | Full application working end-to-end; 10+ Playwright tests passing |

**Week 4 is a shared setup week.** All three interns work together on Wednesday to verify every MFE loads inside the shell before diverging. Do not skip this checkpoint.

---

## 5. Ownership Matrix

| MFE                  | Pages / Views                                                                                                             | Backend Service                            |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------ |
| `mfe-reference-data` | Claim Statuses list/create/edit; Policy Types list/create/edit; Coverage Types list/create/edit; Regions list/create/edit | Reference Data Service `:5003`             |
| `mfe-policies`       | Policy Holders list/create/edit/detail; Policies list/create/edit/detail; Policy + holder filtered view                   | Policies Service `:5002`                   |
| `mfe-claims`         | Claims list (filterable); Submit claim; Claim detail with comments and audit trail; Status update workflow                | Claims Service `:5001`                     |
| `mfe-shell`          | App layout; top navigation; sidebar; dashboard/home; remote routing config                                                | None directly — routes into the three MFEs |

**Shell:** The shell is built collaboratively during Week 4. Each team member contributes the route registration for their remote.

---

## 6. Shell (`mfe-shell`)

### Responsibilities

The shell does exactly three things:

1. **Provides the app frame** — header, navigation, layout. Every page in the app renders inside this frame.
2. **Owns the top-level router** — it defines routes for `/reference-data/**`, `/policies/**`, `/policy-holders/**`, and `/claims/**`. Each route lazy-loads the corresponding remote's exposed module.
3. **Loads remote entries** — at startup (or on first navigation), the shell fetches `remoteEntry.json` from each MFE and registers their exposed modules.

### What the shell does NOT do

- It does not contain any business logic.
- It does not call any backend API directly.
- It does not duplicate pages owned by a remote.

### Routing config overview

In the shell's routes file, each remote is registered as a lazy-loaded route using `@angular-architects/module-federation`'s `loadRemoteModule` helper. The route path matches the remote's internal routing prefix.

Key config properties to research: `remoteEntry`, `exposedModule`, `loadRemoteModule`

Research this: [Official plugin tutorial — shell setup, loadRemoteModule, routing](https://github.com/angular-architects/module-federation-plugin/blob/main/libs/mf/tutorial/tutorial.md)  
Research this: [Nx — Creating a Module Federation Host](https://nx.dev/docs/technologies/module-federation/guides/create-a-host)

### Layout guidance

- Header: app name + top-level navigation links (Reference Data / Policies / Claims)
- Content area: `<router-outlet>` — this is where remote components render
- No footer needed for this project
- Use Angular's built-in routing, no third-party UI framework unless the team agrees on one together

---

## 7. Reference Data MFE (`mfe-reference-data`)

**Port: 4201**  
**Backend: Reference Data Service `:5003`**

### Pages and Routes

| Route                                     | View        | Description                                                |
| ----------------------------------------- | ----------- | ---------------------------------------------------------- |
| `/reference-data`                         | Redirect    | Redirect to `/reference-data/claim-statuses`               |
| `/reference-data/claim-statuses`          | List        | Table of all claim statuses; toggle active/inactive filter |
| `/reference-data/claim-statuses/new`      | Create form | Add a new claim status                                     |
| `/reference-data/claim-statuses/:id/edit` | Edit form   | Edit name/description; soft-delete button                  |
| `/reference-data/policy-types`            | List        | Table of policy types with active toggle                   |
| `/reference-data/policy-types/new`        | Create form | Add new policy type                                        |
| `/reference-data/policy-types/:id/edit`   | Edit form   | Edit/deactivate                                            |
| `/reference-data/coverage-types`          | List        | Table of coverage types                                    |
| `/reference-data/coverage-types/new`      | Create form |                                                            |
| `/reference-data/coverage-types/:id/edit` | Edit form   |                                                            |
| `/reference-data/regions`                 | List        | Table of regions (read-heavy; rarely edited)               |
| `/reference-data/regions/new`             | Create form |                                                            |
| `/reference-data/regions/:id/edit`        | Edit form   |                                                            |

### HTTP Targets

All calls go to `http://localhost:5003`. Define one Angular service per entity group (`ClaimStatusService`, `PolicyTypeService`, `CoverageTypeService`, `RegionService`). Each service maps to the endpoints documented in `requirements/01-reference-data-service.md`.

### Suggested Folder Structure

```text
src/app/
  features/
    claim-statuses/
      claim-status-list/
      claim-status-form/
    policy-types/
      policy-type-list/
      policy-type-form/
    coverage-types/
      coverage-type-list/
      coverage-type-form/
    regions/
      region-list/
      region-form/
  services/
    claim-status.service.ts
    policy-type.service.ts
    coverage-type.service.ts
    region.service.ts
  models/
    claim-status.model.ts
    policy-type.model.ts
    coverage-type.model.ts
    region.model.ts
  app.routes.ts
  app.config.ts
```

### Notes

- These are lookup tables. The lists are simple — a table with Code, Name, Active status, and Edit/Delete buttons.
- The soft-delete action calls `DELETE /api/v1/{entity}/{id}` which sets `IsActive = false`. The UI should reflect this with a greyed-out row or a restore button.
- No pagination needed here — reference data sets are small.

---

## 8. Policies MFE (`mfe-policies`)

**Port: 4202**  
**Backend: Policies Service `:5002`**

### Pages and Routes

| Route                      | View        | Description                                                                                           |
| -------------------------- | ----------- | ----------------------------------------------------------------------------------------------------- |
| `/policies`                | List        | Paginated table of policies; filter by `status` and `policyTypeCode`                                  |
| `/policies/new`            | Create form | Create a policy; select policy holder from dropdown; enter policy type, coverage type, dates, amounts |
| `/policies/:id`            | Detail      | Full policy view with policy holder info                                                              |
| `/policies/:id/edit`       | Edit form   | Edit policy fields                                                                                    |
| `/policy-holders`          | List        | Paginated table of policy holders                                                                     |
| `/policy-holders/new`      | Create form | Create a policy holder                                                                                |
| `/policy-holders/:id`      | Detail      | Policy holder profile + list of their policies                                                        |
| `/policy-holders/:id/edit` | Edit form   | Edit policy holder fields                                                                             |

### HTTP Targets

All calls go to `http://localhost:5002`. Define two Angular services: `PolicyHolderService` and `PolicyService`.

For the create-policy form, the policy type and coverage type dropdowns should be populated from hardcoded seed values (or fetched from Reference Data — but that requires cross-MFE HTTP, which is discussed in Section 10). Recommended approach: fetch them from the Reference Data API directly in the `PolicyService` — this is a direct backend call, not an MFE-to-MFE call.

### Suggested Folder Structure

```text
src/app/
  features/
    policies/
      policy-list/
      policy-detail/
      policy-form/
    policy-holders/
      policy-holder-list/
      policy-holder-detail/
      policy-holder-form/
  services/
    policy.service.ts
    policy-holder.service.ts
    reference-data.service.ts   ← calls :5003 for dropdowns only
  models/
    policy.model.ts
    policy-holder.model.ts
  app.routes.ts
  app.config.ts
```

### Notes

- The policies list needs pagination. Consume `page` and `pageSize` query params. Show total and page controls.
- `PolicyNumber` is auto-generated by the backend — do not show this field in the create form.
- The policy holder dropdown in the create-policy form should fetch from `GET /api/v1/policy-holders` and render as a searchable select.
- Status field (`ACTIVE`, `INACTIVE`, `EXPIRED`, `CANCELLED`) is set on create/edit — render as a dropdown with these four fixed values.

---

## 9. Claims MFE (`mfe-claims`)

**Port: 4203**  
**Backend: Claims Service `:5001`**

### Pages and Routes

| Route                | View              | Description                                                  |
| -------------------- | ----------------- | ------------------------------------------------------------ |
| `/claims`            | List              | Paginated, filterable by `statusCode` and `policyNumber`     |
| `/claims/new`        | Submit claim form | Enter policy number, claim date, amount, description         |
| `/claims/:id`        | Detail            | Full claim view with comments and audit trail tabs           |
| `/claims/:id/status` | Status update     | Reviewer workflow: current status + allowed next transitions |

### HTTP Targets

All calls go to `http://localhost:5001`. Define: `ClaimService`, `ClaimCommentService`.

### Suggested Folder Structure

```text
src/app/
  features/
    claims/
      claim-list/
      claim-detail/
      claim-form/
      claim-status-update/
    comments/
      comment-list/
      comment-form/
    audit/
      audit-trail/
  services/
    claim.service.ts
    claim-comment.service.ts
  models/
    claim.model.ts
    claim-comment.model.ts
    claim-audit.model.ts
  app.routes.ts
  app.config.ts
```

### Notes

- The claim detail page has three sections: claim info, comments, and audit trail. Use tabs or accordion.
- The status update view is the most complex piece. It must show the current status and only offer the transitions that are valid per the workflow: `SUBMITTED → UNDER_REVIEW → APPROVED → PAID` and `UNDER_REVIEW → REJECTED`. Do NOT hard-code status values in the UI — display them; the validation happens on the backend.
- The claim submit form takes a `policyNumber` as free text. The backend validates it. Show backend error messages inline.
- Comments cannot be edited — only posted or deleted.
- The audit trail is read-only. Render it as a chronological timeline or a simple table.

---

## 10. Shared Concerns

### Inter-MFE Communication

The MFEs in this project have minimal need to talk to each other. Design them so each MFE gets all the data it needs from its own backend. If the claims MFE needs policy information, it gets it from the Claims API response (which already called the Policies service). The frontend does not replicate inter-service calls.

The one exception is dropdowns that list reference data items (e.g., policy type codes in the Policies MFE form). The correct approach is for that MFE to call the Reference Data API directly — this is a direct HTTP call to `:5003`, not communication between `mfe-policies` and `mfe-reference-data` Angular apps.

**If you ever need genuine cross-MFE communication** (e.g., to broadcast a "claim status changed" event so the shell can display a notification): use a shared service in the `shared-models` Nx library. This library is loaded by both apps. The service uses a `BehaviorSubject` to publish events.

Research this: [Official plugin tutorial — Step 5: Communication between MFEs via shared library service](https://github.com/angular-architects/module-federation-plugin/blob/main/libs/mf/tutorial/tutorial.md#step-5-communication-between-micro-frontends-and-sharing-monorepo-libraries)  
Research this: [RxJS BehaviorSubject](https://rxjs.dev/api/index/class/BehaviorSubject) — the primitive used to build a shared event bus in a shared Nx library

### Shared Library (`shared-models`)

Create an Nx library `shared-models` that contains:

| File                       | Purpose                                                                                             |
| -------------------------- | --------------------------------------------------------------------------------------------------- |
| `api-response.model.ts`    | TypeScript interface for the standard `{ data, errors, page, pageSize, total }` envelope            |
| `api-error.interceptor.ts` | Angular `HttpInterceptor` that catches 4xx/5xx and transforms them into a consistent error object   |
| `loading.service.ts`       | Optional — a simple `BehaviorSubject<boolean>` to show/hide a global loading indicator in the shell |

Research this: [Nx library generation](https://nx.dev/nx-api/angular/generators/library)

### Error Handling Pattern

All backend APIs return errors in the envelope: `{ "errors": [{ "field": "...", "message": "..." }] }`. Every Angular service in every MFE must handle this consistently.

The pattern:

- On error, catch the HTTP response in the service
- Extract the `errors` array from the response body
- Surface it to the component as an array of error messages
- The component renders errors inline (next to the field or as a summary list)

Do not use `alert()` or browser native dialogs. Do not let unhandled HTTP errors propagate silently.

---

## 11. Local Dev Setup

### Port Conventions

| App                    | Port | Notes                                               |
| ---------------------- | ---- | --------------------------------------------------- |
| `mfe-shell`            | 4200 | Entry point for the full app                        |
| `mfe-reference-data`   | 4201 | Standalone dev port                                 |
| `mfe-policies`         | 4202 | Standalone dev port                                 |
| `mfe-claims`           | 4203 | Standalone dev port                                 |
| Reference Data Service | 5003 | Must be running when any MFE is active              |
| Policies Service       | 5002 | Must be running for `mfe-policies` and `mfe-claims` |
| Claims Service         | 5001 | Must be running for `mfe-claims`                    |

### Running an MFE Standalone

Each MFE can run independently of the shell. When running standalone, the MFE boots with its own `app.config.ts` and its own routes. This is how interns develop and test during Week 5.

Run: `nx serve mfe-reference-data` — app appears at `http://localhost:4201`

The MFE's own router provides full navigation. The shell is not involved. This is the primary dev loop.

### Running Inside the Shell

When running inside the shell, the shell loads the MFE's `remoteEntry.json` at `http://localhost:4201/remoteEntry.json` and lazy-loads the exposed module when the user navigates to `/reference-data/...`.

To run the full integrated app: start all three MFE dev servers first, then start the shell.

In an Nx workspace: `nx run-many --target=serve --projects=mfe-shell,mfe-reference-data,mfe-policies,mfe-claims`

Research this: [Nx `run-many` command](https://nx.dev/nx-api/nx/documents/run-many)

### CORS

The backend services must allow requests from the Angular dev ports (`localhost:4200`–`4203`). Each intern adds a CORS policy in their service's `Program.cs` that allows `http://localhost:4200`, `http://localhost:4201`, `http://localhost:4202`, `http://localhost:4203`.

---

## 12. Learning Resources

Read these before and during the frontend weeks. You are not expected to understand them fully before starting — read once, start building, come back when stuck.

### Angular Fundamentals

| Topic                          | Resource                                             |
| ------------------------------ | ---------------------------------------------------- |
| Angular standalone components  | <https://v19.angular.dev/guide/components>           |
| Angular routing                | <https://v19.angular.dev/guide/routing>              |
| Angular `HttpClient`           | <https://v19.angular.dev/guide/http>                 |
| Angular reactive forms         | <https://v19.angular.dev/guide/forms/reactive-forms> |
| Angular signals (modern state) | <https://v19.angular.dev/guide/signals>              |

### Microfrontend Architecture

| Topic                                           | Resource                                                                                                   |
| ----------------------------------------------- | ---------------------------------------------------------------------------------------------------------- |
| What are microfrontends?                        | <https://micro-frontends.org/>                                                                             |
| Module Federation concept                       | <https://webpack.js.org/concepts/module-federation/>                                                       |
| `@angular-architects/module-federation` docs    | <https://github.com/angular-architects/module-federation-plugin>                                           |
| Nx Module Federation generators                 | <https://nx.dev/nx-api/angular/generators/setup-mf>                                                        |
| Module Federation with Angular (Manfred Steyer) | <https://www.angulararchitects.io/blog/micro-frontends-with-modern-angular-part-1-standalone-and-esbuild/> |

### Nx Monorepo

| Topic                  | Resource                                                                |
| ---------------------- | ----------------------------------------------------------------------- |
| Nx Angular tutorial    | <https://nx.dev/getting-started/tutorials/angular-monorepo-tutorial>    |
| Nx + Module Federation | <https://nx.dev/concepts/module-federation/micro-frontend-architecture> |
| Nx library generation  | <https://nx.dev/nx-api/angular/generators/library>                      |

### Cross-MFE Communication

| Topic                                     | Resource                                                                                                                                                                            |
| ----------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Communication via shared library (Step 5) | <https://github.com/angular-architects/module-federation-plugin/blob/main/libs/mf/tutorial/tutorial.md#step-5-communication-between-micro-frontends-and-sharing-monorepo-libraries> |
| RxJS BehaviorSubject                      | <https://rxjs.dev/api/index/class/BehaviorSubject>                                                                                                                                  |

---
