# Project Lifecycle Workflow

- **Version:** 1.0
- **Last Updated:** 2026-02-24

## Lifecycle

1. Define requirements.
2. Define configuration schema.
3. Implement add-in engines.
4. Generate CAD parts and assemblies.
5. Run validation checks.
6. Export manufacturing deliverables.
7. Release versioned outputs and reports.

## Runtime Generation Flow

1. User starts generation from add-in command.
2. Configuration is loaded and validated.
3. A versioned output run folder is created.
4. Parts are generated from templates.
5. Assembly and configurations are generated.
6. Validation services produce quality checks.
7. Export services generate STEP, DXF, BOM, and reports.
8. The run summary is presented.

## Release Flow

1. Revision metadata is updated.
2. Deliverables are generated in a versioned run path.
3. Release artifacts are promoted to a release folder.
4. Documentation and ADR records are updated.
