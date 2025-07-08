
> **“Prove you need complexity before you pay for it.”**
> *Build a robust monolith, split only when you’ve proven the need.*

# SpireKit

**SpireKit** is a .NET backend foundation designed for sustainable scale. You get modular building blocks, a clean domain-first API model, and the freedom to stay monolithic until microservices are truly justified.

---

## 🏛️ Sections Overview

* **SpireCore:**
  Pure contracts, interfaces, events, and shared utilities.
* **SpireAPI:**
  DDD-inspired architecture—atomic Operations, endpoint auto-mapping, EF infrastructure, JWT, Swagger, DTO-to-TypeScript.
* **SpireCLI:**
  Composable, command-based CLI for all backend/admin tooling.

---

## 🧱 SpireCore

**Purpose:**
The shared backbone for all SpireKit modules.

### What you’ll find

* **Abstractions:**
  `ICreatedAt`, `IStateFlag`, `IHasId`, etc.

* **Events:**
  In-memory domain event dispatcher (`EventEmitter`, `IEventDispatcher`).

* **Utilities:**
  Guid helpers, constants, pagination contracts.

* **Automatic DI Registration:**
  SpireCore enables automatic dependency injection via marker interfaces:

  ```csharp
  public interface ITransientService { }
  public interface IScopedService { }
  public interface ISingletonService { }
  ```

  Any service implementing one of these will be auto-registered with the correct lifetime:

  ```csharp
  builder.Services.AddApplicationServices();
  ```

### What you won’t find

No entities, repositories, or DB logic.
*All other modules depend on SpireCore.*

---

## 📦 SpireAPI

### Project Folder Structure

SpireAPI is made up of several projects for clean separation:

* `SpireCore.API` — Shared cross-cutting concerns
* `Spire.Api.Application` — Application logic (Authentication module)
* `Spire.Api.Contracts` — DTOs shared across boundaries
* `Spire.Api.Host` — API startup project
* `Spire.Api.Infrastructure` — Persistence and EF Core configuration

Use the provided `Export-TemplateFromSpireApi.ps1` script to scaffold new templates from this structure.

---

### 🔐 Authentication Module

SpireAPI currently focuses on a single module: **Authentication**, which provides:

* JWT-based authentication
* User registration & login
* Refresh tokens
* Auditing and token lifecycle tracking

**Module Folder Layout:**

```
Modules/
└── Authentication/
    ├── Configuration/         # Auth settings & options
    ├── Domain/
    │   ├── Models/            # AuthUser, RefreshToken, Audit entities
    │   ├── Services/          # Auth domain services
    ├── Infrastructure/        # DbContext, EF configs, migrations
    ├── Operations/            # Endpoint definitions for login, register, etc.
    └── AuthenticationModuleExtensions.cs  # DI entry point
```

**Key Features:**

* Clean separation of domain logic and endpoints
* Auto-registered operations, no controller boilerplate
* Token refresh and revocation support
* Pluggable and extendable for custom auth flows

---

## 🌐 Domain-Driven Operations (API Endpoints)

Operations are first-class citizens—each one is an atomic use case, auto-mapped to HTTP endpoints.

```csharp
[OperationRoute("auth/login")]
[OperationMethod(OperationMethodType.POST)]
public class LoginOperation : IOperation<LoginRequestDto, AuthResponseDto>
{
    // Implementation
}
```

### Benefits

* No controllers
* Rich metadata via attributes
* Fully testable use cases
* Simple, composable, declarative

---

## 🖥 SpireCLI

A modular command system to streamline backend tools and automation.

* `ICommand` pattern
* Navigable CLI trees
* Reusable in admin panels or scripts

---

## 🚀 Usage

Until NuGet is available, reference locally:

```bash
dotnet add reference ../SpireKit/SpireCore.csproj
```

---

## 📝 The SpireKit Philosophy

* **Start monolithic, split when needed**
* **Prioritize domain logic**
* **Keep endpoints atomic and self-documenting**
* **Avoid over-engineering**
* **Make complexity earn its place**

---

## 🧩 Roadmap

* NuGet packaging
* Pluggable Identity providers
* Multi-tenant and account linking support
* Advanced session and audit tooling

---

**Build simple. Focus on the domain. Evolve only when necessary.**
— The SpireKit Philosophy
