# Software Architecture

- **Title:** Software Architecture
- **Scope:** This document defines the software architecture, layers, and design principles for the AxionFrame SolidWorks Add-in.

## Platform Scope

AxionFrame is a SolidWorks Add-in platform for parametric generation of a modular, height-adjustable X-frame table and manufacturing deliverables.

## Primary User Actions in Add-in UI

- **Build**: Generates CAD files (parts and assemblies).
- **Final Output**: Generates STEP, DXF, BOM, and report files in `Output/`.

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
   - Validation services.
   - Export orchestration services.
   - Infrastructure services for configuration, diagnostics, and run metadata.

3. **Domain Module Layer** (`API/Modules`)
   - Frame generation.
   - Pivot subsystem generation.
   - Height-adjustment mechanism generation.
   - Plate and brace generation.

4. **Shared Contracts Layer** (`API/Shared`)
   - Configuration contracts.
   - DTOs and identifiers.
   - Reusable utilities.

5. **Testing Layer** (`API/Tests`)
   - Unit, integration, and regression suites mapped to architecture layers.

The Stage 2.1 ownership note for this architecture is `Docs/Software/S2.1-ArchitectureOwnershipMap.md`.
The target configuration contract is `Docs/Software/ConfigurationSchemaSpecification.md`.
The target validator contract is `Docs/Software/ValidationAndErrorHandlingSpecification.md`.
The target naming grammar is `Docs/Software/DeterministicNamingStandard.md`.

## Canonical Domain Model

AxionFrame uses the following canonical split between product domains and core services:

- **Product domain modules**: `Frame`, `Pivot`, `Height Indexing`, and `Plate and Brace`
- **Core service areas**: `Exports`, `Validation`, and `Infrastructure`

This split is normative for implementation planning and documentation:

- Product geometry and subsystem behavior belong to the `Domain Module Layer`
- Cross-domain validation logic, final-output orchestration, and shared runtime infrastructure belong to the `Core Engine Layer`
- A feature ticket may reference a product domain together with one or more supporting core service areas, but the two categories should not be treated as interchangeable

## Design Principles

- Deterministic naming for features, mates, and reference geometry.
- Strong configuration contracts with schema validation.
- Reproducible outputs with run metadata.
- Traceability from engineering rule to implementation.
