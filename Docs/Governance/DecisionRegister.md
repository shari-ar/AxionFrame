# Project Decision Register

- **Title:** Project Decision Register
- **Status:** Active
- **Owner:** Engineering Governance
- **Scope:** Approved product, engineering, workflow, documentation, and DevOps decisions for AxionFrame.

## Decision Authority

This register provides the current decision index. Each referenced document provides the normative technical detail.

| ID | Area | Approved Decision | Authoritative Document |
|---|---|---|---|
| DR-001 | Repository | AxionFrame continues in the existing repository and preserves its Git history. | `Docs/Workflow/CR-001-MechanicalRebaselineDecision.md` |
| DR-002 | Product concept | The selected product uses the approved fixed-height X-frame workbench concept. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-003 | Configuration | Each generated workbench has a fixed physical height, and the `Height` setting controls that height before generation. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-004 | Parameters | The initial configurable parameter set uses the values defined in the approved equation baseline. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-005 | End frames | The assembly uses two identical X-frame structures, one at each end of the workbench. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-006 | Node geometry | Each end frame uses two distinct internal nodes named `N1` and `N2`. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-007 | Joint architecture | Each member end receives a centered axial slot that captures a polygonal node plate inside the RHS section. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-008 | Fastening | Each member-to-node connection uses two fasteners and two internal spacer elements around the node plate. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-009 | Local reinforcement | Connection regions receive local reinforcement while the baseline RHS wall thickness remains configurable. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-010 | Work surface | The workbench uses a perforated work surface with continuous clamping access around all four sides. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-011 | Portability | Operational portability uses complete disassembly into transportable components and a constant operating footprint. | `Docs/Mechanical/DesignAndManufacturing.md` |
| DR-012 | Engineering process | Engineering development uses Design Intent, Requirements, Feasibility, Engineering Proof, and Mechanical Baseline approval. | `roadmap.md` |
| DR-013 | Concept authority | The selected X-frame concept enters engineering development as the sole approved concept input. | `Docs/Workflow/CR-001-MechanicalRebaselineDecision.md` |
| DR-014 | Runtime workflow | Runtime delivery uses Settings, Input Validation, Build, Build Verification Gate, and Final Output. | `Docs/Workflow/ProjectLifecycle.md` |
| DR-015 | SolidWorks verification | SolidWorks 2026 execution occurs on the designated external verification workstation through a formal verification package. | `Docs/Verification/SolidWorksVerificationPolicy.md` |
| DR-016 | Repository visibility | The GitHub repository uses public visibility on the GitHub Free plan. | `Docs/Governance/DevOpsPolicy.md` |
| DR-017 | CI execution | GitHub-hosted Windows runners execute compilation and SolidWorks-independent verification through fresh builds. | `Docs/Governance/DevOpsPolicy.md` |
| DR-018 | Documentation delivery | GitHub Pages publishes the complete `Docs/` set through a custom GitHub Actions workflow. | `Docs/Governance/DevOpsPolicy.md` |
| DR-019 | Distribution | GitHub Releases distributes versioned desktop assets, manifests, checksums, and release notes. | `Docs/Governance/DevOpsPolicy.md` |
| DR-020 | Documentation language | Active documentation uses precise, affirmative US English for a technical audience. | `Docs/Governance/DocumentationStandards.md` |
| DR-021 | R1 performance baseline | The rated workbench uses the measurable product, structural, joint, work-surface, portability, manufacturing, CAD, and delivery requirements in the R1 baseline. | `Docs/Mechanical/ProductRequirementsSpecification.md` |
| DR-022 | Engineering implementation | SolidWorks 2026 documentation provides the sole software authority, every code directory carries a local README, and each automated operation preserves fully defined mechanical and manufacturing intent. | `AGENTS.md` and `Docs/Software/DevelopmentGuide.md` |

## Change Control

Every approved change updates this register, its authoritative document, the active roadmap, and the affected verification scope in one pull request.
