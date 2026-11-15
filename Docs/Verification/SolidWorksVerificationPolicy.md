# SolidWorks Verification Policy

- **Title:** SolidWorks Verification Policy
- **Status:** Active
- **Owner:** Joint Software and Mechanical Verification
- **Scope:** Formal handoff, execution, evidence capture, and acceptance for SolidWorks 2026 verification.

## Verification Roles

- **Build Producer:** Creates the verified software build and verification package.
- **SolidWorks Verification Operator:** Executes the package on the designated SolidWorks 2026 workstation and records evidence.
- **Engineering Reviewer:** Evaluates results against requirements and acceptance criteria.

## Verification Package Contents

Each package contains:

- Package ID
- Git commit SHA
- Branch or tag
- Product version
- Build timestamp
- Configuration payload
- Configuration hash
- Application binaries
- Required runtime dependencies
- Test plan
- Test cases
- Execution procedure
- Expected results
- Acceptance criteria
- Evidence directory structure
- Manifest
- SHA-256 checksums

## SolidWorks Execution Evidence

The operator records:

- SolidWorks 2026 edition and service pack
- Workstation identifier
- Test execution timestamp
- Generated part and assembly files
- Feature-tree screenshots
- Configuration screenshots
- Dimension measurements
- Mate and rebuild status
- Add-in logs
- Generated output inventory
- Test result for every case
- Linked issue identifiers for observed defects

## Result States

- **Pass:** Every acceptance criterion succeeds.
- **Fail:** At least one acceptance criterion produces a confirmed deviation.
- **Blocked:** The workstation, installation, dependency, or input state prevents execution.

## Build Verification Gate

A Pass result authorizes Final Output execution for the verified commit and configuration hash. Each new commit or configuration change receives a new verification identity.

## Evidence Storage

- Pull requests store verification summaries and links.
- Workflow artifacts store verification packages and returned evidence for thirty days.
- Release assets store the verification report associated with each published release.
- The roadmap evidence registry stores package ID, commit SHA, configuration hash, result, reviewer, and completion date.
