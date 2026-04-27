# Software Development Guide

- **Title:** Software Development Guide
- **Scope:** This document defines repository conventions, command behavior, and readiness and completion criteria for software delivery.

## Repository Conventions

- C# source resides under `API/`.
- Software components align with architecture layers.
- Domain modules publish clear interfaces.
- Configuration behavior follows `Docs/Software/ConfigurationSchemaSpecification.md`.
- Validation behavior follows `Docs/Software/ValidationAndErrorHandlingSpecification.md`.
- Naming behavior follows `Docs/Software/DeterministicNamingStandard.md`.

## Add-in Command Model

- **Build Command** orchestrates CAD generation flow.
- **Final Output Command** orchestrates export flow and report generation.

## Core Technical Contracts

- Configuration keys, defaults, types, and cross-field rules are defined in `Docs/Software/ConfigurationSchemaSpecification.md`.
- Validation severity, validator IDs, and deterministic error output are defined in `Docs/Software/ValidationAndErrorHandlingSpecification.md`.
- Deterministic feature, mate, configuration, export, and traceability names are defined in `Docs/Software/DeterministicNamingStandard.md`.

## Testing Strategy

`API/Tests` uses three suites:

- `Unit/` for parser, validator, naming, and path logic.
- `Integration/` for Add-in flow and CAD generation workflow.
- `Regression/` for stability checks across revisions.

## Definition of Ready

A feature is ready for implementation when:

- Functional requirements exist in `Docs/Workflow` or `Docs/Mechanical`.
- Configuration keys and schema entries are defined.
- Validator behavior for affected rules is defined.
- Expected outputs are listed.

## Definition of Done

A feature is complete when:

- Implementation matches architecture contracts.
- Configuration, validation, and naming behavior match their technical specifications.
- Tests pass for the relevant suites.
- Documentation references are updated.
- Output artifact expectations are documented.
