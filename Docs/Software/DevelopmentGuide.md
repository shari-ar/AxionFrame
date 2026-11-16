# Software Development Guide

- **Title:** Software Development Guide
- **Status:** Active
- **Owner:** Software Engineering
- **Scope:** Repository conventions, SolidWorks 2026 authority, engineering implementation behavior, testing, readiness, completion, and delivery evidence.

## Authoritative References

- SolidWorks 2026 documentation provides the sole authority for API behavior, feature behavior, automation contracts, and supported capabilities.
- Active project documentation defines mechanical intent, configuration behavior, validation behavior, naming behavior, and output behavior.
- The Project Decision Register identifies the approved decision set.

## Repository Conventions

- C# source resides under `API/`.
- Software components align with the documented architecture layers.
- Domain modules expose clear contracts.
- Every code directory contains a `README.md` file.
- Each code-directory README defines purpose, responsibilities, inputs, outputs, dependencies, invariants, engineering context, and verification approach.
- Configuration behavior follows `ConfigurationSchemaSpecification.md` after rebaseline approval.
- Validation behavior follows `ValidationAndErrorHandlingSpecification.md` after rebaseline approval.
- Naming behavior follows `DeterministicNamingStandard.md`.

## Engineering Automation Standard

- Each automation sequence represents the workflow of a professional mechanical engineer operating SolidWorks 2026.
- Geometry creation uses fully defined sketches, stable references, explicit feature parameters, deterministic selection, and reproducible rebuild behavior.
- Assembly creation uses fully defined component placement, explicit mates, deterministic references, and traceable configuration state.
- Manufacturing features carry dimensions, tolerances, materials, process intent, and output eligibility.
- Modern SolidWorks 2026 capabilities receive preference when they improve determinism, maintainability, verification, and manufacturing readiness.
- Every generated result supports technical review through named features, named references, logs, measurements, and validation evidence.

## Command Model

- **Build** orchestrates validated CAD part and assembly generation.
- **Build Verification Gate** evaluates automated evidence and formal SolidWorks verification evidence.
- **Final Output** generates approved manufacturing artifacts and reports.

## Testing Strategy

- `Unit` covers parsers, validators, naming, identifiers, paths, formulas, and deterministic rules.
- `Integration` covers orchestration, configuration flow, module contracts, artifact assembly, and SolidWorks-independent boundaries.
- `Regression` covers stable behavior across approved revisions.
- Formal SolidWorks verification follows `../Verification/SolidWorksVerificationPolicy.md`.
- CI performs fresh dependency restoration and fresh builds.

## Definition of Ready

A feature reaches Ready status when:

- Approved requirement IDs define its behavior.
- Configuration keys and schema entries define its inputs.
- Engineering rules define geometry and manufacturing intent.
- Validator behavior defines acceptance criteria.
- Deterministic naming defines traceability hooks.
- Expected CAD and manufacturing outputs identify deliverables.
- Planned tests and verification cases identify evidence.
- A code-directory README update forms part of the planned change when directory behavior changes.

## Definition of Done

A feature reaches Done status when:

- Implementation matches active architecture and engineering contracts.
- SolidWorks 2026 references support every API-dependent behavior.
- Unit, integration, and regression suites pass.
- Applicable SolidWorks verification cases pass.
- Documentation and code-directory READMEs reflect the delivered behavior.
- Build artifacts, logs, manifests, and verification evidence remain traceable to the commit SHA and configuration hash.
- Pull-request review records the acceptance result.

## Related Documents

- `../Governance/DecisionRegister.md`
- `../Governance/DevOpsPolicy.md`
- `../Mechanical/DesignAndManufacturing.md`
- `../Verification/SolidWorksVerificationPolicy.md`
- `../../roadmap.md`
