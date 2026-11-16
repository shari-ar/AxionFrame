# Critical Engineering Rules

- **Title:** Critical Engineering Rules
- **Status:** Active
- **Owner:** Joint Mechanical and Software Engineering
- **Scope:** Mandatory engineering rules that control configuration validation, CAD generation, Build verification, Final Output authorization, and release evidence.

## Enforcement Levels

| Level | Meaning | System response |
|---|---|---|
| `E1` | Configuration-critical | Input Validation returns `Blocked` |
| `E2` | Geometry-critical | Build returns `Failed` |
| `E3` | Verification-critical | Build Verification Gate returns `Failed` |
| `E4` | Output-critical | Final Output authorization returns `Blocked` |

## Rule Index

| Rule ID | Source requirement | Level | Binding rule | Pass condition |
|---|---|---|---|---|
| AXF-RULE-001 | AXF-PRD-001 through AXF-PRD-003 | E2 | Every end frame uses the approved A-B-C-D-N1-N2 topology. | Point, member, and connectivity match `100%` |
| AXF-RULE-002 | AXF-PRD-004 | E1 | `Height` defines one fixed generated operating height. | Height deviation `≤1.0 mm`; mechanical state count `1` |
| AXF-RULE-003 | AXF-PRD-005 | E1 | `Length` and `Width` define the constant operating footprint. | Footprint deviation `≤2.0 mm`; extension-state count `0` |
| AXF-RULE-004 | AXF-PRD-006 | E2 | Every primary product interface supports the approved complete-disassembly architecture. | Permanent inter-component assembly-joint count `0` |
| AXF-RULE-005 | AXF-STR-001 through AXF-STR-003 | E3 | The rated configuration carries every defined vertical service load. | Load-case pass coverage `100%` |
| AXF-RULE-006 | AXF-STR-004 | E3 | Every service load case maintains the minimum yield safety factor. | Minimum yield safety factor `≥1.50` |
| AXF-RULE-007 | AXF-STR-005 | E3 | Vertical proof loading preserves structural integrity and geometry. | Residual displacement `≤0.50 mm`; crack count `0` |
| AXF-RULE-008 | AXF-STR-006, AXF-STR-007 | E3 | Work-surface displacement remains inside the approved limits. | Distributed displacement `≤2.0 mm`; concentrated displacement `≤3.0 mm` |
| AXF-RULE-009 | AXF-STR-008 through AXF-STR-011 | E3 | Lateral, overturning, torsional, and buckling performance meet the approved criteria. | Required-result coverage `100%` |
| AXF-RULE-010 | AXF-JNT-001 | E2 | Each connected RHS end captures one node plate through a centered axial slot. | Center deviation `≤0.25 mm`; slot symmetry deviation `≤0.25 mm` |
| AXF-RULE-011 | AXF-JNT-002 | E2 | Each member end uses two fasteners and two internal spacers. | Fastener count `2`; spacer count `2` |
| AXF-RULE-012 | AXF-JNT-003 | E3 | Local reinforcement transfers installation preload through the joint stack. | RHS wall permanent set `≤0.10 mm` |
| AXF-RULE-013 | AXF-JNT-004, AXF-JNT-005 | E3 | Static and cyclic joint slip remain inside the approved limits. | Static slip `≤0.10 mm`; cyclic slip `≤0.20 mm`; preload loss `≤15%` |
| AXF-RULE-014 | AXF-JNT-006 | E3 | The joint preserves fit and serviceability through repeated assembly. | Assembly cycles `100`; all endurance criteria pass |
| AXF-RULE-015 | AXF-JNT-007, AXF-JNT-008 | E1 | Fastener hardware uses the approved property class and retention sequence. | Property class `≥8.8`; qualification record present |
| AXF-RULE-016 | AXF-WRK-001, AXF-WRK-002 | E2 | Perforation accuracy and work-surface flatness meet the approved criteria. | Hole and flatness criteria pass |
| AXF-RULE-017 | AXF-WRK-003, AXF-WRK-004 | E3 | Every side supports the approved clamp envelope and clamp action. | Perimeter coverage `100%`; two-location load test passes |
| AXF-RULE-018 | AXF-PRT-001, AXF-PRT-002 | E3 | Product and component masses remain inside the transport targets. | Total mass `≤90 kg`; component mass `≤25 kg` |
| AXF-RULE-019 | AXF-MFG-001 through AXF-MFG-004 | E3 | Material, cutting, plate, hole, slot, edge, and flatness records meet manufacturing criteria. | Manufacturing acceptance coverage `100%` |
| AXF-RULE-020 | AXF-MFG-007 | E3 | The metal-fabrication grounding path meets the bonding target. | Bonding resistance `≤0.10 Ω` |
| AXF-RULE-021 | AXF-CAD-001 | E2 | Every generated sketch reaches a fully defined state. | Under-defined count `0`; over-defined count `0` |
| AXF-RULE-022 | AXF-CAD-002 | E2 | Every generated document completes a forced rebuild. | Rebuild error count `0`; rebuild warning count `0` |
| AXF-RULE-023 | AXF-CAD-003 | E2 | Three identical validated inputs produce deterministic models. | Identity deviation count `0`; geometry deviation `≤0.01 mm` |
| AXF-RULE-024 | AXF-CAD-004 | E4 | Every component carries the required manufacturing identity. | Property coverage `100%` |
| AXF-RULE-025 | AXF-CAD-005 | E4 | Final Output uses one approved Build identity. | Identity and evidence coverage `100%` |

## Rule Implementation Contract

Each implemented rule carries:

- Stable rule ID
- Source requirement ID
- Configuration inputs
- Unit system
- Evaluation stage
- Deterministic evaluation logic
- Pass result payload
- Failure result payload
- Evidence location
- Automated test coverage
- SolidWorks verification case where applicable

## Gate Acceptance

R1 rule coverage reaches approval when every critical product requirement maps to one enforcement level and one deterministic pass condition.

## Related Documents

- `ProductRequirementsSpecification.md`
- `RequirementsTraceabilityMatrix.md`
- `../Software/ValidationAndErrorHandlingSpecification.md`
- `../Verification/SolidWorksVerificationPolicy.md`
- `../Workflow/R1-RequirementsBaselineApproval.md`
