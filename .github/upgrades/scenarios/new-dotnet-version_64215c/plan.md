# .NET 8.0 Upgrade Plan

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
This plan outlines the upgrade of **Project Z Application (Windows).vbproj** from .NET Framework 4.8 to .NET 8.0. This is a focused, single-project upgrade within a 5-project solution where the remaining 4 projects are already on .NET 8.

### Scope

**Projects Requiring Upgrade**: 1 of 5
- **Project Z Application (Windows).vbproj** (net48 → net8.0-windows)

**Projects Already on Target Framework**: 4 of 5
- Project Z Windows.vbproj (net8.0-windows ✅)
- Project Z Audio.vbproj (net8.0-windows7.0 ✅)
- Project Z Tower Defense.vbproj (net8.0-windows7.0 ✅)
- Project Z Video FX.vbproj (net8.0-windows ✅)

### Current State
- **Project Z Application (Windows).vbproj**:
  - Current Framework: .NET Framework 4.8
  - Project Type: Classic WinForms (non-SDK-style)
  - Dependencies: 0 project references
  - Dependants: 0 (standalone executable)
  - Package Count: 8 NuGet packages
  - Lines of Code: 1,079
  - Code Files: 19

### Target State
- **Project Z Application (Windows).vbproj**:
  - Target Framework: .NET 8.0 (net8.0-windows)
  - Project Type: SDK-style WinForms
  - All 8 packages remain compatible (no version changes required)

### Selected Strategy
**All-At-Once Strategy** - Single atomic upgrade operation.

**Rationale**:
- Only 1 project requires upgrade
- Project is standalone (no dependencies, no dependants)
- Small codebase (1,079 LOC)
- All packages are already compatible with .NET 8
- No security vulnerabilities
- No API compatibility issues detected
- Low complexity, low risk

### Complexity Assessment
**Overall Complexity: Low** ✅

**Discovered Metrics**:
- Total projects: 5
- Projects requiring upgrade: 1
- Dependency depth: 0
- High-risk projects: 0
- Security vulnerabilities: 0
- API compatibility issues: 0
- Package compatibility: 100% (11/11 packages compatible)

**Classification**: Simple solution - single standalone project upgrade with no breaking changes.

### Critical Issues
**None identified**. This is a clean upgrade scenario with:
- ✅ All packages compatible
- ✅ No API compatibility issues
- ✅ No security vulnerabilities
- ✅ No circular dependencies
- ✅ No complex breaking changes

### Recommended Approach
**Single-Phase Atomic Upgrade**:
1. Convert project to SDK-style
2. Update target framework to net8.0-windows
3. Build and validate
4. Test functionality

### Iteration Strategy
This plan uses a **fast batch approach** with 3 total detail iterations due to the simple nature of the upgrade (single project, no dependencies, no breaking changes).

---

## Migration Strategy

### Approach Selection

**Selected: All-At-Once Strategy**

This upgrade follows an **All-At-Once** approach because only a single project requires migration, making phased/incremental strategies unnecessary.

### Justification

**Why All-At-Once:**
1. **Single Project Scope**: Only 1 of 5 projects requires upgrade
2. **Zero Dependencies**: The project has no project references to coordinate with
3. **Standalone Nature**: The project has no dependants that could be affected
4. **Package Compatibility**: All 8 NuGet packages are already .NET 8 compatible
5. **No Breaking Changes**: Assessment detected zero API compatibility issues
6. **Small Codebase**: 1,079 LOC is manageable for atomic upgrade
7. **Low Risk**: No security vulnerabilities or high-risk indicators

**Why Not Incremental:**
- Incremental strategies (phased, dependency-ordered) are designed for multi-project upgrades with complex dependency graphs
- With a standalone project, there are no dependencies to phase
- No intermediate testing checkpoints are needed between project upgrades
- The overhead of incremental planning exceeds the benefit for a single-project upgrade

### All-At-Once Strategy Rationale

The All-At-Once strategy is optimal for this scenario:
- **Fastest completion**: Single coordinated operation
- **No multi-targeting complexity**: Direct framework change without compatibility layers
- **Clean dependency resolution**: No package version conflicts across projects
- **Simple coordination**: No need to synchronize multiple developers or teams
- **Clear success criteria**: Project either builds on .NET 8 or it doesn't

### Dependency-Based Ordering

**Not applicable** for this upgrade because:
- The project requiring upgrade has no project dependencies
- There are no projects that depend on this project
- Migration order considerations only apply to multi-project upgrades

### Parallel vs Sequential Execution

**Not applicable** for this upgrade because:
- Only one project is being upgraded
- Parallelization requires multiple concurrent upgrade targets
- All operations occur within a single project scope

### Phase Definitions

**Phase 0: Preparation** (Optional validation)
- Verify .NET 8 SDK installation
- Confirm no global.json SDK version constraints
- Ensure clean build on current framework (net48)

**Phase 1: Atomic Upgrade** (Single coordinated operation)
- Convert project file to SDK-style format
- Update TargetFramework to net8.0-windows
- Restore NuGet packages (no version changes needed)
- Build solution to identify any compilation errors
- Fix any compilation errors discovered (expected: none based on assessment)
- Rebuild to verify success

**Phase 2: Validation**
- Verify application launches
- Functional smoke testing
- Performance validation (optional)

### Risk Mitigation Built Into Strategy

- **SDK-style conversion tool**: Use Visual Studio's built-in converter or `upgrade-assistant` for reliable transformation
- **Package compatibility pre-verified**: Assessment confirms all packages are compatible
- **Rollback simplicity**: Git branch isolation enables instant rollback if issues arise
- **No dependent project impact**: Isolated nature means failed upgrade doesn't block other projects

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a clean, simple dependency structure:

```
Project Z Windows.vbproj (net8.0-windows) ✅ [Leaf Node]
    ↑
    ├── Project Z Audio.vbproj (net8.0-windows7.0) ✅
    │       ↑
    │       └── Project Z Video FX.vbproj (net8.0-windows) ✅
    │
    ├── Project Z Tower Defense.vbproj (net8.0-windows7.0) ✅
    │
    └── Project Z Video FX.vbproj (net8.0-windows) ✅

Project Z Application (Windows).vbproj (net48) ⚠️ [Standalone - No Dependencies]
```

**Legend**:
- ✅ Already on .NET 8
- ⚠️ Requires upgrade to .NET 8
- [Leaf Node] = No dependencies
- [Standalone] = No dependencies, no dependants

### Project Groupings by Migration Phase

**Phase 0: Already Upgraded** (No action required)
- Project Z Windows.vbproj
- Project Z Audio.vbproj
- Project Z Tower Defense.vbproj
- Project Z Video FX.vbproj

**Phase 1: Single Atomic Upgrade**
- **Project Z Application (Windows).vbproj** (net48 → net8.0-windows)

### Critical Path Identification

**No critical path exists** because:
- The project requiring upgrade has zero dependencies
- The project requiring upgrade has zero dependants
- The upgrade can proceed independently without affecting other projects
- No coordination with other projects is required

### Circular Dependencies

**None detected**. The solution has a clean acyclic dependency graph.

### Migration Order Rationale

Since **Project Z Application (Windows).vbproj** is completely isolated (standalone executable), it can be upgraded independently without consideration of:
- Dependency upgrade order
- Multi-targeting strategies
- Incremental compatibility layers
- Coordination with other projects

This isolation makes the upgrade exceptionally straightforward and low-risk.

---

## Project-by-Project Migration Plans

### Project Z Application (Windows).vbproj

**Current State**:
- Target Framework: net48 (.NET Framework 4.8)
- Project Type: Classic WinForms (non-SDK-style)
- SDK-style: False
- Project Dependencies: 0
- Dependants: 0
- Package Count: 8
- Lines of Code: 1,079
- Code Files: 19

**Target State**:
- Target Framework: net8.0-windows (.NET 8.0)
- Project Type: SDK-style WinForms
- SDK-style: True
- All packages remain at current versions (all compatible)

---

#### Migration Steps

##### 1. Prerequisites

**Verify .NET 8 SDK Installation**:
- Ensure .NET 8 SDK is installed on development machine
- Command: `dotnet --list-sdks` should show `8.0.xxx`
- If missing, download from https://dotnet.microsoft.com/download/dotnet/8.0

**Check for global.json Constraints**:
- Look for `global.json` in solution root or project folder
- If present, verify SDK version allows .NET 8 (version 8.0.x or higher)
- Remove or update version constraint if it blocks .NET 8

**Baseline Build**:
- Ensure project builds successfully on current framework (net48)
- Fix any existing build warnings/errors before upgrade
- Commit current state if not already done

##### 2. SDK-Style Project Conversion

**Option A: Visual Studio Built-in Converter (Recommended)**
1. Right-click on **Project Z Application (Windows).vbproj** in Solution Explorer
2. Select "Upgrade" or "Migrate to SDK-style project" (feature availability depends on VS version)
3. Review changes proposed by converter
4. Accept conversion

**Option B: .NET Upgrade Assistant CLI**
1. Install: `dotnet tool install -g upgrade-assistant`
2. Run: `upgrade-assistant upgrade "Project Z Application\Project Z Application (Windows).vbproj"`
3. Follow interactive prompts
4. Select SDK-style conversion option

**Option C: Manual Conversion**
If automated tools fail, manually convert the project file:

**Before (Classic .vbproj structure)**:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="..." DefaultTargets="Build" xmlns="...">
  <PropertyGroup>
    <Configuration Condition="...">Debug</Configuration>
    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
    <OutputType>WinExe</OutputType>
    <RootNamespace>...</RootNamespace>
    <AssemblyName>...</AssemblyName>
    <!-- Many other properties -->
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Windows.Forms" />
    <!-- Explicit file references -->
  </ItemGroup>
  <ItemGroup>
    <Compile Include="Form1.vb" />
    <Compile Include="My Project\AssemblyInfo.vb" />
    <!-- All files explicitly listed -->
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.VisualBasic.targets" />
</Project>
```

**After (SDK-style .vbproj structure)**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <OutputType>WinExe</OutputType>
    <UseWindowsForms>true</UseWindowsForms>
    <RootNamespace>Project_Z_Application__Windows_</RootNamespace>
    <AssemblyName>Project Z Application (Windows)</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="nkast.Kni.Platform.SDL2.GL" Version="4.2.9001.1" />
    <PackageReference Include="nkast.Xna.Framework" Version="4.2.9001" />
    <PackageReference Include="nkast.Xna.Framework.Audio" Version="4.2.9001" />
    <PackageReference Include="nkast.Xna.Framework.Content" Version="4.2.9001" />
    <PackageReference Include="nkast.Xna.Framework.Game" Version="4.2.9001" />
    <PackageReference Include="nkast.Xna.Framework.Graphics" Version="4.2.9001" />
    <PackageReference Include="nkast.Xna.Framework.Input" Version="4.2.9001" />
    <PackageReference Include="nkast.Xna.Framework.Media" Version="4.2.9001" />
    <PackageReference Include="Speckle.Triangle" Version="1.0.0" />
  </ItemGroup>
</Project>
```

**Key Changes in Manual Conversion**:
- Add `Sdk="Microsoft.NET.Sdk"` attribute to `<Project>` tag
- Change `TargetFrameworkVersion` to `TargetFramework` with value `net8.0-windows`
- Add `<UseWindowsForms>true</UseWindowsForms>` for WinForms support
- Remove all `<Compile>` items (files are auto-included in SDK-style)
- Remove explicit assembly references (automatically handled)
- Convert `packages.config` references to `<PackageReference>` elements (if not already done)
- Remove `<Import>` statements (handled by SDK)

##### 3. Update Target Framework

If not already changed during SDK-style conversion:
- Locate `<TargetFramework>` element in project file
- Change value from `net48` to `net8.0-windows`
- Note: Use `net8.0-windows` (not just `net8.0`) to ensure Windows-specific APIs are available

```xml
<TargetFramework>net8.0-windows</TargetFramework>
```

##### 4. Package Updates

**Good news**: Assessment confirms all packages are compatible with .NET 8 at their current versions.

**Current Packages** (no changes required):
| Package | Version | Status |
|---------|---------|--------|
| nkast.Kni.Platform.SDL2.GL | 4.2.9001.1 | ✅ Compatible |
| nkast.Xna.Framework | 4.2.9001 | ✅ Compatible |
| nkast.Xna.Framework.Audio | 4.2.9001 | ✅ Compatible |
| nkast.Xna.Framework.Content | 4.2.9001 | ✅ Compatible |
| nkast.Xna.Framework.Game | 4.2.9001 | ✅ Compatible |
| nkast.Xna.Framework.Graphics | 4.2.9001 | ✅ Compatible |
| nkast.Xna.Framework.Input | 4.2.9001 | ✅ Compatible |
| nkast.Xna.Framework.Media | 4.2.9001 | ✅ Compatible |
| Speckle.Triangle | 1.0.0 | ✅ Compatible |

**Action**: Restore packages (no version changes needed)
```bash
dotnet restore "Project Z Application\Project Z Application (Windows).vbproj"
```

##### 5. Expected Breaking Changes

**Assessment Result**: Zero API compatibility issues detected.

**Potential Areas to Monitor** (even though assessment found no issues):

**WinForms on .NET 8**:
- Most WinForms APIs are compatible, but verify:
  - Custom control rendering (GDI+ behavior may differ slightly)
  - Application.DoEvents() usage (still supported but discouraged)
  - Thread marshalling for UI updates (same patterns apply)

**XNA Framework Compatibility** (nkast.Xna packages):
- These are .NET Standard 2.0 compatible packages
- Should work identically on .NET 8
- Verify game loop timing if performance-sensitive

**Visual Basic Language Features**:
- VB.NET is fully supported in .NET 8
- All language features from .NET Framework are retained
- `My` namespace still available with Windows-specific references

**Known .NET 8 Breaking Changes** (unlikely to affect this project):
- None identified in assessment for this specific codebase
- Review https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0 if unexpected issues arise

##### 6. Code Modifications

**Expected**: None

Based on assessment findings:
- Zero API compatibility issues
- Zero source compatibility issues
- Zero behavioral breaking changes

**If Compilation Errors Occur** (unexpected based on assessment):

1. **Namespace Changes**:
   - Add missing `Imports` statements if namespace organization changed
   - Check for moved types (rare in WinForms/VB.NET)

2. **API Signature Changes**:
   - Update method calls if signatures changed (assessment shows none expected)
   - Add explicit type conversions if implicit conversions removed

3. **Configuration Changes**:
   - app.config may need adjustments for runtime configuration
   - Verify binding redirects are no longer needed (SDK-style handles automatically)

##### 7. Build and Verify

**Build the Project**:
```bash
dotnet build "Project Z Application\Project Z Application (Windows).vbproj" --configuration Release
```

**Expected Result**: Build succeeds with 0 errors, 0 warnings

**If Build Fails**:
1. Review error messages for patterns
2. Check for issues related to:
   - Missing Windows SDK references (should be automatic with `net8.0-windows`)
   - Package restore failures (verify package sources)
   - VB.NET compiler issues (verify VB language version in project properties)
3. Address errors one category at a time
4. Rebuild after each fix

**Verify Output**:
- Check `bin\Release\net8.0-windows\` folder for compiled application
- Verify all dependencies are copied to output folder
- Confirm executable is generated (`.exe` file)

##### 8. Testing Strategy

**Unit Tests**:
- Not applicable (no test projects detected for this application)

**Manual Functional Testing**:
1. **Launch Application**: Run the `.exe` from output folder
2. **Verify UI Rendering**: Ensure all forms display correctly
3. **Test Core Features**:
   - Any game/graphics functionality (XNA Framework usage)
   - Audio playback if applicable
   - User input handling
   - File I/O operations
4. **Error Handling**: Verify exception handling still works
5. **Performance**: Check for any noticeable performance changes

**Smoke Test Checklist**:
- [ ] Application launches without errors
- [ ] Main window displays correctly
- [ ] UI controls respond to input
- [ ] No missing dependencies error dialogs
- [ ] Application closes cleanly

##### 9. Validation Checklist

**Technical Validation**:
- [ ] Project file is SDK-style format
- [ ] TargetFramework is `net8.0-windows`
- [ ] All packages restored successfully
- [ ] Build succeeds with 0 errors
- [ ] Build produces 0 warnings (or warnings are documented/acceptable)
- [ ] Output executable is generated in `bin\` folder
- [ ] Output targets correct runtime (win-x64, win-x86, or any)

**Functional Validation**:
- [ ] Application launches successfully
- [ ] UI renders correctly
- [ ] Core functionality works as expected
- [ ] No regressions observed compared to net48 version
- [ ] Performance is acceptable

**Quality Validation**:
- [ ] Code compiles without warnings
- [ ] No deprecated API usage (or documented if unavoidable)
- [ ] Project file is clean and maintainable

---

### Projects Already on .NET 8 (No Action Required)

The following projects are already on .NET 8 and require no migration:

#### Project Z Windows.vbproj
- Current Framework: net8.0-windows ✅
- Status: No upgrade needed

#### Project Z Audio.vbproj
- Current Framework: net8.0-windows7.0 ✅
- Status: No upgrade needed

#### Project Z Tower Defense.vbproj
- Current Framework: net8.0-windows7.0 ✅
- Status: No upgrade needed

#### Project Z Video FX.vbproj
- Current Framework: net8.0-windows ✅
- Status: No upgrade needed

---

## Risk Management

### High-Risk Changes

| Project | Risk Level | Description | Mitigation |
|---------|-----------|-------------|------------|
| Project Z Application (Windows).vbproj | 🟢 Low | SDK-style conversion + framework change | Use automated conversion tools; assessment shows no API issues; all packages compatible |

**Overall Risk Assessment: Low** ✅

This upgrade presents minimal risk due to:
- Single project scope (limited blast radius)
- Standalone nature (no dependent projects affected)
- Clean package compatibility (100% compatible)
- No API breaking changes detected
- Small codebase (manageable troubleshooting surface)
- Classic WinForms to SDK-style WinForms has well-established conversion path

### Security Vulnerabilities

**None identified**. Assessment shows:
- ✅ Zero packages with known CVEs
- ✅ Zero deprecated packages requiring replacement
- ✅ All packages have active .NET 8 support

### Contingency Plans

#### SDK-Style Conversion Fails

**Scenario**: Automated conversion produces invalid project file or introduces errors

**Mitigation**:
1. Use Visual Studio's built-in "Upgrade Project" feature first (most reliable)
2. If that fails, try `upgrade-assistant` CLI tool
3. If both fail, manually create SDK-style project file using reference template
4. Worst case: Revert to net48, research specific conversion blocker

**Alternative**: Keep project as classic .NET Framework if conversion proves overly complex (delays .NET 8 benefits but maintains stability)

#### Compilation Errors After Upgrade

**Scenario**: Project fails to build after framework change despite assessment showing no issues

**Mitigation**:
1. Review build errors for patterns (missing APIs, namespace changes)
2. Check for conditional compilation that may differ between .NET Framework and .NET
3. Verify Windows-specific APIs have appropriate `net8.0-windows` TFM
4. Consult .NET 8 breaking changes documentation for unexpected issues
5. Rollback and investigate specific APIs causing issues

**Alternative**: Multi-target both `net48` and `net8.0-windows` temporarily to isolate issues

#### Runtime Errors (Passes Build, Fails at Runtime)

**Scenario**: Application builds successfully but crashes or behaves incorrectly

**Mitigation**:
1. Enable all .NET runtime exception details for diagnostics
2. Check for P/Invoke signatures that may differ between frameworks
3. Review dependency injection container behavior (if used)
4. Test all major application features systematically
5. Compare behavior against net48 version side-by-side

**Alternative**: Deploy to staging/test environment before production

#### Performance Degradation

**Scenario**: Application runs slower on .NET 8 than .NET Framework 4.8

**Mitigation**:
1. Profile application to identify bottlenecks
2. Review JIT compilation settings
3. Check for compatibility mode features (e.g., legacy loading contexts)
4. Verify UI rendering performance (WinForms on .NET 8 may have different characteristics)

**Alternative**: Performance issues are rare when upgrading to .NET 8, but if critical, delay upgrade and investigate specific workloads

### Rollback Strategy

**Git-Based Rollback** (Recommended):
1. All changes occur in isolated `upgrade-to-NET8` branch
2. Revert by switching back to `master` branch: `git checkout master`
3. No impact on other projects (already on .NET 8)
4. Can retry upgrade after fixing issues

**Partial Rollback**:
- Not applicable (single project, atomic upgrade)

**Time to Rollback**: < 5 minutes (simple branch switch)

---

## Testing & Validation Strategy

### Overview

Since this is a single-project upgrade with no dependencies or dependants, testing follows a straightforward validation approach focused on the upgraded project itself.

### Phase Testing Requirements

#### Phase 0: Pre-Upgrade Validation

**Purpose**: Establish baseline before upgrade

**Tests**:
1. **Build Verification**: Confirm project builds successfully on net48
2. **Functionality Baseline**: Document current application behavior
3. **Performance Baseline** (optional): Record startup time, memory usage

**Success Criteria**:
- [ ] Project builds with 0 errors on net48
- [ ] Application launches and runs without errors
- [ ] Core features work as expected

#### Phase 1: Post-Upgrade Validation

**Purpose**: Verify upgrade succeeded without regressions

**Tests**:
1. **Build Verification**: Confirm project builds on net8.0-windows
2. **Dependency Verification**: Ensure all packages restored correctly
3. **Runtime Verification**: Confirm application launches and functions
4. **Feature Comparison**: Compare behavior against net48 baseline

**Success Criteria**:
- [ ] Project builds with 0 errors on net8.0-windows
- [ ] All packages restore without conflicts
- [ ] Application launches without errors
- [ ] No functional regressions observed
- [ ] Performance is acceptable (comparable to net48)

### Smoke Tests

**Quick Validation After Upgrade** (5-10 minutes):

1. **Application Launch**:
   - Double-click executable in `bin\Release\net8.0-windows\`
   - Verify no "missing dependency" errors
   - Verify no framework version errors

2. **UI Rendering**:
   - Verify main window displays correctly
   - Check all UI controls render properly
   - Verify layout/sizing is correct

3. **Basic Interaction**:
   - Click buttons/controls to verify responsiveness
   - Test keyboard input if applicable
   - Verify mouse events work correctly

4. **Core Functionality** (project-specific):
   - If game/graphics application: Verify rendering works
   - If audio application: Verify audio playback
   - Test primary use cases of the application

5. **Clean Shutdown**:
   - Close application normally
   - Verify no errors on exit
   - Verify no orphaned processes

**Expected Outcome**: All smoke tests pass without issues

### Comprehensive Validation

**Detailed Testing After Smoke Tests Pass** (optional, recommended before production deployment):

#### Functional Testing

| Test Area | Test Cases | Expected Result |
|-----------|-----------|-----------------|
| **Application Startup** | Launch from desktop shortcut, Launch from command line, Launch with arguments (if applicable) | Application starts successfully in all scenarios |
| **UI Components** | All forms display correctly, All controls functional, Tab order preserved, Tooltips display | No UI regressions |
| **XNA Framework Features** | Graphics rendering, Game loop timing, Input handling, Audio playback | Graphics and audio work correctly |
| **File I/O** | Read configuration files, Write output files, Handle missing files | File operations work as expected |
| **Error Handling** | Trigger known error conditions, Verify error messages display, Verify graceful degradation | Error handling unchanged |

#### Non-Functional Testing

| Test Area | Test Cases | Expected Result |
|-----------|-----------|-----------------|
| **Performance** | Application startup time, Memory usage, Frame rate (if applicable) | Performance comparable to net48 or better |
| **Compatibility** | Test on Windows 10, Test on Windows 11, Test with different .NET 8 SDK versions | Works on supported Windows versions |
| **Stability** | Extended runtime testing (leave running for hours), Stress testing (if applicable) | No crashes or memory leaks |

### Testing Checkpoints

**Checkpoint 1: After SDK-Style Conversion**
- Verify project file is valid XML
- Verify project loads in Visual Studio without errors
- Do NOT build yet (wait until target framework updated)

**Checkpoint 2: After Target Framework Update**
- Restore packages: `dotnet restore`
- Verify all packages restore successfully
- Check for any package compatibility warnings

**Checkpoint 3: After First Build**
- Verify build succeeds (0 errors)
- Review any warnings (document or fix)
- Verify output folder contains executable

**Checkpoint 4: After First Run**
- Application launches without errors
- Smoke tests pass
- No obvious regressions

**Checkpoint 5: Before Commit**
- Comprehensive validation complete
- All tests pass
- Performance acceptable
- Ready to merge upgrade branch

### Test Automation

**Current State**: No automated test projects detected in solution

**Recommendation**: Consider adding unit tests for future upgrades, especially for:
- Core business logic
- Graphics rendering validation
- Audio processing validation

**For This Upgrade**: Manual testing is sufficient given:
- Single project scope
- Small codebase (1,079 LOC)
- Low complexity
- No detected breaking changes

### Regression Testing

**Regression Risk**: Low (no API changes detected)

**Key Areas to Monitor**:
1. **XNA Framework Compatibility**: Graphics and audio rendering
2. **WinForms Behavior**: UI rendering and event handling
3. **VB.NET Language Features**: `My` namespace, implicit conversions

**Regression Test Plan**:
- Compare application behavior side-by-side (net48 vs net8.0-windows)
- Document any differences observed
- Determine if differences are acceptable or require fixes

### Performance Validation

**Baseline Metrics** (capture before upgrade):
- Application startup time
- Memory usage at idle
- Frame rate during typical usage (if game/graphics app)

**Post-Upgrade Metrics** (capture after upgrade):
- Same measurements as baseline
- Compare to identify improvements or regressions

**Expected Outcome**:
- .NET 8 typically shows improved performance over .NET Framework
- Startup time may be faster
- Memory usage may be lower
- JIT compilation generally more efficient

**Acceptable Thresholds**:
- Startup time: ±20% of baseline
- Memory usage: ±15% of baseline
- Frame rate: No degradation (or improvement)

If metrics fall outside acceptable thresholds, investigate and optimize before production deployment.

---

## Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | Dependencies | Risk | Rationale |
|---------|-----------|--------------|------|-----------|
| **Project Z Application (Windows).vbproj** | 🟢 Low | 0 | 🟢 Low | Standalone project, all packages compatible, no API issues, SDK-style conversion well-documented |
| Project Z Windows.vbproj | ✅ N/A | 0 | N/A | Already on .NET 8 |
| Project Z Audio.vbproj | ✅ N/A | 1 | N/A | Already on .NET 8 |
| Project Z Tower Defense.vbproj | ✅ N/A | 1 | N/A | Already on .NET 8 |
| Project Z Video FX.vbproj | ✅ N/A | 2 | N/A | Already on .NET 8 |

### Phase Complexity Assessment

**Phase 1: Atomic Upgrade**
- **Complexity**: Low
- **Primary Task**: SDK-style conversion + target framework update
- **Dependency Coordination**: None required (standalone project)
- **Expected Challenges**: Minimal - WinForms SDK-style conversion is well-established

### Resource Requirements

**Skill Levels Required**:
- **.NET Migration Experience**: Moderate (familiarity with SDK-style projects helpful)
- **WinForms Knowledge**: Basic (understand project structure)
- **Git/Source Control**: Basic (branch management, commit/rollback)
- **Troubleshooting**: Basic (interpret build errors if any)

**Parallel Capacity**:
- Not applicable (single project upgrade)
- Single developer can complete entire upgrade

**Estimated Relative Effort**:
- **Low complexity**: Straightforward SDK-style conversion and framework update
- **No package updates required**: Eliminates package compatibility troubleshooting
- **No API migration**: No code changes expected based on assessment
- **Single project scope**: No coordination overhead

**Effort Distribution**:
- SDK-style conversion: Primary task
- Build verification: Quick validation
- Testing: Standard smoke testing
- Documentation: Minimal (single project change)

---

## Source Control Strategy

### Branching Strategy

**Upgrade Branch**: `upgrade-to-NET8`
**Source Branch**: `master`
**Merge Target**: `master`

**Branch Workflow**:
1. ✅ Pre-upgrade commit on `master` (completed during assessment setup)
2. ✅ Create `upgrade-to-NET8` branch from `master`
3. Perform all upgrade work in `upgrade-to-NET8` branch
4. Validate and test in isolated branch
5. Merge `upgrade-to-NET8` → `master` after validation complete

**Rationale**:
- Isolated branch protects `master` from instability during upgrade
- Easy rollback by simply switching back to `master`
- Other projects already on .NET 8 remain unaffected
- Clean merge history documents upgrade as atomic change

### Commit Strategy

**Recommended: Single Commit Approach**

Since this is a single-project atomic upgrade with All-At-Once strategy, prefer a single comprehensive commit:

**Single Commit Structure**:
```
Commit Message: "Upgrade Project Z Application (Windows) to .NET 8.0"

Changes:
- Convert project to SDK-style format
- Update TargetFramework from net48 to net8.0-windows
- Retain all packages at compatible versions (no changes)

Files Changed:
- Project Z Application/Project Z Application (Windows).vbproj

Assessment: Zero breaking changes detected
Validation: Build succeeds, smoke tests pass
```

**Benefits of Single Commit**:
- Clean history (upgrade is single logical operation)
- Easy to revert entire upgrade if needed
- Simplifies code review
- Aligns with All-At-Once strategy philosophy

**Alternative: Multi-Commit Approach** (if preferred)

If you prefer incremental commits for safety:

**Commit 1**: SDK-style conversion
```
git commit -m "Convert Project Z Application (Windows) to SDK-style format"
```

**Commit 2**: Target framework update
```
git commit -m "Update Project Z Application (Windows) target framework to net8.0-windows"
```

**Commit 3**: Validation and fixes (if any)
```
git commit -m "Fix build issues and validate .NET 8 upgrade"
```

**Trade-offs**:
- ✅ More granular rollback capability
- ✅ Easier to identify which step caused issues
- ❌ More commits for simple operation
- ❌ Intermediate commits may not build (non-compilable states)

### Commit Message Format

**Recommended Format**:
```
[.NET 8 Upgrade] <Subject>

<Body describing changes>

<Validation results>
```

**Example**:
```
[.NET 8 Upgrade] Migrate Project Z Application (Windows) to .NET 8.0

Changes:
- Converted project from classic to SDK-style format
- Updated TargetFramework from net48 to net8.0-windows
- Verified all 8 NuGet packages compatible (no version changes)
- Validated build succeeds with 0 errors, 0 warnings

Assessment Results:
- Zero API breaking changes detected
- Zero package compatibility issues
- Zero security vulnerabilities

Testing:
- Build: PASS (Release configuration)
- Smoke Tests: PASS (application launches and functions correctly)
- Performance: Acceptable (comparable to net48 baseline)

Branch: upgrade-to-NET8
Ready for merge to master
```

### Review and Merge Process

#### Pre-Merge Checklist

Before merging `upgrade-to-NET8` → `master`:

**Technical Validation**:
- [ ] Project builds successfully on net8.0-windows
- [ ] All packages restore without errors
- [ ] Zero build errors
- [ ] Zero build warnings (or documented)
- [ ] Output executable generated correctly

**Functional Validation**:
- [ ] Application launches without errors
- [ ] Smoke tests pass
- [ ] No functional regressions observed
- [ ] Performance acceptable

**Code Quality**:
- [ ] Project file is clean and maintainable
- [ ] No temporary/debug code left in
- [ ] Commit messages are clear and descriptive

**Documentation**:
- [ ] Assessment.md reviewed
- [ ] Plan.md followed
- [ ] Any deviations from plan documented

#### Merge Criteria

**Merge when**:
- All checklist items above are complete
- Stakeholders approve upgrade (if required)
- Timing aligns with release schedule (if applicable)

**Merge Command**:
```bash
git checkout master
git merge upgrade-to-NET8 --no-ff -m "Merge .NET 8 upgrade for Project Z Application (Windows)"
git push origin master
```

**Use `--no-ff`** (no fast-forward) to preserve branch history showing this was a deliberate upgrade operation.

#### Post-Merge Actions

After merging to `master`:
1. Tag the release (optional but recommended):
   ```bash
   git tag -a v1.0.0-net8 -m "Version 1.0.0 upgraded to .NET 8.0"
   git push origin v1.0.0-net8
   ```

2. Build release artifacts from `master`
3. Deploy to test/staging environment
4. Perform final validation in production-like environment
5. Deploy to production (if applicable)

6. Optional: Delete upgrade branch (keep history)
   ```bash
   git branch -d upgrade-to-NET8
   git push origin --delete upgrade-to-NET8
   ```

### Conflict Resolution

**Expected Conflicts**: None (isolated single-project change)

**If Conflicts Occur** (e.g., if `master` changed during upgrade):
1. Update `upgrade-to-NET8` branch from `master`:
   ```bash
   git checkout upgrade-to-NET8
   git merge master
   ```
2. Resolve any conflicts in project file
3. Rebuild and retest after conflict resolution
4. Commit conflict resolution
5. Proceed with merge to `master`

### Rollback Procedures

#### Rollback Before Merge

**Simple**: Just switch back to `master` branch
```bash
git checkout master
```
- Upgrade branch remains for later retry
- No impact on `master` branch

#### Rollback After Merge

**Option 1: Revert Merge Commit** (recommended)
```bash
git revert -m 1 <merge-commit-hash>
git push origin master
```
- Creates new commit that undoes upgrade
- Preserves history
- Safe for shared branches

**Option 2: Hard Reset** (use with caution)
```bash
git reset --hard <commit-before-merge>
git push origin master --force
```
- ⚠️ **Dangerous**: Rewrites history
- Only use if merge not yet pulled by others
- Loses upgrade work (but preserved in branch)

### Backup Strategy

**Before Starting Upgrade**:
1. ✅ Commit current state (completed)
2. ✅ Create upgrade branch (completed)
3. Optional: Create backup archive of entire solution
   ```bash
   tar -czf project-z-backup-pre-net8-upgrade.tar.gz "C:\Users\Patho\source\repos\JackOfFates\Project-Z"
   ```

**During Upgrade**:
- Commit frequently if using multi-commit approach
- Push to remote if working on shared repository

**After Upgrade**:
- Merge to `master` only after full validation
- Tag release for easy reference
- Keep upgrade branch for historical reference

---

## Success Criteria

### Technical Criteria

The upgrade is technically successful when:

#### Project Configuration
- [ ] **Project Z Application (Windows).vbproj** is SDK-style format
- [ ] Project targets `net8.0-windows` (verified in project file)
- [ ] `<UseWindowsForms>true</UseWindowsForms>` is present in project file
- [ ] All 8 NuGet packages are referenced as `<PackageReference>` elements
- [ ] No `packages.config` file remains (if SDK-style conversion removed it)

#### Build Success
- [ ] Project builds without errors: `dotnet build` returns exit code 0
- [ ] Project builds without warnings (or warnings documented as acceptable)
- [ ] Build output folder contains executable: `bin\Release\net8.0-windows\Project Z Application (Windows).exe`
- [ ] All dependencies are copied to output folder correctly
- [ ] Build succeeds in both Debug and Release configurations

#### Package Compatibility
- [ ] All 8 packages restore successfully
- [ ] No package dependency conflicts reported
- [ ] Package versions match assessment recommendations:
  - nkast.Kni.Platform.SDL2.GL v4.2.9001.1
  - nkast.Xna.Framework v4.2.9001
  - nkast.Xna.Framework.Audio v4.2.9001
  - nkast.Xna.Framework.Content v4.2.9001
  - nkast.Xna.Framework.Game v4.2.9001
  - nkast.Xna.Framework.Graphics v4.2.9001
  - nkast.Xna.Framework.Input v4.2.9001
  - nkast.Xna.Framework.Media v4.2.9001
  - Speckle.Triangle v1.0.0

#### Runtime Success
- [ ] Application launches without errors
- [ ] No missing framework/runtime errors
- [ ] No missing dependency errors
- [ ] Application closes cleanly without errors

### Quality Criteria

The upgrade meets quality standards when:

#### Code Quality
- [ ] Project file is clean and maintainable (no commented-out sections, no obsolete properties)
- [ ] No compiler warnings related to deprecated APIs
- [ ] No `#if` conditional compilation artifacts from upgrade process
- [ ] VB.NET code unchanged (zero code modifications required per assessment)

#### Functional Quality
- [ ] Application functionality unchanged from net48 version
- [ ] UI renders correctly (no visual regressions)
- [ ] XNA Framework features work correctly (graphics, audio, input)
- [ ] WinForms controls behave as expected
- [ ] Error handling works correctly
- [ ] File I/O operations function properly

#### Performance Quality
- [ ] Application startup time is acceptable (comparable to net48 or better)
- [ ] Memory usage is acceptable (comparable to net48 or better)
- [ ] Frame rate/responsiveness unchanged or improved
- [ ] No performance regressions identified

#### Test Coverage
- [ ] Smoke tests pass (application launch, basic interaction, clean shutdown)
- [ ] Comprehensive functional testing complete (if performed)
- [ ] No regressions identified during testing
- [ ] Performance validation complete (if performed)

### Process Criteria

The upgrade follows proper process when:

#### All-At-Once Strategy Principles
- [ ] **Single atomic operation**: Project upgraded in one coordinated effort (not incremental)
- [ ] **No intermediate states**: Moved directly from net48 to net8.0-windows without multi-targeting
- [ ] **No multi-targeting complexity**: Project targets only net8.0-windows (not multiple frameworks)
- [ ] **Single validation checkpoint**: Build and test once after complete upgrade (not after each step)
- [ ] **Unified package update**: All packages considered simultaneously (even though no updates needed)

#### Source Control
- [ ] All changes committed to `upgrade-to-NET8` branch
- [ ] Commit messages follow agreed format (descriptive, include validation results)
- [ ] Single commit used for atomic upgrade (or multi-commit approach documented)
- [ ] No temporary/debug commits remain (squashed if necessary)
- [ ] Branch ready for merge to `master`

#### Documentation
- [ ] Assessment.md reviewed and findings validated
- [ ] Plan.md followed (or deviations documented with rationale)
- [ ] Any unexpected issues documented
- [ ] Commit messages provide clear upgrade history
- [ ] README or migration notes updated (if applicable)

#### Validation
- [ ] All pre-merge checklist items complete
- [ ] Technical validation criteria met
- [ ] Functional validation criteria met
- [ ] Quality validation criteria met
- [ ] Stakeholder approval obtained (if required)

### Upgrade Complete Definition

**The .NET 8 upgrade is considered COMPLETE when**:

1. ✅ **All Technical Criteria Met**:
   - Project is SDK-style targeting net8.0-windows
   - Builds succeed with 0 errors
   - All packages compatible
   - Application runs without errors

2. ✅ **All Quality Criteria Met**:
   - Code quality maintained
   - No functional regressions
   - Performance acceptable
   - Testing complete

3. ✅ **All Process Criteria Met**:
   - All-At-Once strategy principles followed
   - Source control properly managed
   - Documentation complete
   - Validation thorough

4. ✅ **Production Ready**:
   - Changes merged to `master` (or ready to merge)
   - Release artifacts built and tested
   - Deployment to production approved (if applicable)
   - Rollback plan confirmed

### Success Metrics

**Quantitative Measures**:
- Build errors: **0**
- Build warnings: **0** (or acceptable warnings documented)
- Package compatibility: **100%** (11/11 packages compatible)
- API compatibility: **100%** (0 breaking changes detected)
- Functional test pass rate: **100%**
- Performance within acceptable thresholds: **Yes**

**Qualitative Measures**:
- Developer confidence in upgrade: **High** (low complexity, well-documented)
- User experience unchanged: **Yes** (no functional regressions)
- Maintainability improved: **Yes** (SDK-style is more maintainable)
- Future upgrade path clear: **Yes** (.NET 8 → .NET 9/10 will be easier)

### Sign-Off Checklist

Before declaring upgrade complete and closing this initiative:

- [ ] All success criteria sections reviewed and verified
- [ ] All checklist items marked complete
- [ ] Stakeholders informed of completion
- [ ] Upgrade documentation archived for future reference
- [ ] Lessons learned documented (if any)
- [ ] Next steps identified (e.g., remaining projects, future upgrades)

---

**Upgrade Status**: 🔄 In Progress → ✅ Complete (after all criteria met)
