# SpireKit

> **Monolith First, Microservices When Proven**
> *“Prove you need complexity before you pay for it.”*

**SpireKit** is a foundational .NET library for building modular backends that scale from a maintainable monolith to decomposable microservices—without needless upfront complexity.
It centralizes reusable backend building blocks: robust entities, repositories, domain events, CLI utilities, DTO mappers, and more. Reference it directly for now; NuGet coming soon.

---

## 🏗️ Project Philosophy

* **Start simple, scale when needed:**
  Build your core product as a clean, maintainable monolith.
  Break into microservices only *after* complexity and team size demand it.
* **All code, no magic:**
  Everything in SpireKit is visible, overridable, and designed for direct reference.
* **Maximum reuse, minimal coupling:**
  Modules are swappable and self-contained, so future migration is easy.

---

## 📦 Key Features

* 🧱 **EF Core Entities & Repositories**
  Base entities (class & record), audit/state patterns, generic repositories, paginated queries, and multi-DbContext support.
* 🧪 **Domain Events**
  In-memory event dispatcher for clean, decoupled business logic.
* 🖥 **Console/CLI System**
  Command-driven menus, composable CLI trees, and contextual logging.
* 🔁 **TypeScript DTO Mapper**
  Automate TypeScript interface generation from C# DTOs—no manual duplication.
* ⚙️ **General Utilities**
  Guid helpers, state flag constants, Swagger enhancements, controller ordering, and more.

---

## 🗂️ Project Structure

```
SpireKit/
|   README.md
|   SpireCore.csproj
|
+-- Abstractions/         # Core interfaces (e.g., ICreatedAt, IStateFlag)
|   \-- Interfaces/
|
+-- API/
|   +-- EntityFramework/
|   |   +-- DbContexts/
|   |   +-- Entities/
|   |   +-- Repositories/
|   |   \-- Services/
|   +-- JWT/
|   |   +-- ServiceIdentity/
|   |   \-- UserIdentity/
|   +-- Operations/
|   |   +-- Attributes/
|   |   \-- Dtos/
|   +-- Services/
|   \-- Swagger/
|       \-- SwaggerControllerOrders/
|
+-- Commands/             # CLI system core
+-- Constants/            # e.g. StateFlags.cs
+-- Events/
|   \-- Dispatcher/
+-- Lists/
|   \-- Pagination/
+-- Mappings/
|   \-- Language/         # TypeScript mapper
+-- Utils/
\-- structure.txt
```

---

## 🚀 Getting Started

Until NuGet support is live, **add a project reference**:

```bash
dotnet add reference ../SpireKit/SpireCore.csproj
```

> (Path may vary depending on your solution layout.)

---

## ✅ Usage Examples

### Entity + Repository

```csharp
// Inherit full audit/state support for any entity:
public class User : BaseEntityClass<Guid>, ICreatedAt, IUpdatedAt, IStateFlag
{
    public string Username { get; set; }
    public string Email { get; set; }
}

// Plug into the generic repository:
public class UserRepository : BaseRepository<User, Guid, AppDbContext>
{
    public UserRepository(AppDbContext ctx) : base(ctx) { }
}
```

---

### DTO → TypeScript Mapping

```csharp
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
```

After running `TypescriptDtoMapper`:

```ts
export interface UserDto {
  id: string;
  username: string;
  email: string;
}
```

*Supports camelCase, nullability, and primitive mapping out of the box.*

---

## 📝 Philosophy in Practice

* **No premature microservices:**
  The project structure, CLI system, and repository patterns are all designed for seamless migration to microservices *only when justified*.
* **Everything is override-friendly:**
  You can swap out or extend any core part as your product matures.
* **“Own complexity only when it pays you back.”**

---

## 🧩 Planned

* **NuGet packaging** for clean consumption.
* **Example solution** for rapid adoption.
* **Docs**

---

## 🗣 Feedback

> SpireKit is used daily in production for backend and AI-centric projects.
> Suggestions or PRs welcome!

---

**Build clean. Build simple. Only split when you must.**
— *The SpireKit Philosophy*
