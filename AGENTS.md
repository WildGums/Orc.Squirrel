# Orc.Squirrel

Orc.Squirrel is a library that adds extra features on top of [Squirrel](https://github.com/Squirrel/Squirrel.Windows) to allow application updates to come from different channels.

Orc.Squirrel consists of 2 sub-projects:

- **Orc.Squirrel** — Core library with update service and channel management functionality.
- **Orc.Squirrel.Xaml** — WPF UI components for update dialogs and views.

---

## Critical Rules (Read First)

These rules are **non-negotiable**. Violating them causes broken builds, crashes, or downstream breakage.

### 1. Never Edit Generated Files

Files matching `*.generated.cs`, `*.generated.xaml` are auto-generated.

- **NEVER** manually edit these files

### 2. ABI / API Stability

This project maintains stable ABI / API. Breaking changes break downstream apps.

| Allowed | Never |
|---------|-------|
| Add new overloads | Modify existing signatures |
| Add new methods | Remove public APIs |
| Add new classes | Change return types |

### 3. Tests Are Mandatory

**Building alone is NOT sufficient.** Run tests before claiming completion (see [Commands](#commands)).

### 4. Branch Protection (COMPLIANCE REQUIRED)

**Direct commits to protected branches are a policy violation.**

| Repository | Protected Branches |
|------------|-------------------|
| Orc.Squirrel | `master` |
| Orc.Squirrel | `develop` |

**Required workflow:**

1. **Create a feature branch FIRST** — Use naming convention: `feature/issue-NNNN-description` (where `NNNN` is the GitHub issue number)
2. **Make all commits on the feature branch** — Never commit directly to protected branches
3. **Submit a Pull Request** — Changes must be reviewed by a human before merging

```bash
# CORRECT — Always create a feature branch first
git checkout -b feature/issue-1234-fix-description

# NEVER DO THIS — Policy violation
git checkout develop && git commit  # FORBIDDEN

# NEVER DO THIS — Policy violation
git checkout master && git commit  # FORBIDDEN
```

The repository has protected branches that must be respected.

---

## Commands

Single source of truth for all commands:

| Task | Command |
|------|---------|
| **Build** | `dotnet cake --target=build` |
| **Test** | `dotnet cake --target=test` |
| **Build and test** | `dotnet cake --target=buildandtest` |

---

## Architecture & Directories

### Solution Overview

```
Orc.Squirrel      => Core update service, channel management, Squirrel/Velopack integration
Orc.Squirrel.Xaml => WPF views and view models for update dialogs
```

### Directory Guide

| Directory / Pattern | Editable? | Notes |
|---------------------|-----------|-------|
| `*.generated.cs` | No | Leave as-is |
| `*.generated.xaml` | No | Leave as-is |
| `deployment/` | No | Deployment / build scripts |
| `src/Orc.Squirrel/` | Yes | Core library code |
| `src/Orc.Squirrel.Xaml/` | Yes | WPF UI components |
| `src/Orc.Squirrel.Tests/` | Yes | Unit and integration tests |

---

## Writing Code

### Anti-Patterns (Never Do This)

| Anti-Pattern | Why |
|-------------|-----|
| Modifying method signatures | ABI breaking |
| Manual edits to `*.generated.cs`, `*.generated.xaml` | Overwritten on regenerate |
| Using default parameters in public APIs | ABI breaking |
| **Skipping failing tests** | **Unacceptable — tests must pass** |

---

## Testing & Debugging

### Running Tests

```bash
dotnet cake --target=test
```

### Tests MUST Pass

> **NON-NEGOTIABLE:** Tests must PASS before claiming completion.
>
> - Do NOT skip failing tests
> - Do NOT claim completion if tests fail
> - Do NOT use `SkipException` to work around failures

### Writing Tests

1. Use NUnit to write tests
2. Group tests in a nested `Facts` class named after the feature under test
3. Combine Pascal / Snake case for test methods (e.g. `Feature_Does_Work`)

```csharp
[Test]
public void Feature_Does_Work()
{
    var result = 47 - 5;

    Assert.That(result, Is.EqualTo(42));
}
```

Public API stability is verified using snapshot tests (`PublicApiFacts`). If you intentionally change the public API, update the corresponding `.verified.txt` snapshot files in `src/Orc.Squirrel.Tests/`:

- `PublicApiFacts.Orc_Squirrel_HasNoBreakingChanges_Async.verified.txt`
- `PublicApiFacts.Orc_Squirrel_Xaml_HasNoBreakingChanges_Async.verified.txt`

To regenerate the snapshots, run the tests once and accept the new output by copying the `.received.txt` files over the corresponding `.verified.txt` files, or use the `AcceptAllChangesOnNoMatchingVerifier` approach supported by the Verify library.

**Philosophy:** Tests FAIL when wrong, never skip (except missing hardware).

### Debugging Methodology

1. **Establish baseline** — What's the known-good state?
2. **One change at a time** — Verify each change before proceeding
3. **Track changes in a table** — Log what you changed and the result
4. **Platform differences are signals** — If X works and Y fails, the difference IS the answer
5. **Revert if worse** — Don't pile fixes on top of failures

---

## Further Reading

| Topic | Document |
|-------|----------|
| Contributing guidelines | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Project documentation | [WildGums Open Source](http://opensource.wildgums.com) |
