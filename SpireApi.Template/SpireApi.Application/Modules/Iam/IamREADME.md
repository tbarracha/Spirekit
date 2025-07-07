# 🛡️ IAM – Identity & Access Management Module

## ‼️ AI Generated - Contains Wrong or Incomplete Information ‼️

> **Identity is the new perimeter.**
> SpireKit’s **IAM** module gives you a clear, extensible RBAC model—Groups, Roles, Permissions—ready to plug into any Spire-powered API.

---

## ✨ Why another IAM layer?

* **Decoupled from authentication** – Auth (JWT, refresh tokens, etc.) lives in the **Authentication** module; IAM focuses purely on *who can do what* once a user is known.
* **Vertical slices** – Each aggregate (Groups, Permissions, Roles, Users) is self-contained: entities ➜ repos ➜ DTOs ➜ operations.
* **RBAC done right** – Hierarchical groups, role inheritance, fine-grained permission scopes.
* **Event‐driven** – Listens to `AuthUserRegistered` and other domain events to auto-provision IAM users.

---

## 📂 Folder Layout (vertical, per-aggregate)

```
Modules/
└── Iam/
    ├── Configuration/
    │   └── IamSettings.cs
    ├── Domain/
    │   ├── Groups/                 # Group, GroupType, GroupMember…
    │   │   ├── Contexts/
    │   │   ├── Models/
    │   │   ├── Repositories/
    │   │   └── Dtos/
    │   ├── Permissions/            # Permission, PermissionScope…
    │   ├── Roles/                  # Role, RolePermission…
    │   ├── Users/                  # IamUser, UserRole…
    │   └── Services/               # Cross-aggregate orchestration (GroupService, …)
    ├── EventHandling/              # e.g. OnAuthUserRegisteredHandler
    ├── Infrastructure/             # BaseIamDbContext, generic repos
    ├── Operations/
    │   ├── Groups/                 # CreateGroupOperation, etc.
    │   ├── Roles/
    │   ├── Permissions/
    │   └── UserRoles/
    └── IamModuleExtensions.cs      # DI entry-point (AddIamModuleServices)
```

Each aggregate (`Groups`, `Roles`, `Permissions`, `Users`) keeps its own Models, Repositories, DTOs, and (optional) Contexts.
Operations (HTTP endpoints) live in `Operations/<Aggregate>` so the API surface mirrors the domain model.

---

## 🗞️ Core Concepts

| Concept              | Purpose                                                                            |
| -------------------- | ---------------------------------------------------------------------------------- |
| **Group**            | A collection of users (optionally hierarchical via *ParentGroupId*).               |
| **Role**             | Named set of permissions; can be attached to a user or to every member of a group. |
| **Permission**       | Atomic capability (`"Article.Edit"`, `"Invoice.Approve"`).                         |
| **PermissionScope**  | Narrower slice of a permission (e.g. *Own*, *All*, *Department*).                  |
| **UserRole**         | Links a user to a role; supports expiration & auditing.                            |
| **RolePermission**   | Many-to-many link; allows attaching scopes and fine-grained flags.                 |
| **GroupMemberAudit** | Immutable trail of joins, leaves, bans, role changes, etc.                         |

### Default workflow

1. **Auth module** verifies credentials, fires **`AuthUserRegistered`** when a new user signs up.
2. **Event handler** in IAM creates an `IamUser` row and optional default **Group** membership.
3. Admin (or automation) assigns **Roles** to the user or their **Group**.
4. Requests hit endpoints protected with `[OperationAuthorize("Permission=Article.Edit")]`.
5. Middleware asks `IamPermissionEvaluator` → checks Roles → Permissions → Scopes → ✅/❌.

---

## 🧩 Integration Events

* **`AuthUserRegistered`** → creates `IamUser`, assigns default Role/Group.
* **`GroupOwnershipTransferred`** → adjusts `GroupMemberAudit`, emits notification.
* **`PermissionScopeChanged`** → cache-busts permission evaluator.

---

## 🏗️ Persistence

* **DbContext:** `BaseIamDbContext` (inherits SpireCore’s `BaseAuditableDbContext`).
* **Migrations:** add via `dotnet ef migrations add AddIam` in `Modules/Iam/Infrastructure`.
* **Soft-delete:** every entity has `StateFlag` (`ACTIVE`, `DELETED`, `BANNED`, …).

---

## 🛣️ HTTP Endpoints (Operations)

| Verb   | Route                         | Operation Class                 | Permission               |
| ------ | ----------------------------- | ------------------------------- | ------------------------ |
| `POST` | `/api/groups`                 | `CreateGroupOperation`          | `Group.Create`           |
| `POST` | `/api/groups/{id}/members`    | `CreateGroupMemberOperation`    | `Group.ManageMembers`    |
| `GET`  | `/api/roles/{id}`             | `GetRoleByIdOperation`          | `Role.Read`              |
| `POST` | `/api/roles/{id}/permissions` | `CreateRolePermissionOperation` | `Role.ManagePermissions` |
| …      | …                             | …                               | …                        |

All operations are tiny classes implementing `IOperation<Req,Res>`; no controllers needed.

---

## 📐 Extending

* **Add a new aggregate** → Create `Domain/<Aggregate>` tree; scaffold operations under `Operations/<Aggregate>`.
* **Custom permission logic** → Implement `IPermissionEvaluator`, register override before calling `AddIamModuleServices`.

---

## 🛣️ Roadmap

* JSON-based **policy DSL** for complex conditions (time-gated, attribute-based).
* **Audit dashboards** via SpireCLI (`spire iam audits group {groupId}`).
* **GraphQL IAM explorer** to visualise role–permission inheritance.

---

**Secure by design, modular by default. Welcome to SpireKit IAM.**
