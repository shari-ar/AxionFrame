# Software Architecture

- **Title:** Software Architecture
- **Status:** Active Target Baseline
- **Owner:** Software Engineering
- **Scope:** Layer ownership, product domains, orchestration boundaries, and engineering traceability for the AxionFrame SolidWorks 2026 add-in.

## Platform Outcome

AxionFrame converts validated workbench settings into a deterministic SolidWorks 2026 Build. A verified Build identity then produces manufacturing-ready Final Output artifacts.

## Runtime Flow

```text
Settings
  -> Input Validation
  -> Build Orchestrator
  -> Generated Parts and Assembly
  -> Build Evidence
  -> Build Verification Gate
  -> Final Output Orchestrator
  -> Release Assets
```

## Layered Architecture

| Layer | Location | Responsibilities |
|---|---|---|
| Add-in Host | `API/Addin` | SolidWorks lifecycle, commands, user interface, events, and workflow entry points |
| Core Engine | `API/Core` | Session orchestration, configuration, validation, geometry services, assembly services, evidence, exports, diagnostics, and run identity |
| Domain Modules | `API/Modules` | Product geometry and domain-specific engineering rules |
| Shared Contracts | `API/Shared` | Typed configuration, identifiers, data-transfer contracts, units, and reusable utilities |
| Tests | `API/Tests` | Unit, contract, integration, deterministic-regression, and SolidWorks-independent verification |

## Product Domains

| Domain | Ownership |
|---|---|
| Frame | End-frame reference geometry, A-B-C-D-N1-N2 topology, RHS members, and longitudinal frame structure |
| Node Joint | Polygonal plates, member-end slots, spacers, local reinforcement, fasteners, retention hardware, fits, and clearances |
| Work Surface | Perforated top, support interfaces, and continuous four-side clamping access |
| Assembly | Component placement, mates, configurations, rebuild state, and mass properties |
| Verification | Build identity, engineering-rule evidence, verification package, workstation evidence, and gate result |
| Final Output | STEP, DXF, BOM, cut list, fastener schedule, reports, manifests, and checksums |

## Core Services

| Service | Contract |
|---|---|
| Configuration | Loads typed parameters, units, defaults, ranges, and dependencies |
| Validation | Evaluates engineering rules before feature creation |
| Geometry | Creates fully defined sketches, reference geometry, features, bodies, and parts |
| Assembly | Creates deterministic component placement, mates, configurations, and rebuild operations |
| Naming | Produces stable document, feature, body, component, mate, and output identifiers |
| Evidence | Records input identity, Build identity, rule results, model inventory, and execution metadata |
| Export | Produces Final Output artifacts from an approved Build identity |

## Orchestration Rules

1. The configuration service creates one immutable validated settings snapshot.
2. The Build orchestrator assigns one Build identity to the settings snapshot and generated model set.
3. Domain modules consume typed settings and approved engineering rules.
4. Geometry services create fully defined SolidWorks 2026 features through documented API contracts.
5. Evidence services record every generated document, feature group, assembly state, and validation result.
6. The Build Verification Gate binds its approval to one Build identity.
7. The Final Output orchestrator consumes the approved Build identity and its generated model set.
8. Release packaging binds binaries, documentation, verification evidence, outputs, manifest, and checksums to one version.

## Engineering Traceability

```text
Decision ID
  -> Requirement ID
  -> Engineering Rule ID
  -> Configuration or Domain Contract
  -> Implementation Component
  -> Test Case ID
  -> Evidence Record
  -> Release Version
```

## Design Principles

- Every generated result is deterministic for an identical validated settings snapshot.
- Every mechanical feature carries explicit engineering intent, units, constraints, and acceptance rules.
- Every code directory carries a local `README.md` technical contract.
- Every SolidWorks automation behavior follows SolidWorks 2026 documentation.
- Every feature-creation sequence mirrors a professional mechanical-engineering workflow.
- Every Final Output artifact traces to one approved Build identity.
