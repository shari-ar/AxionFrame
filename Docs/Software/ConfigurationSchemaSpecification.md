# Configuration Schema Specification

- **Title:** Configuration Schema Specification
- **Scope:** This document defines the target configuration schema for AxionFrame, including key structure, data types, required/default behavior, units policy, allowed enumerations, and cross-field validation rules.

## Purpose

This document provides the canonical configuration contract for AxionFrame implementation. It is the normative source for:

- configuration-key structure
- data typing and units
- required versus optional fields
- baseline default values
- allowed enumerated sets and numeric ranges
- cross-field dependency rules

## Schema Design Principles

- Configuration keys use stable dot-separated paths.
- Each key has one declared type.
- Units are explicit in documentation and implicit in schema field meaning.
- Required keys must always be present after default resolution.
- Defaults must be deterministic and version-controlled.
- Cross-field validation rules must be evaluated after primitive type validation.

## Canonical Schema Areas

- `frame.*` for frame geometry and profile selection
- `pivot.*` for pivot geometry and hole strategy
- `height.*` for supported configurations, activation, and validation
- `plateBrace.*` for fabrication-relevant plate and brace behavior
- `exports.*` for final-output orchestration settings
- `validation.*` for validation execution behavior
- `run.*` for run metadata and packaging behavior

## Type System

The schema supports the following field types:

- `string`
- `boolean`
- `integer`
- `decimal`
- `enum`
- `array<string>`
- `array<decimal>`
- `object`

## Units Policy

- Linear dimensions are expressed in `mm`.
- Angular dimensions are expressed in `deg`.
- Counts are expressed as `integer` values.
- Tolerances use the same unit family as the validated value.
- Enumerated profile names are unit-qualified inside the literal value when needed.

## Required Configuration Table

| Key | Type | Required | Default | Allowed Range / Set | Unit | Notes |
|---|---|---|---|---|---|---|
| `frame.layout.primary.memberExtentMin` | `decimal` | Yes | `620.0` | `620.0-980.0` | `mm` | Lower bound for baseline frame member extent |
| `frame.layout.primary.memberExtentMax` | `decimal` | Yes | `980.0` | `620.0-980.0` | `mm` | Upper bound for baseline frame member extent |
| `frame.layout.primary.placementTolerance` | `decimal` | Yes | `0.5` | `0.0-0.5` | `mm` | Maximum permitted placement deviation |
| `frame.profile.selection.allowedProfiles` | `array<string>` | Yes | `40x40x2.0_SHS,60x30x2.0_RHS` | `40x40x2.0_SHS`, `60x30x2.0_RHS` | `N/A` | Approved baseline frame profiles |
| `frame.profile.selection.dimensionTolerance` | `decimal` | Yes | `0.2` | `0.0-0.2` | `mm` | Profile dimensional tolerance |
| `frame.naming.ruleSet` | `enum` | Yes | `AXF_STANDARD_V1` | `AXF_STANDARD_V1` | `N/A` | Naming grammar version for frame features |
| `pivot.geometry.primary.axisLocationMin` | `decimal` | Yes | `300.0` | `300.0-450.0` | `mm` | Lower bound for pivot-axis location |
| `pivot.geometry.primary.axisLocationMax` | `decimal` | Yes | `450.0` | `300.0-450.0` | `mm` | Upper bound for pivot-axis location |
| `pivot.geometry.primary.alignmentTolerance` | `decimal` | Yes | `0.25` | `0.0-0.25` | `mm` | Pivot-axis alignment tolerance |
| `pivot.hole.strategy.diameterMin` | `decimal` | Yes | `10.5` | `10.5-11.0` | `mm` | Lower bound for pivot-hole diameter |
| `pivot.hole.strategy.diameterMax` | `decimal` | Yes | `11.0` | `10.5-11.0` | `mm` | Upper bound for pivot-hole diameter |
| `pivot.hole.strategy.positionTolerance` | `decimal` | Yes | `0.2` | `0.0-0.2` | `mm` | Pivot-hole positional tolerance |
| `pivot.naming.mates` | `enum` | Yes | `AXF_STANDARD_V1` | `AXF_STANDARD_V1` | `N/A` | Naming grammar version for pivot mates |
| `height.supportedConfigurations.values` | `array<decimal>` | Yes | `680.0,730.0,780.0` | `680.0`, `730.0`, `780.0` | `mm` | Supported finished table heights |
| `height.indexing.activation.requiredCount` | `integer` | Yes | `3` | `3` | `count` | Expected number of supported height states |
| `height.indexing.activation.strictDeterminism` | `boolean` | Yes | `true` | `true,false` | `N/A` | Requires deterministic state activation |
| `height.validation.supportedSet` | `array<decimal>` | Yes | `680.0,730.0,780.0` | `680.0`, `730.0`, `780.0` | `mm` | Height set accepted by final validation |
| `height.validation.dimensionTolerance` | `decimal` | Yes | `1.0` | `0.0-1.0` | `mm` | Maximum permitted height deviation |
| `plateBrace.dimensions.primary.thicknessMin` | `decimal` | Yes | `5.0` | `5.0-8.0` | `mm` | Lower bound for plate thickness |
| `plateBrace.dimensions.primary.thicknessMax` | `decimal` | Yes | `8.0` | `5.0-8.0` | `mm` | Upper bound for plate thickness |
| `plateBrace.dimensions.primary.dimensionTolerance` | `decimal` | Yes | `0.2` | `0.0-0.2` | `mm` | Plate and brace dimensional tolerance |
| `plateBrace.export.dxfEligible` | `boolean` | Yes | `true` | `true,false` | `N/A` | Indicates fabrication DXF eligibility |
| `plateBrace.naming.ruleSet` | `enum` | Yes | `AXF_STANDARD_V1` | `AXF_STANDARD_V1` | `N/A` | Naming grammar version for plate/brace features |
| `exports.step.enabled` | `boolean` | Yes | `true` | `true,false` | `N/A` | Enables STEP generation |
| `exports.dxf.enabled` | `boolean` | Yes | `true` | `true,false` | `N/A` | Enables DXF generation |
| `exports.bom.enabled` | `boolean` | Yes | `true` | `true,false` | `N/A` | Enables BOM generation |
| `exports.validationReport.enabled` | `boolean` | Yes | `true` | `true,false` | `N/A` | Enables validation report generation |
| `validation.mode` | `enum` | Yes | `StrictRelease` | `BuildOnly`, `FinalOutput`, `StrictRelease` | `N/A` | Selects validation enforcement scope |
| `validation.stopOnCriticalFailure` | `boolean` | Yes | `true` | `true,false` | `N/A` | Stops execution on critical rule failure |
| `run.packageOutputs` | `boolean` | Yes | `true` | `true,false` | `N/A` | Enables run-folder packaging |

## Cross-Field Rules

- `frame.layout.primary.memberExtentMin` must be less than or equal to `frame.layout.primary.memberExtentMax`.
- `pivot.geometry.primary.axisLocationMin` must be less than or equal to `pivot.geometry.primary.axisLocationMax`.
- `pivot.hole.strategy.diameterMin` must be less than or equal to `pivot.hole.strategy.diameterMax`.
- `plateBrace.dimensions.primary.thicknessMin` must be less than or equal to `plateBrace.dimensions.primary.thicknessMax`.
- `height.supportedConfigurations.values` must match `height.validation.supportedSet` exactly for release validation.
- `height.indexing.activation.requiredCount` must equal the number of entries in `height.supportedConfigurations.values`.
- If `exports.dxf.enabled` is `true`, then `plateBrace.export.dxfEligible` must also be `true` for at least one fabrication-relevant component.
- If `validation.mode` is `StrictRelease`, then `validation.stopOnCriticalFailure` must be `true`.

## Default Resolution Rules

- Defaults are applied before cross-field validation.
- User-supplied values override defaults only when they remain inside the allowed type and range.
- Arrays with approved baseline defaults must preserve documented ordering unless explicitly documented otherwise.

## Traceability Mapping

| Trace Matrix Key | Schema Area |
|---|---|
| `frame.layout.primary` | `frame.layout.primary.*` |
| `frame.profile.selection` | `frame.profile.selection.*` |
| `frame.naming.ruleSet` | `frame.naming.ruleSet` |
| `pivot.geometry.primary` | `pivot.geometry.primary.*` |
| `pivot.hole.strategy` | `pivot.hole.strategy.*` |
| `pivot.naming.mates` | `pivot.naming.mates` |
| `height.supportedConfigurations` | `height.supportedConfigurations.values` |
| `height.indexing.activation` | `height.indexing.activation.*` |
| `height.validation.supportedSet` | `height.validation.*` |
| `plateBrace.dimensions.primary` | `plateBrace.dimensions.primary.*` |
| `plateBrace.export.dxfEligible` | `plateBrace.export.dxfEligible` |
| `plateBrace.naming.ruleSet` | `plateBrace.naming.ruleSet` |

## Source Documents Used for Confirmation

- `Docs/Software/Architecture.md`
- `Docs/Software/DevelopmentGuide.md`
- `Docs/Mechanical/DesignAndManufacturing.md`
- `Docs/Mechanical/S1.1-RequirementTraceMatrix.md`
- `Docs/Mechanical/S1.3-CriticalRuleIndex.md`
- `Docs/Workflow/ProjectLifecycle.md`
- `roadmap.md`
