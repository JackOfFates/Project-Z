# Project Z Application (Windows) .NET 8.0 Upgrade Tasks

## Overview

This document tracks the upgrade of Project Z Application (Windows) from .NET Framework 4.8 to .NET 8.0. The project will be converted to SDK-style format and upgraded in a single atomic operation.

**Progress**: 0/2 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites
**References**: Plan §Phase 0 Prerequisites

- [▶] (1) Verify .NET 8 SDK is installed per Plan §Phase 0 Prerequisites
- [ ] (2) .NET 8 SDK version 8.0.x or higher is available (**Verify**)
- [ ] (3) Check for global.json in solution root or project folder
- [ ] (4) If global.json exists, verify SDK version constraint allows .NET 8 or update constraint
- [ ] (5) global.json does not block .NET 8 SDK (**Verify**)

---

### [ ] TASK-002: Atomic SDK-style conversion and framework upgrade with compilation fixes
**References**: Plan §SDK-Style Project Conversion, Plan §Update Target Framework, Plan §Package Updates, Plan §Expected Breaking Changes

- [ ] (1) Convert Project Z Application (Windows).vbproj to SDK-style format per Plan §SDK-Style Project Conversion (use Visual Studio converter, upgrade-assistant, or manual conversion)
- [ ] (2) Project file is SDK-style format with `Sdk="Microsoft.NET.Sdk"` attribute (**Verify**)
- [ ] (3) Update TargetFramework property from net48 to net8.0-windows per Plan §Update Target Framework
- [ ] (4) Add `<UseWindowsForms>true</UseWindowsForms>` property to project file
- [ ] (5) TargetFramework is net8.0-windows and UseWindowsForms is true (**Verify**)
- [ ] (6) Verify all 8 packages are referenced as PackageReference elements (no version changes required per Plan §Package Updates)
- [ ] (7) All 8 packages present in project file at current compatible versions (**Verify**)
- [ ] (8) Restore dependencies: `dotnet restore "Project Z Application\Project Z Application (Windows).vbproj"`
- [ ] (9) All dependencies restored successfully (**Verify**)
- [ ] (10) Build solution and fix all compilation errors per Plan §Expected Breaking Changes (assessment shows zero expected, but address any discovered)
- [ ] (11) Solution builds with 0 errors (**Verify**)
- [ ] (12) Commit changes with message: "TASK-002: Upgrade Project Z Application (Windows) to .NET 8.0 - Convert to SDK-style format, update target framework from net48 to net8.0-windows, verify all packages compatible"

---
