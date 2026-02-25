# Software Architecture

- **Version:** 1

## Scope

AxionFrame is a SolidWorks Add-in platform for parametric generation of a modular, height-adjustable X-frame table and manufacturing deliverables.

## Primary User Actions in Add-in UI

- **Build**: generates CAD files (parts and assemblies).
- **Final Output**: generates STEP, DXF, BOM, and report files in `Output/`.

## Layered Architecture

1. **Add-in Host Layer** (`API/Addin`)
   - SolidWorks connection lifecycle.
   - Command and UI registration.
   - Event wiring and document lifecycle hooks.
   - Build and Final Output command entry points.

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
   - Unit, integration, and regression suites mapped to architecture layers.

## Design Principles

- Deterministic naming for features, mates, and reference geometry.
- Strong configuration contracts with schema validation.
- Reproducible outputs with run metadata.
- Traceability from engineering rule to implementation.
