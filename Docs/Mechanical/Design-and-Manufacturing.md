# Mechanical Design and Manufacturing Guide

- **Version:** 1

## Design Intent

The mechanical system delivers a modular X-frame table with height-adjustable configurations and bolted joints for production workflows.

## Core Mechanical Areas

- Frame member layout and profile selection.
- Pivot joint geometry and hole strategy.
- Height indexing strategy for supported table configurations.
- Plate and brace details with manufacturable dimensions.

## Relationship to Add-in User Flow

- User can change default settings in SolidWorks Add-in.
- User clicks **Build** to generate CAD parts and assemblies.
- User clicks **Final Output** to generate manufacturing deliverables.

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
