# AxionFrame Program Roadmap

- **Title:** AxionFrame Program Roadmap
- **Version:** 4
- **Status:** Active
- **Owner:** Joint Software and Mechanical Engineering
- **Scope:** Stage-gated delivery of the SolidWorks 2026 workbench generator, verified manufacturing outputs, project infrastructure, and releases.

## Program Outcome

AxionFrame generates a parametric, fixed-height, fully demountable X-frame workbench in SolidWorks 2026. The program delivers verified CAD models, manufacturing data, formal verification evidence, and versioned release assets.

## Delivery Sequence

```text
R0 Governance and Rebaseline
  -> R1 Design Intent and Requirements
  -> R2 Engineering Proof and Mechanical Baseline
  -> R3 Software Contract Rebaseline
  -> R4 Build Delivery
  -> R5 SolidWorks Verification
  -> R6 Final Output
  -> R7 Platform and Prerelease
  -> R8 Release Audit and Stable Release
```

## Stage Completion Standard

| Record | Required content |
|---|---|
| Deliverables | Version-controlled engineering artifacts |
| Traceability | Requirement, decision, implementation, and evidence links |
| Verification | Repeatable procedure and recorded result |
| Acceptance | Measurable criteria with an approved result |
| Gate | Named approval record |

## R0 — Governance and Rebaseline

**Outcome:** The repository carries one authoritative product direction and one delivery system.

| Deliverable | Verification | Acceptance |
|---|---|---|
| `CR-001-MechanicalRebaselineDecision.md` | Decision review | Approved scope and impact set |
| `DecisionRegister.md` | Decision trace review | Every current decision has one identifier |
| `DocumentationStandards.md` | Language and structure review | Active documents follow the project standard |
| `DevOpsPolicy.md` | Platform policy review | GitHub workflow, Pages, security, artifact, and release rules are explicit |
| Version 4 roadmap | Stage-gate review | Every delivery stage has an outcome and gate |

**Gate R0:** Governance Baseline Approval.

## R1 — Design Intent and Requirements

**Outcome:** The selected X-frame concept carries measurable engineering requirements.

| Deliverable | Verification | Acceptance |
|---|---|---|
| Design Intent baseline | Topology review | A, B, C, D, N1, and N2 define each end frame |
| Product Requirements Specification | Requirement quality review | Load, stiffness, stability, portability, clamping, manufacturability, safety, and appearance requirements use measurable values |
| Requirements Traceability Matrix | Coverage review | Every product requirement maps to verification evidence |
| Critical Engineering Rules | Rule review | Geometry, joint, material, fastener, and output rules have identifiers |

**Gate R1:** Requirements Baseline Approval.

## R2 — Engineering Proof and Mechanical Baseline

**Outcome:** Engineering evidence establishes the selected concept as a production-ready mechanical baseline.

| Deliverable | Verification | Acceptance |
|---|---|---|
| Load-case specification | Load-path review | Static, lateral, torsional, clamping, impact, and cyclic cases have magnitudes and boundary conditions |
| Hand calculations | Independent calculation review | Strength, stiffness, stability, bolt, bearing, tear-out, slot, spacer, and local reinforcement checks meet requirements |
| FEA package | Model-quality review | Mesh, contacts, constraints, convergence, stress, displacement, and buckling evidence meet acceptance criteria |
| Representative joint test definition | Test-readiness review | Cyclic loading, measurement resolution, acceptance threshold, and evidence capture are explicit |
| Mechanical Baseline | Joint approval | Materials, profiles, plate geometry, joint architecture, fasteners, tolerances, work surface, and finishes are frozen |

**Gate R2:** Mechanical Baseline Approval.

## R3 — Software Contract Rebaseline

**Outcome:** Software contracts express the approved mechanical baseline.

| Deliverable | Verification | Acceptance |
|---|---|---|
| Configuration schema | Schema tests | `Height`, `Width`, `Length`, `Top-Plate-Thk`, `RHS-W`, `RHS-H`, `RHS-Thk`, and `Polygonal-Plate-Thk` have units, ranges, defaults, and dependencies |
| Validation specification | Boundary tests | Every engineering rule produces deterministic validation behavior |
| Naming specification | Repeatability tests | Identical inputs produce identical names and identifiers |
| Architecture baseline | Architecture review | Frame, Node Joint, Longitudinal Structure, Work Surface, Assembly, Verification, and Final Output domains have clear ownership |
| Code-directory README set | Repository audit | Every code directory carries its local technical contract |

**Gate R3:** Software Contract Approval.

## R4 — Build Delivery

**Outcome:** Build generates the complete workbench model from approved settings.

| Increment | Generated scope | Acceptance evidence |
|---|---|---|
| R4.1 | End-frame topology | Fully defined A-B-C-D-N1-N2 geometry and deterministic rebuild |
| R4.2 | Node joints | Center slots, 10 mm polygonal plates, spacers, local reinforcement, two bolts per member end, and selected retention hardware |
| R4.3 | Longitudinal structure | Two end frames and longitudinal members form the full frame |
| R4.4 | Work surface | Perforated surface and continuous four-side clamping access |
| R4.5 | Assembly | Deterministic parts, mates, configuration, and feature tree |
| R4.6 | Build evidence | Validation report, model inventory, rebuild result, and Build identity |

**Gate R4:** Build Candidate Approval.

## R5 — SolidWorks Verification

**Outcome:** The designated workstation confirms the Build candidate in SolidWorks 2026.

| Deliverable | Verification | Acceptance |
|---|---|---|
| Verification Package | Package integrity check | Commit SHA, version, configuration hash, binaries, dependencies, procedures, expected results, manifest, and SHA-256 checksums match |
| Workstation execution record | Procedure execution | SolidWorks edition, service pack, workstation, model tree, configurations, dimensions, mates, rebuild status, logs, and screenshots are recorded |
| Verification report | Result review | Every test case carries Pass, Fail, or Blocked status and linked evidence |
| Build Verification Gate record | Approval review | Required test cases carry Pass status for the verified Build identity |

**Gate R5:** Build Verification Approval.

## R6 — Final Output

**Outcome:** Final Output creates manufacturing-ready deliverables from the verified Build identity.

| Deliverable | Acceptance |
|---|---|
| STEP package | Geometry identity and export validation pass |
| DXF package | Material, thickness, scale, units, layer, and contour validation pass |
| BOM and cut list | Part identity, quantity, material, profile, length, and mass checks pass |
| Fastener schedule | Joint location, specification, quantity, and retention method checks pass |
| Validation report | Requirements and engineering-rule results are complete |
| Manifest and checksums | File inventory, Build identity, versions, hashes, and timestamps are complete |

**Gate R6:** Manufacturing Output Approval.

## R7 — Platform and Prerelease

**Outcome:** GitHub provides repeatable integration, publishing, security, project management, and prerelease distribution.

| Deliverable | Acceptance |
|---|---|
| Pull-request workflow | GitHub-hosted Windows performs fresh dependency restoration, compilation, and SolidWorks-independent tests |
| GitHub Pages workflow | The complete `Docs/` tree publishes from the approved branch |
| Security workflows | Dependabot, CodeQL, and secret scanning produce reviewed results |
| Project controls | Issues, Projects, and Milestones trace planned work and releases |
| Prerelease | Semantic version, installer or archive, manifest, checksums, release notes, and applicable verification report publish through GitHub Releases |

**Gate R7:** Prerelease Approval.

## R8 — Release Audit and Stable Release

**Outcome:** A complete evidence set supports the stable release.

| Deliverable | Verification | Acceptance |
|---|---|---|
| Traceability audit | Requirement-to-release review | Every released requirement maps to implementation and passing evidence |
| Documentation audit | Active-document review | Published documents match released behavior |
| Release asset audit | Manifest and checksum review | Every release asset has verified identity and integrity |
| Stable release | Release approval | Versioned assets, evidence, and release notes are published |

**Gate R8:** Stable Release Approval.

## Active Execution Point

The program executes R0 first. R1 begins after Governance Baseline Approval. Each later stage begins after approval of its preceding gate.
