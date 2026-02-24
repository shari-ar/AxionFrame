# Software Development Guide

- **Version:** 1.1
- **Last Updated:** 2026-02-24

## Repository Conventions

- C# source resides under `API/`.
- Software components align with architecture layers.
- Domain modules publish clear interfaces.

## Add-in Command Model

- **Build Command** orchestrates CAD generation flow.
- **Final Output Command** orchestrates export flow and report generation.

## Testing Strategy

`API/Tests` uses three suites:

- `Unit/` for parser, validator, naming, and path logic.
- `Integration/` for Add-in flow and CAD generation workflow.
- `Regression/` for stability checks across revisions.

## Definition of Ready

A feature is ready for implementation when:

- Functional requirements exist in `Docs/Workflow` or `Docs/Mechanical`.
- Configuration keys and schema entries are defined.
- Expected outputs are listed.

## Definition of Done

A feature is complete when:

- Implementation matches architecture contracts.
- Tests pass for the relevant suites.
- Documentation references are updated.
- Output artifact expectations are documented.
