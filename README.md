# SpireKit

**SpireKit** is a modular, reusable base library designed to unify and streamline shared logic across all Genspire .NET projects. It provides core building blocks such as base entity definitions, repository patterns, utility extensions, and standardized interfaces.

---

## 📦 Features

- 🧱 Base entity and interface contracts (`ICreatedAt`, `IUpdatedAt`, `IStateFlag`)
- 🧮 Generic repository implementations for Entity Framework Core
- 🧰 Common utilities and extension methods
- 📐 Shared constants, enums, and value types
- 📦 Designed for easy integration via NuGet

---

## 🔧 Installation

You can reference SpireKit in your projects either by:

### 1. Direct Project Reference
```bash
dotnet add reference ../SpireKit/SpireKit.csproj
````

### 2. Via NuGet (coming soon)

```bash
dotnet add package SpireKit
```

---

## 🗂 Project Structure

```
SpireKit/
├── Entities/
├── Interfaces/
├── Repositories/
├── Extensions/
├── Constants/
└── SpireKit.csproj
```

---

## ✅ Usage Example

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

## 🛡 License

MIT License — feel free to use, modify, and distribute.

---

## 👨‍💻 Maintained by

[Genspire Projects](https://github.com/yourusername)
