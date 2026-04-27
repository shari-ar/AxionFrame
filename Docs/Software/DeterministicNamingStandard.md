# Deterministic Naming Standard

- **Title:** Deterministic Naming Standard
- **Scope:** This document defines the deterministic naming grammar for AxionFrame features, mates, configurations, exports, and traceability identifiers.

## Purpose

This document provides the naming rules that ensure repeated runs with identical inputs produce identical names and traceability references.

## Naming Principles

- Names are deterministic.
- Names are machine-parseable.
- Names encode domain ownership.
- Names avoid user-local or timestamp-derived content.
- Names remain stable across repeated runs with identical inputs.

## Canonical Prefixes

- `AXF_FRM_` for frame features
- `AXF_PVT_` for pivot features
- `AXF_PLT_` for plate and brace features
- `AXF_MATE_` for assembly mates
- `AXF_CFG_` for configuration states
- `AXF_EXP_` for export artifacts
- `AXF_VAL_` for validation groups or report sections

## Grammar

- **Feature names:** `AXF_<DOMAIN>_<COMPONENT>_<DESCRIPTOR>`
- **Mate names:** `AXF_MATE_<DOMAIN>_<DESCRIPTOR>`
- **Configuration names:** `AXF_CFG_<DOMAIN>_<DESCRIPTOR>`
- **Export artifact names:** `AXF_EXP_<TYPE>_<DESCRIPTOR>`
- **Validation section identifiers:** `AXF_VAL_<DOMAIN>_<DESCRIPTOR>`

## Domain Tokens

- `FRM` for Frame
- `PVT` for Pivot
- `HGT` for Height Indexing
- `PLT` for Plate and Brace

## Normalization Rules

- Use uppercase ASCII letters, digits, and underscore separators only.
- Replace spaces and punctuation with underscores.
- Do not emit duplicate underscores.
- Do not append random IDs or timestamps to deterministic names.
- Numeric tokens must use canonical decimal-free forms when the value is an approved discrete state, for example `680` rather than `680.0`.

## Required Stable Hooks

| Requirement ID | Required Hook |
|---|---|
| `FRM-001` | `AXF_FRM_LAYOUT_PRIMARY` |
| `FRM-002` | `AXF_FRM_PROFILE_MAIN` |
| `FRM-003` | `AXF_FRM_*` |
| `PVT-001` | `AXF_PVT_JOINT_PRIMARY` |
| `PVT-002` | `AXF_PVT_HOLE_PATTERN` |
| `PVT-003` | `AXF_MATE_PVT_PRIMARY` |
| `HGT-001` | `AXF_CFG_HEIGHT_*` |
| `HGT-002` | `AXF_CFG_HEIGHT_INDEXED` |
| `HGT-003` | `AXF_HGT_VALIDATION_SET` |
| `PLT-001` | `AXF_PLT_BRACE_PRIMARY` |
| `PLT-002` | `AXF_PLT_EXPORT_DXF` |
| `PLT-003` | `AXF_PLT_*` |

## Examples

- `AXF_FRM_LAYOUT_PRIMARY`
- `AXF_PVT_JOINT_PRIMARY`
- `AXF_MATE_PVT_PRIMARY`
- `AXF_CFG_HGT_680`
- `AXF_EXP_DXF_PLATE_SET`
- `AXF_VAL_PLT_TRACEABILITY`

## Naming Compliance Rule

A run is naming-compliant only when all names required by the active rule set match the expected canonical grammar and remain unchanged across repeated runs with identical inputs.

## Source Documents Used for Confirmation

- `Docs/Mechanical/S1.1-RequirementTraceMatrix.md`
- `Docs/Mechanical/S1.3-CriticalRuleIndex.md`
- `Docs/Software/Architecture.md`
- `roadmap.md`
