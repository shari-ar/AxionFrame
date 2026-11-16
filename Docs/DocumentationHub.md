# AxionFrame Documentation Hub

- **Title:** AxionFrame Documentation Hub
- **Status:** Active
- **Owner:** Joint Software and Mechanical Engineering
- **Scope:** Active documentation structure, reading order, authority, and lifecycle classification.

## Product Outcome

AxionFrame generates a parametric, fixed-height, fully demountable X-frame workbench through a SolidWorks 2026 add-in. Build creates CAD parts and assemblies. Build Verification confirms the generated result. Final Output creates STEP, DXF, BOM, validation reports, manifests, and checksums.

## Documentation Authority

| Priority | Document | Authority |
|---:|---|---|
| 1 | `Governance/DecisionRegister.md` | Approved decision index |
| 2 | `Workflow/CR-001-MechanicalRebaselineDecision.md` | Active product transition decision |
| 3 | `Mechanical/DesignAndManufacturing.md` | Active mechanical design intent |
| 4 | `Mechanical/ProductRequirementsSpecification.md` | Measurable R1 product requirements |
| 5 | `Mechanical/RequirementsTraceabilityMatrix.md` | Decision-to-evidence traceability |
| 6 | `Mechanical/CriticalEngineeringRules.md` | Mandatory engineering-rule index |
| 7 | `../roadmap.md` | Active delivery sequence and gates |
| 8 | `Workflow/ProjectLifecycle.md` | Runtime and release lifecycle |
| 9 | `Software/Architecture.md` | Software architecture baseline |
| 10 | `Software/DevelopmentGuide.md` | Implementation and verification rules |
| 11 | `Governance/DevOpsPolicy.md` | GitHub, CI, Pages, artifact, and release policy |
| 12 | `Verification/SolidWorksVerificationPolicy.md` | SolidWorks verification package and evidence policy |
| 13 | `Governance/DocumentationStandards.md` | Documentation language and quality rules |

## Directory Structure

- **Governance/** contains decisions, documentation standards, ownership rules, and DevOps policy.
- **Mechanical/** contains product intent, engineering requirements, manufacturing rules, and mechanical verification records.
- **Software/** contains architecture, contracts, naming, validation, and development guidance.
- **Workflow/** contains lifecycle definitions, change records, feature tickets, and stage evidence.
- **Verification/** contains formal verification policies, plans, packages, and reports.

## Recommended Reading Order

1. `Governance/DecisionRegister.md`
2. `Workflow/CR-001-MechanicalRebaselineDecision.md`
3. `Mechanical/DesignAndManufacturing.md`
4. `Mechanical/ProductRequirementsSpecification.md`
5. `Mechanical/RequirementsTraceabilityMatrix.md`
6. `Mechanical/CriticalEngineeringRules.md`
7. `../roadmap.md`
8. `Workflow/ProjectLifecycle.md`
9. `Software/Architecture.md`
10. `Software/DevelopmentGuide.md`
11. `Governance/DevOpsPolicy.md`
12. `Verification/SolidWorksVerificationPolicy.md`
13. `Governance/DocumentationStandards.md`

## Historical Delivery Evidence

The numbered `S0.x` through `S3.x` documents preserve the execution evidence associated with the former mechanical baseline. `CR-001` provides the active transition authority. The R1 requirements, traceability matrix, and critical rules provide the active successors for former Stage 1 mechanical artifacts.

## Publishing

GitHub Pages publishes the complete `Docs/` tree through a custom GitHub Actions workflow. The published navigation follows this hub and presents active documents before historical evidence.
