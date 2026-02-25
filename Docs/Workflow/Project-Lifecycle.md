# Project Lifecycle Workflow

- **Version:** 1

## Lifecycle

1. Define requirements.
2. Define configuration schema.
3. Implement add-in engines.
4. Build CAD files through the Add-in.
5. Run validation checks.
6. Generate final manufacturing deliverables.
7. Publish release outputs and reports.

## Runtime User Flow in SolidWorks

1. User opens the AxionFrame Add-in inside SolidWorks.
2. User can change default settings in the Add-in interface.
3. User clicks **Build**.
4. Add-in generates CAD parts and assemblies.
5. User reviews generated CAD in SolidWorks.
6. User clicks **Final Output**.
7. Add-in generates STEP, DXF, BOM, and reports in `Output/`.
8. Add-in presents a run summary with generated files.

## Build Operation Scope

Build operation covers:

- Parameter loading.
- Parameter validation.
- CAD part generation.
- CAD assembly generation.
- Configuration activation for target table heights.

## Final Output Operation Scope

Final Output operation covers:

- STEP export.
- DXF export for plate components.
- BOM generation.
- Validation report generation.
- Output packaging in run-based folders.

## Release Flow

1. Revision metadata is updated.
2. Deliverables are generated in a versioned run path.
3. Release artifacts are promoted to a release folder.
4. Documentation and ADR records are updated.
