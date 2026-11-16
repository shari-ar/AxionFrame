# AxionFrame

AxionFrame is a SolidWorks 2026 add-in that generates a parametric, fixed-height, fully demountable X-frame workbench and its manufacturing deliverables. The generated height remains fixed during operation, while the `Height` setting controls the generated geometry before Build execution.

## Product Workflow

```text
Settings
  -> Input Validation
  -> Build
  -> Build Verification Gate
  -> Final Output
  -> Release
```

- **Build** generates SolidWorks parts and assemblies.
- **Build Verification Gate** validates the generated result through automated evidence and formal SolidWorks 2026 verification.
- **Final Output** generates STEP, DXF, BOM, validation reports, manifests, and checksums.
- **Release** publishes versioned desktop assets through GitHub Releases.

## Active Engineering Baseline

The active product direction uses:

- Two identical end-frame structures.
- A selected X-frame architecture with two distinct internal nodes.
- Polygonal node plates captured inside slotted RHS member ends.
- Two-fastener connections with internal spacer elements.
- Local reinforcement at connection regions.
- A perforated work surface with continuous four-side clamping access.
- Complete disassembly into transportable components.
- A constant operating footprint.

The complete mechanical intent appears in `Docs/Mechanical/DesignAndManufacturing.md`.

## Documentation

- `Docs/DocumentationHub.md` provides the active documentation index.
- `Docs/Governance/DecisionRegister.md` records approved project decisions.
- `Docs/Workflow/CR-001-MechanicalRebaselineDecision.md` records the controlled mechanical rebaseline.
- `Docs/Mechanical/ProductRequirementsSpecification.md` defines the measurable product baseline.
- `Docs/Mechanical/RequirementsTraceabilityMatrix.md` maps decisions and requirements to verification evidence.
- `Docs/Mechanical/CriticalEngineeringRules.md` defines mandatory engineering-rule enforcement.
- `roadmap.md` defines the active stage-gate delivery plan.
- `Docs/Governance/DevOpsPolicy.md` defines GitHub and delivery policy.
- `Docs/Verification/SolidWorksVerificationPolicy.md` defines formal CAD verification.

GitHub Pages publishes the complete `Docs/` content through a custom GitHub Actions workflow.

## Platform Baseline

- Windows development environment
- SolidWorks 2026
- SolidWorks 2026 API and interop assemblies
- .NET Framework 4.8
- Visual Studio with C# and MSBuild support

## Repository Workflow

1. Create a short-lived feature branch.
2. Update active documentation and requirements.
3. Implement the approved scope.
4. Run fresh-build CI checks.
5. Produce the formal SolidWorks verification package.
6. Record verification evidence.
7. Merge through a pull request.
8. Publish versioned release assets after gate approval.
