# Mechanical Design and Manufacturing Guide

- **Title:** Mechanical Design and Manufacturing Guide
- **Scope:** This document defines mechanical design intent, manufacturing outputs, and traceability points for AxionFrame.

## Design Intent

The mechanical system delivers a modular X-frame table with height-adjustable configurations and bolted joints for production workflows.

## Core Mechanical Areas

- Frame member layout and profile selection.
- Pivot joint geometry and hole strategy.
- Height indexing strategy for supported table configurations.
- Plate and brace details with manufacturable dimensions.

## Baseline Mechanical Value Set

The following baseline values define the target mechanical specification for the first implementation and validation release of AxionFrame:

- **Frame layout baseline**
  - Primary structural member extents: `620 mm` minimum to `980 mm` maximum
  - Member placement tolerance relative to the model datum set: `+/-0.5 mm`
- **Frame profile baseline**
  - Allowed primary frame profiles: `40 x 40 x 2.0 mm SHS` and `60 x 30 x 2.0 mm RHS`
  - Profile dimensional tolerance used for validation and BOM acceptance: `+/-0.2 mm`
- **Pivot geometry baseline**
  - Pivot-axis location range from the lower frame datum: `300 mm` to `450 mm`
  - Pivot-axis alignment tolerance: `+/-0.25 mm`
- **Pivot hole baseline**
  - Allowed pivot-hole diameter range for bolted joints: `10.5 mm` to `11.0 mm`
  - Hole positional tolerance: `+/-0.2 mm`
- **Height-indexing baseline**
  - Supported finished table heights: `680 mm`, `730 mm`, and `780 mm`
  - Supported configuration count: `3`
  - Height-validation tolerance at each supported state: `+/-1.0 mm`
- **Plate and brace baseline**
  - Allowed plate thickness range: `5.0 mm` to `8.0 mm`
  - Plate and brace dimensional tolerance: `+/-0.2 mm`

These values are the approved baseline for documentation, schema design, validator design, and initial implementation planning unless a later controlled engineering change supersedes them.

## Relationship to Add-in User Flow

- Users can change default settings in the SolidWorks Add-in.
- Users select **Build** to generate CAD parts and assemblies.
- Users select **Final Output** to generate manufacturing deliverables.

## Manufacturing Deliverables

Each final-output run creates:

- Assembly files.
- Part files.
- STEP exports.
- DXF exports for plate components.
- BOM outputs.
- Validation reports.

## Engineering Traceability

Mechanical rules reference implementation points by:

- Configuration key name.
- Feature or mate naming pattern.
- Output report section.
