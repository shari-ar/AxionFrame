# Validation and Error Handling Specification

- **Title:** Validation and Error Handling Specification
- **Scope:** This document defines AxionFrame validator behavior, enforcement severity, execution timing, rule-to-validator mapping, and deterministic error reporting requirements.

## Purpose

This document is the canonical specification for how AxionFrame validates configuration, geometry, manufacturing readiness, export readiness, and release readiness.

## Validation Objectives

- Reject invalid configuration before geometry generation.
- Prevent critical mechanical-rule violations from reaching release outputs.
- Produce deterministic, auditable validation results.
- Map every critical rule to at least one validator or report check.

## Validation Layers

1. **Schema Validation**
   - Type checks
   - Required-field checks
   - Numeric-range checks
   - Enumerated-set checks
2. **Cross-Field Validation**
   - Dependency and consistency checks across related keys
3. **Build Validation**
   - Geometry existence and activation checks after Build
4. **Final Output Validation**
   - Export eligibility, BOM readiness, and output completeness checks
5. **Release Validation**
   - Final gate checks before packaging or promotion

## Severity Model

- **Critical** — execution must stop; release output cannot proceed
- **Error** — current operation fails, but prior artifacts remain auditable
- **Warning** — operation may continue; issue must be recorded in the validation report
- **Info** — non-blocking traceability or observability message

## Enforcement Rules

- All `Strict` rules from `Docs/Mechanical/S1.3-CriticalRuleIndex.md` must be enforced at `Critical` or `Error` severity.
- A `Critical` failure blocks `Build`, `Final Output`, and packaging progression.
- An `Error` failure blocks the current stage but does not invalidate already-recorded evidence.
- A `Warning` never blocks execution by itself.
- Validation results must be deterministic for identical inputs.

## Execution Timing

- Schema and cross-field validation run before any geometry generation.
- Build validation runs after part and assembly generation but before Build is reported complete.
- Final Output validation runs before each export class and again after artifact generation for completeness checks.
- Release validation runs before final packaging and promotion.

## Error Message Contract

Each blocking or non-blocking validation message must include:

- `severity`
- `ruleId`
- `validatorId`
- `configKey` or `artifactScope`
- `message`
- `expected`
- `actual`
- `recommendedAction`

## Determinism Rules

- The same failing input must produce the same `ruleId`, `validatorId`, and `message` text.
- Numeric values in messages must use normalized units and formatting.
- Validation ordering must be stable by validator group and `ruleId`.

## Rule-to-Validator Mapping

| Rule ID | Validator ID | Validation Layer | Severity | Blocking |
|---|---|---|---|---|
| `FRM-001` | `VAL-FRM-001-LAYOUT` | Build Validation | Critical | Yes |
| `FRM-002` | `VAL-FRM-002-PROFILE` | Schema + Build Validation | Critical | Yes |
| `FRM-003` | `VAL-FRM-003-NAMING` | Build Validation | Warning | No |
| `PVT-001` | `VAL-PVT-001-GEOMETRY` | Build Validation | Critical | Yes |
| `PVT-002` | `VAL-PVT-002-HOLES` | Build + Final Output Validation | Critical | Yes |
| `PVT-003` | `VAL-PVT-003-MATES` | Build Validation | Warning | No |
| `HGT-001` | `VAL-HGT-001-SUPPORTED-CONFIGS` | Schema + Build Validation | Critical | Yes |
| `HGT-002` | `VAL-HGT-002-ACTIVATION` | Build Validation | Critical | Yes |
| `HGT-003` | `VAL-HGT-003-HEIGHT-VALIDITY` | Build + Release Validation | Critical | Yes |
| `PLT-001` | `VAL-PLT-001-DIMENSIONS` | Build Validation | Critical | Yes |
| `PLT-002` | `VAL-PLT-002-DXF-ELIGIBILITY` | Final Output Validation | Critical | Yes |
| `PLT-003` | `VAL-PLT-003-NAMING` | Build + Final Output Validation | Warning | No |

## Report Integration

- All validation messages appear in the validation report.
- Warnings and infos are retained even when the operation succeeds.
- A final pass/fail result is derived from the highest severity encountered.

## Source Documents Used for Confirmation

- `Docs/Mechanical/S1.1-RequirementTraceMatrix.md`
- `Docs/Mechanical/S1.3-CriticalRuleIndex.md`
- `Docs/Software/Architecture.md`
- `Docs/Workflow/ProjectLifecycle.md`
- `roadmap.md`
