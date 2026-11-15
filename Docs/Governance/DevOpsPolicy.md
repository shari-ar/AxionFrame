# DevOps and GitHub Policy

- **Title:** DevOps and GitHub Policy
- **Status:** Active
- **Owner:** Software Engineering
- **Scope:** Source control, continuous integration, documentation publishing, verification handoff, security, artifacts, and releases.

## Repository Model

- The repository uses public visibility on the GitHub Free plan.
- The `main` branch represents the approved integration baseline.
- Short-lived feature branches contain scoped changes.
- Pull requests provide review, traceability, and required-check evidence.
- Branch protection requires approved checks before integration.

## Continuous Integration

- GitHub-hosted Windows runners execute compilation, unit tests, documentation checks, and SolidWorks-independent regression checks.
- Every workflow performs a fresh dependency restoration and a fresh build.
- Workflow definitions use version-pinned actions.
- CI results provide logs, test reports, configuration hashes, and build metadata.

## SolidWorks Verification Handoff

- The designated external workstation provides SolidWorks 2026 execution.
- Each verification cycle receives a formal package defined in `Docs/Verification/SolidWorksVerificationPolicy.md`.
- Verification evidence returns to the associated pull request, issue, or roadmap record.
- A passing Build Verification Gate authorizes Final Output execution.

## GitHub Pages

- A custom GitHub Actions workflow publishes the complete `Docs/` source tree.
- Documentation publishing runs after approved changes reach `main`.
- The published site reflects the active documentation index in `Docs/DocumentationHub.md`.
- Documentation publishing uses a fresh build process.

## Project Management

- GitHub Issues record features, defects, engineering tasks, and verification findings.
- GitHub Projects provides the operational roadmap board.
- GitHub Milestones group work by engineering gate and release target.
- Pull requests link the relevant issue, requirement IDs, verification evidence, and milestone.

## Security and Quality

- Dependabot monitors supported dependencies and GitHub Actions references.
- CodeQL analyzes supported source code.
- Secret scanning protects repository content.
- Repository permissions follow least-privilege access.

## Artifacts and Releases

- Workflow artifacts retain build outputs, logs, test results, and verification packages for thirty days.
- GitHub Releases provide the durable software distribution channel.
- Semantic Versioning identifies release history.
- Pre-release versions use the `0.x` series during engineering development.
- Release assets include the installer or distribution archive, manifest, checksums, release notes, and applicable verification report.
- Approved reference CAD assets use Git LFS when repository size and access patterns justify that storage model.
- Generated CAD and manufacturing outputs use workflow artifacts and release assets.

## Deployment Environments

- The `cad-test` environment tracks the external SolidWorks verification gate.
- The `release` environment tracks final publication authorization.
- GitHub Pages provides the project documentation deployment target.
- GitHub Releases provide the desktop application delivery target.
