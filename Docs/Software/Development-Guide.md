# Software Development Guide

- **Version:** 1.0
- **Last Updated:** 2026-02-24

## Repository Conventions

- C# source remains under `API/`.
- New software components align with architecture layers.
- Every domain module publishes a clear interface.

## Testing Strategy

`API/Tests` uses three suites:

- `Unit/` for parser, validator, naming, and path logic.
- `Integration/` for generation flow in controlled SolidWorks sessions.
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
