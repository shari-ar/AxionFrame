# Software Architecture

- **Version:** 1.0
- **Last Updated:** 2026-02-24

## Scope

AxionFrame is a SolidWorks add-in platform for parametric generation of a modular, height-adjustable X-frame table and manufacturing deliverables.

## Layered Architecture

1. **Add-in Host Layer** (`API/Addin`)
   - SolidWorks connection lifecycle.
   - Command and UI registration.
   - Event wiring and document lifecycle hooks.

2. **Core Engine Layer** (`API/Core`)
   - Session orchestration.
   - Geometry and feature creation services.
   - Assembly and mate services.
   - Validation and export services.

3. **Domain Module Layer** (`API/Modules`)
   - Frame generation.
   - Pivot subsystem generation.
   - Height-adjustment mechanism generation.

4. **Shared Contracts Layer** (`API/Shared`)
   - Configuration contracts.
   - DTOs and identifiers.
   - Reusable utilities.

5. **Testing Layer** (`API/Tests`)
   - Unit, integration, and regression tests mapped to architecture layers.

## Design Principles

- Deterministic naming for features, mates, and reference geometry.
- Strong configuration contracts with schema validation.
- Reproducible outputs with run metadata.
- Traceability from engineering rule to implementation.
