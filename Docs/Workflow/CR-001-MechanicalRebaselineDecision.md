# CR-001 Mechanical Rebaseline Decision

- **Title:** CR-001 Mechanical Rebaseline Decision
- **Status:** Approved
- **Owner:** Engineering Governance
- **Effective Date:** 2026-11-15
- **Scope:** Controlled transition from the original height-adjustable frame baseline to the selected fixed-height parametric workbench baseline.

## Decision

AxionFrame continues in the existing repository and preserves its engineering and software history. The active product baseline uses the selected fixed-height X-frame workbench concept documented in `Docs/Mechanical/DesignAndManufacturing.md`.

The approved X-frame concept provides the sole concept input for engineering development. The engineering workflow records Design Intent, Requirements, Feasibility, Engineering Proof, and Mechanical Baseline approval.

## Reused Software Assets

- SolidWorks add-in registration and host integration
- Settings user interface foundation
- Configuration loading foundation
- Deterministic naming services
- Validation infrastructure
- Build lifecycle instrumentation
- Logging and run metadata
- Test project foundation
- Final Output architecture intent

## Rebaseline Impact

- Frame requirements receive the new end-frame and node geometry.
- The `Height` setting becomes a generation parameter for a fixed-height assembly.
- Joint engineering receives slotted RHS member ends, internal polygonal plates, spacer elements, and two-fastener connections.
- Work-surface engineering receives perforation and four-side clamping requirements.
- Portability engineering receives complete disassembly and transportable-component requirements.
- Configuration contracts receive the approved equation parameter set.
- Mechanical validators receive the new fixed-frame rules.
- Domain ownership receives frame, node-joint, work-surface, longitudinal-structure, and assembly responsibilities.
- The previous pivot-mechanism and height-indexing requirements retain historical traceability under the former baseline.
- Stage `S3.2` returns to active engineering after Mechanical Baseline approval and contract revalidation.

## Acceptance Basis

This decision becomes operational through:

1. The active mechanical design document.
2. The project decision register.
3. Roadmap Version 4.
4. Updated configuration and validation contracts.
5. Updated software domain ownership.
6. Updated Build verification evidence.

## Related Documents

- `Docs/Governance/DecisionRegister.md`
- `Docs/Mechanical/DesignAndManufacturing.md`
- `Docs/Workflow/ProjectLifecycle.md`
- `Docs/Verification/SolidWorksVerificationPolicy.md`
- `roadmap.md`
