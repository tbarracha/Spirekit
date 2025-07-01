## 🚀 How to Run `spire` from Any Shell

### 1. Pack your tool into a NuGet package

From your `SpireCLI` directory:

```bash
dotnet pack -c Release
```

This creates a `.nupkg` file under `bin/Release`.

---

### 2. Install or Upgrade as a Global Tool

#### 🔁 First-time install:

```bash
dotnet tool install --global --add-source ./bin/Release SpireCLI
```

#### ⬆️ Upgrade if already installed:

```bash
dotnet tool update --global --add-source ./bin/Release SpireCLI
```

> ✅ This will pick up your newly packed version and replace the old one.

---

### 3. Verify Installation

Open a **new shell** (to ensure `PATH` is reloaded), then run:

```bash
spire help
```

---

### 🧪 Local Install (for testing without affecting global tools)

```bash
dotnet tool install --tool-path ./tools --add-source ./bin/Release SpireCLI
./tools/spire help
```

---

### 🧼 Uninstall

#### ❌ Remove the global install:

```bash
dotnet tool uninstall --global SpireCLI
```

#### ❌ Remove a local install:

```bash
rm -rf ./tools
```