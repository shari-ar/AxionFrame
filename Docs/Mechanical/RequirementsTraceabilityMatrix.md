# Requirements Traceability Matrix

- **Title:** Requirements Traceability Matrix
- **Status:** Active
- **Owner:** Joint Mechanical, Software, and Verification Engineering
- **Scope:** Bidirectional traceability from approved decisions through requirements, engineering proof, implementation, and verification evidence.

## Verification Method Codes

| Code | Method |
|---|---|
| `A` | Engineering analysis or hand calculation |
| `F` | Finite-element analysis |
| `I` | Drawing, CAD, configuration, or physical inspection |
| `T` | Physical test or timed demonstration |
| `S` | Automated software test |
| `C` | Certificate, material record, or supplier qualification review |

## Product and Configuration Traceability

| Requirement | Decision source | Verification | Primary evidence | Delivery stage |
|---|---|---|---|---|
| AXF-PRD-001 | DR-005 | I | End-frame topology inspection record | R2, R4, R5 |
| AXF-PRD-002 | DR-006 | I, S | Reference-point inventory and topology test | R3, R4 |
| AXF-PRD-003 | DR-005, DR-006 | I, S | Member-connectivity matrix and generated-model inspection | R3, R4, R5 |
| AXF-PRD-004 | DR-003 | I, S | Configuration contract test and height measurement | R3, R5 |
| AXF-PRD-005 | DR-011 | I, S | Envelope measurement and configuration test | R3, R5 |
| AXF-PRD-006 | DR-011 | I, T | Interface inventory and disassembly demonstration | R2, R5 |

## Structural Traceability

| Requirement | Decision source | Verification | Primary evidence | Delivery stage |
|---|---|---|---|---|
| AXF-STR-001 | DR-002 | A, F, T | Load-case specification, structural report, proof-load record | R2, R5 |
| AXF-STR-002 | DR-002 | A, F, T | Concentrated-load report and proof-load record | R2, R5 |
| AXF-STR-003 | DR-010 | A, F, T | Edge-load report and physical loading record | R2, R5 |
| AXF-STR-004 | DR-012 | A, F | Strength utilization table | R2 |
| AXF-STR-005 | DR-012 | T | Vertical proof-load report | R2, R5 |
| AXF-STR-006 | DR-012 | A, F, T | Distributed-load displacement results | R2, R5 |
| AXF-STR-007 | DR-012 | A, F, T | Concentrated-load displacement results | R2, R5 |
| AXF-STR-008 | DR-012 | A, F, T | Lateral stiffness report | R2, R5 |
| AXF-STR-009 | DR-011 | A, T | Overturning calculation and stability test | R2, R5 |
| AXF-STR-010 | DR-012 | A, F, T | Torsional stiffness report | R2, R5 |
| AXF-STR-011 | DR-012 | A, F | Buckling calculation and eigenvalue report | R2 |

## Node-Joint Traceability

| Requirement | Decision source | Verification | Primary evidence | Delivery stage |
|---|---|---|---|---|
| AXF-JNT-001 | DR-007 | I, S | Joint-section drawing, tolerance analysis, geometry test | R2, R3, R4 |
| AXF-JNT-002 | DR-008 | I, S | Joint BOM and generated-component inventory | R3, R4, R5 |
| AXF-JNT-003 | DR-009 | A, F, T | Local-wall analysis and preload test | R2 |
| AXF-JNT-004 | DR-007, DR-008 | A, T | Joint slip calculation and static slip test | R2 |
| AXF-JNT-005 | DR-007, DR-008 | T | Representative joint cyclic-test report | R2 |
| AXF-JNT-006 | DR-011 | T, I | Assembly-cycle endurance report | R2, R5 |
| AXF-JNT-007 | DR-008 | C, I | Fastener certificate and BOM inspection | R2, R6 |
| AXF-JNT-008 | DR-008 | C, I | Procurement qualification record and joint BOM | R2, R6 |

## Work-Surface and Clamping Traceability

| Requirement | Decision source | Verification | Primary evidence | Delivery stage |
|---|---|---|---|---|
| AXF-WRK-001 | DR-010 | I, S | Perforation drawing and generated-hole inspection | R2, R4, R5 |
| AXF-WRK-002 | DR-010 | F, T | Surface-flatness analysis and measurement map | R2, R5 |
| AXF-WRK-003 | DR-010 | I, T | Clamp-envelope model and four-side access demonstration | R2, R5 |
| AXF-WRK-004 | DR-010 | A, F, T | Clamp-load report and permanent-set measurement | R2, R5 |
| AXF-WRK-005 | DR-010, DR-011 | I, T | Replacement procedure and datum-repeatability record | R2, R5 |

## Portability and Human-Factor Traceability

| Requirement | Decision source | Verification | Primary evidence | Delivery stage |
|---|---|---|---|---|
| AXF-PRT-001 | DR-011 | A, I | SolidWorks mass-properties report and BOM reconciliation | R2, R5, R6 |
| AXF-PRT-002 | DR-011 | A, I | Component mass ranking and transport plan | R2, R6 |
| AXF-PRT-003 | DR-011 | T | Timed assembly demonstration | R5 |
| AXF-PRT-004 | DR-011 | T | Timed disassembly demonstration | R5 |
| AXF-PRT-005 | DR-011 | C, T | Foot-material certificate and sliding test | R2, R5 |

## Manufacturing and Finish Traceability

| Requirement | Decision source | Verification | Primary evidence | Delivery stage |
|---|---|---|---|---|
| AXF-MFG-001 | DR-012 | C, I | Material certificate and BOM material record | R2, R6 |
| AXF-MFG-002 | DR-012 | I | Profile drawing and first-article inspection | R2, R6 |
| AXF-MFG-003 | DR-007 | I | Plate drawing, tolerance stack, and first-article inspection | R2, R6 |
| AXF-MFG-004 | DR-012 | I | Edge-quality inspection record | R6 |
| AXF-MFG-005 | DR-002 | C, I | Coating-system qualification and finish-sample approval | R2, R6 |
| AXF-MFG-006 | DR-008 | I | Visible-hardware schedule and appearance inspection | R2, R5, R6 |
| AXF-MFG-007 | DR-002 | A, T | Bonding design record and resistance measurement | R2, R5 |

## CAD and Delivery Traceability

| Requirement | Decision source | Verification | Primary evidence | Delivery stage |
|---|---|---|---|---|
| AXF-CAD-001 | DR-017 | S, I | Sketch-state audit | R4, R5 |
| AXF-CAD-002 | DR-017 | S, I | Forced-rebuild report | R4, R5 |
| AXF-CAD-003 | DR-017 | S | Three-run deterministic regression report | R3, R4 |
| AXF-CAD-004 | DR-019 | S, I | Component-property coverage report | R4, R6 |
| AXF-CAD-005 | DR-014, DR-015, DR-019 | S, I | Build Verification Gate record and Final Output manifest | R5, R6, R7 |

## Coverage Summary

| Coverage item | Required result |
|---|---:|
| Requirements with decision sources | 100% |
| Requirements with verification methods | 100% |
| Requirements with named evidence | 100% |
| Requirements with delivery-stage ownership | 100% |
| Orphan requirement count | 0 |
| Orphan evidence count | 0 |

## Change Control

Every requirement change updates this matrix, `ProductRequirementsSpecification.md`, `CriticalEngineeringRules.md`, affected engineering proof, affected tests, and the Decision Register in one pull request.

## Related Documents

- `ProductRequirementsSpecification.md`
- `CriticalEngineeringRules.md`
- `DesignAndManufacturing.md`
- `../Governance/DecisionRegister.md`
- `../Workflow/R1-RequirementsBaselineApproval.md`
- `../../roadmap.md`
