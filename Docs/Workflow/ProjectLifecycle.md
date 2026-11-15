# Project Lifecycle Workflow

- **Title:** Project Lifecycle Workflow
- **Status:** Active
- **Owner:** Joint Software and Mechanical Engineering
- **Scope:** Engineering, implementation, Build, verification, Final Output, and release lifecycle.

## Program Lifecycle

1. Record approved decisions.
2. Define Design Intent.
3. Define measurable requirements and traceability.
4. Complete feasibility and Engineering Proof.
5. Approve the Mechanical Baseline.
6. Rebaseline software contracts and validators.
7. Implement SolidWorks generation modules.
8. Execute automated verification.
9. Produce the SolidWorks verification package.
10. Execute SolidWorks 2026 verification on the designated workstation.
11. Approve the Build Verification Gate.
12. Generate Final Output artifacts.
13. Complete release verification.
14. Publish the versioned release.

## Runtime User Flow

1. The operator opens AxionFrame inside SolidWorks 2026.
2. The operator reviews and updates configuration settings.
3. AxionFrame validates the input contract.
4. The operator selects **Build**.
5. AxionFrame generates deterministic parts and assemblies.
6. AxionFrame records run metadata, configuration hash, logs, and CAD evidence.
7. The Build Verification Gate evaluates the approved evidence set.
8. The operator selects **Final Output** for a verified Build identity.
9. AxionFrame generates STEP, DXF, BOM, validation reports, manifests, and checksums.
10. AxionFrame presents the run summary and output inventory.

## Build Scope

Build includes:

- Configuration loading
- Default resolution
- Input validation
- Part generation
- Assembly generation
- Deterministic naming
- Rebuild evaluation
- Geometry validation
- Run metadata
- Structured logging
- Verification evidence generation

## Build Verification Gate

The gate evaluates:

- Commit SHA
- Product version
- Configuration payload and hash
- Automated test results
- SolidWorks 2026 verification package result
- Critical requirement coverage
- Geometry and dimension results
- Rebuild and mate status
- Artifact manifest integrity

A Pass result authorizes Final Output for the verified Build identity.

## Final Output Scope

Final Output includes:

- SolidWorks assembly and part files
- STEP exports
- DXF exports for plate components
- Bill of Materials
- Cut list
- Fastener schedule
- Validation report
- Manifest
- SHA-256 checksums
- Run summary

## Release Flow

1. The release candidate receives a semantic version.
2. CI executes fresh-build checks.
3. The verification package receives a Pass result.
4. Final Output generates the release artifact set.
5. The release manifest reconciles every artifact.
6. Release notes describe approved changes and verification evidence.
7. GitHub Releases publishes the durable distribution assets.
8. GitHub Pages publishes the matching documentation baseline.

## Related Documents

- `../Governance/DecisionRegister.md`
- `../Governance/DevOpsPolicy.md`
- `CR-001-MechanicalRebaselineDecision.md`
- `../Mechanical/DesignAndManufacturing.md`
- `../Verification/SolidWorksVerificationPolicy.md`
- `../../roadmap.md`
