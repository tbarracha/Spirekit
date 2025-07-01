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

---

## 🔧 Using Scripts from `/scripts`

For convenience, several PowerShell scripts are included in the `/scripts` folder to automate common operations:

#### **Update the Global Tool After Packing**

```powershell
# From your SpireCLI/scripts directory:
.\update-spirecli.ps1
```

* **What it does:**
  Packs your tool, then updates the global install to your latest build.

---

#### **Uninstall, Repack, and Reinstall from Scratch**

```powershell
# From your SpireCLI/scripts directory:
.\reinstall-spirecli.ps1
```

* **What it does:**
  Uninstalls the global tool if present, repacks your NuGet package, and reinstalls globally from your latest build.

---

#### **Bump the Version, Pack, and Update Automatically**

```powershell
# From your SpireCLI/scripts directory:

# Bump major version (e.g., 1.2.3 → 2.0.0)
.\upgrade-spirecli.ps1 --major

# Bump minor version (e.g., 1.2.3 → 1.3.0)
.\upgrade-spirecli.ps1 --minor

# Bump patch version (e.g., 1.2.3 → 1.2.4)
.\upgrade-spirecli.ps1 --patch
```

* **What it does:**
  Bumps the chosen version field in your `.csproj`, repacks, and updates the tool in one step.

---

### ⚡ **Why use these scripts?**

* No need to memorize long `dotnet` commands!
* Instantly bump, pack, and update your tool with a single script call.
* Prevents mistakes and ensures your CLI is always up to date.

---

**Tip:**
If you add more automation, just include them in `/scripts` and update this doc!
