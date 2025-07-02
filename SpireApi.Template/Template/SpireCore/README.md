# SpireKit

**SpireKit** is a modular .NET library that centralizes reusable backend components for all \*Spire projects. It includes base entities, repository patterns, domain events, CLI utilities, and more — designed for direct project reference (NuGet support coming soon).

---

## 📦 Key Features

* 🧱 **Entity Framework Core Support**
  Base entities, generic repositories, pagination, and multi-context services.

* 🧪 **Domain Events**
  In-memory event dispatching for decoupled business logic.

* 🖥 **Console Utilities**
  Command-driven CLI tools with stateful menus and logging.

* 🔁 **TypeScript DTO Mapper**
  Auto-generate TypeScript interfaces from backend models.

* ⚙️ **Utilities & Extensions**
  Includes `GuidUtility`, Swagger UI enhancements, and controller ordering.

---

## 📁 Project Overview

```
SpireKit/
├── API/
│   ├── EntityFramework/         → EF Core entities, repositories, services
│   └── Extensions/Swagger/      → Swagger helpers and filters
├── Cli/                         → Console command system
├── Core/                        → Interfaces, constants, domain events
├── Mappings/                    → Cross-language mappers (e.g. TypeScript)
├── Utils/                       → General-purpose utilities
└── Spirekit.csproj
```

---

## 🔧 Usage

Until NuGet support is added, reference the project directly:

```bash
dotnet add reference ../Spirekit/Spirekit.csproj
```

---

## ✅ Example

### 🔹 Entity + Repository

```csharp
public class User : BaseEntityClass<Guid>, ICreatedAt, IUpdatedAt, IStateFlag
{
    public string Username { get; set; }
    public string Email { get; set; }
}
```

```csharp
public class UserRepository : BaseRepository<User, Guid, AppDbContext>
{
    public UserRepository(AppDbContext context) : base(context) {}
}
```

---

### 🔸 DTO → TypeScript Mapping

```csharp
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
```

After using `TypescriptDtoMapper`, this generates:

```ts
export interface UserDto {
  id: string;
  username: string;
  email: string;
}
```

> Supports options for camelCase conversion, nullable detection, and primitive type mapping.

