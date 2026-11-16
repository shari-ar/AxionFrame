# Product Requirements Specification

- **Title:** Product Requirements Specification
- **Status:** Active
- **Owner:** Joint Product and Mechanical Engineering
- **Scope:** Measurable product requirements for the selected AxionFrame workbench concept.

## Product Class

AxionFrame delivers a professional general-fabrication workbench for woodworking, steel fabrication, drilling, grinding, clamping, table construction, cabinet construction, and assembly work. The rated operating environment is an indoor workshop between `5 °C` and `40 °C` with relative humidity between `20%` and `85%`.

## Rated Baseline Configuration

| Parameter | Rated value | Unit |
|---|---:|---|
| `Height` | 1020 | mm |
| `Width` | 750 | mm |
| `Length` | 1500 | mm |
| `Top-Plate-Thk` | 2 | mm |
| `RHS-W` | 60 | mm |
| `RHS-H` | 40 | mm |
| `RHS-Thk` | 2 | mm |
| `Polygonal-Plate-Thk` | 10 | mm |

The rated performance values in this specification apply to this configuration. R2 establishes the validated parameter envelope for configurable variants.

## Rated-Performance Basis

| Target | Engineering basis |
|---|---|
| `5.0 kN` distributed service load | Approximately `510 kg` of total static loading for cabinet, table, fixture, and material assembly work |
| `2.0 kN` concentrated service load | Approximately `204 kg` of localized tooling, workpiece, and operator-applied loading |
| `1.0 kN` edge service load | Approximately `102 kg` of localized edge-supported workpiece loading |
| `1.50` proof factor | Verification margin above the published service rating |
| `0.50 kN` horizontal structural load | High manual fabrication action applied at work-surface height |
| `150 N` overturning action | Empty-table free-standing stability target with a `1.50` resisting-moment ratio |
| `5.0 kN` clamp action | Heavy manual clamp action at two simultaneous edge locations |
| `90 kg` total mass and `25 kg` component mass | Robust steel construction with transportable disassembled components |

## Requirement Language

`Shall` identifies a binding product requirement. Each requirement has one verification method and one acceptance result in `RequirementsTraceabilityMatrix.md`.

## Product and Configuration Requirements

| ID | Requirement | Acceptance criterion |
|---|---|---|
| AXF-PRD-001 | The workbench shall use two identical transverse X end frames connected by longitudinal structure. | End-frame count `2`; topology match `100%` |
| AXF-PRD-002 | Each end frame shall use reference points `A`, `B`, `C`, `D`, `N1`, and `N2`. | Required point count `6`; duplicate internal-node count `0` |
| AXF-PRD-003 | Each end frame shall use members `A-B`, `C-D`, `B-N1`, `N1-N2`, `N2-C`, `A-N1`, and `N2-D`. | Required member count `7`; topology deviation count `0` |
| AXF-PRD-004 | Each generated assembly shall have one fixed operating height defined by the `Height` input. | Generated height deviation `±1.0 mm`; mechanical height state count `1` |
| AXF-PRD-005 | The operating footprint shall equal the generated `Length` and `Width`. | Extension-state count `0`; operating footprint deviation `±2.0 mm` |
| AXF-PRD-006 | Portability shall use complete disassembly into transportable components. | Permanent inter-component assembly-joint count `0`; required disassembly-interface coverage `100%` |

## Structural Requirements

| ID | Requirement | Acceptance criterion |
|---|---|---|
| AXF-STR-001 | The workbench shall carry a uniformly distributed vertical service load across the work surface. | Service load `5.0 kN` |
| AXF-STR-002 | The workbench shall carry a concentrated vertical service load at every work-surface location inside the support boundary. | Service load `2.0 kN` over a `100 mm × 100 mm` loading pad |
| AXF-STR-003 | The workbench shall carry a vertical service load at every clamping edge. | Service load `1.0 kN` over a `100 mm` edge segment |
| AXF-STR-004 | Structural members and connections shall achieve the required service strength. | Minimum yield safety factor `1.50` for every service load case |
| AXF-STR-005 | The structure shall pass vertical proof loading. | Proof factor `1.50`; residual top displacement `≤0.50 mm`; crack count `0` |
| AXF-STR-006 | The work surface shall maintain global stiffness under distributed service load. | Maximum vertical displacement `≤2.0 mm` |
| AXF-STR-007 | The work surface shall maintain local stiffness under concentrated service load. | Maximum vertical displacement `≤3.0 mm` |
| AXF-STR-008 | The frame shall resist a horizontal structural service load applied at work-surface height with floor contacts constrained. | Horizontal load `0.50 kN`; top displacement `≤5.0 mm`; yield safety factor `≥1.50` |
| AXF-STR-009 | The free-standing workbench shall maintain overturning stability in its empty operating condition. | Horizontal design action `150 N` at work-surface height; resisting-moment ratio `≥1.50` |
| AXF-STR-010 | The frame shall maintain torsional stiffness under opposing corner actions. | Opposing horizontal actions `300 N`; relative corner displacement `≤3.0 mm` |
| AXF-STR-011 | Compression members shall maintain elastic buckling capacity. | Minimum linear buckling load factor `2.00` for every service load case |

## Node-Joint Requirements

| ID | Requirement | Acceptance criterion |
|---|---|---|
| AXF-JNT-001 | Each RHS member end shall capture its polygonal node plate through one centered axial slot. | Plate centered within `±0.25 mm`; slot symmetry deviation `≤0.25 mm` |
| AXF-JNT-002 | Each member-to-plate interface shall use two fasteners and two internal spacer elements. | Fastener count `2`; spacer count `2` per member end |
| AXF-JNT-003 | Each bolted region shall use local reinforcement that transfers clamp load through the connection stack. | RHS wall permanent set `≤0.10 mm` at installation preload |
| AXF-JNT-004 | Each connection shall maintain position under service shear. | Relative slip `≤0.10 mm` at calculated peak service member force |
| AXF-JNT-005 | A representative connection shall maintain cyclic stability. | `10,000` fully reversed cycles at `60%` of calculated peak service member force; accumulated slip `≤0.20 mm`; preload loss `≤15%`; crack count `0` |
| AXF-JNT-006 | Connection hardware shall support repeated assembly. | `100` assembly cycles; hole-growth value `≤0.20 mm`; spacer permanent-set value `≤0.10 mm`; thread rejection count `0` |
| AXF-JNT-007 | Carbon-steel bolts, screws, and studs shall use a defined mechanical property class. | Minimum property class `8.8` in accordance with ISO 898-1:2013 |
| AXF-JNT-008 | Fastener retention shall follow the approved procurement rule. | Qualified integrated threaded solution selected first; qualified locknut assembly selected second |

## Work-Surface and Clamping Requirements

| ID | Requirement | Acceptance criterion |
|---|---|---|
| AXF-WRK-001 | The work surface shall use a repeatable perforation pattern referenced from assembly datums. | Hole-position deviation `≤0.25 mm`; hole-diameter deviation `≤0.10 mm` |
| AXF-WRK-002 | The unloaded work surface shall meet the required global flatness. | Total flatness deviation `≤1.50 mm` across the rated `1500 mm × 750 mm` surface |
| AXF-WRK-003 | All four work-surface sides shall provide continuous clamp access. | Accessible perimeter coverage `100%`; underside clear depth `≥75 mm`; inward clear reach `≥50 mm` |
| AXF-WRK-004 | Each clamping edge shall carry two simultaneous clamp actions. | Clamp action `5.0 kN` at each of two locations; local permanent set `≤0.50 mm` |
| AXF-WRK-005 | The work surface shall remain a replaceable transport component. | Detachable interface coverage `100%`; replacement preserves datum repeatability within `±0.50 mm` |

## Portability and Human-Factor Requirements

| ID | Requirement | Acceptance criterion |
|---|---|---|
| AXF-PRT-001 | The complete rated workbench shall meet the transport mass target. | Total calculated mass `≤90 kg` |
| AXF-PRT-002 | Every transport component shall meet the manual-lift target. | Maximum individual component mass `≤25 kg` |
| AXF-PRT-003 | Two trained adults shall assemble the rated workbench using common hand tools. | Assembly time `≤45 min`; required tool-size count `≤2` |
| AXF-PRT-004 | Two trained adults shall disassemble the rated workbench using the same tool set. | Disassembly time `≤30 min`; damaged-component count `0` |
| AXF-PRT-005 | Floor interfaces shall provide stable stationary contact on dry level concrete. | Foot-contact count `4`; static friction coefficient `≥0.60` |

## Manufacturing and Finish Requirements

| ID | Requirement | Acceptance criterion |
|---|---|---|
| AXF-MFG-001 | Structural carbon-steel material shall provide a traceable minimum yield strength. | Minimum specified yield strength `235 MPa` |
| AXF-MFG-002 | Profile cutting shall support deterministic assembly geometry. | Cut-length deviation `±0.50 mm`; cut-angle deviation `±0.25°` |
| AXF-MFG-003 | Plate, hole, and slot manufacturing shall support the joint tolerance stack. | Profile deviation `±0.25 mm`; hole-position deviation `±0.20 mm`; node-plate flatness `≤0.50 mm` |
| AXF-MFG-004 | Exposed cut edges shall support safe handling and coating quality. | Burr height `≤0.10 mm`; edge break `0.5 mm` to `1.0 mm` |
| AXF-MFG-005 | Coated structural components shall support indoor workshop corrosion protection and visual consistency. | ISO 12944 corrosivity category `C2`; dry-film thickness and coating system defined by the qualified supplier; visible color and gloss match the approved finish sample |
| AXF-MFG-006 | Visible fasteners shall express one consistent industrial design language. | Head-family count `1` per visible interface class; finish-family count `1`; outward-facing installation coverage `100%` |
| AXF-MFG-007 | The frame and work surface shall provide a dedicated electrical bonding point for metal-fabrication grounding. | Bonding resistance from work surface to marked grounding point `≤0.10 Ω` |

## CAD and Delivery Requirements

| ID | Requirement | Acceptance criterion |
|---|---|---|
| AXF-CAD-001 | Every generated sketch shall carry a fully defined SolidWorks state. | Under-defined sketch count `0`; over-defined sketch count `0` |
| AXF-CAD-002 | Every generated part and assembly shall complete a forced rebuild. | Rebuild error count `0`; rebuild warning count `0` |
| AXF-CAD-003 | Identical validated inputs shall create identical model identities and geometry. | Naming deviation count `0`; configuration-hash deviation count `0`; measured-geometry deviation `≤0.01 mm` across three builds |
| AXF-CAD-004 | Every generated component shall carry manufacturing identity. | Material, part number, revision, description, quantity, and mass-property coverage `100%` |
| AXF-CAD-005 | Final Output shall consume one approved Build identity. | Commit SHA, configuration hash, Build ID, verification result, manifest, and checksum coverage `100%` |

## Standards Basis

| Reference | Application |
|---|---|
| [ISO 12100:2010](https://www.iso.org/standard/51528.html) | Risk assessment and risk-reduction structure |
| [ISO 898-1:2013](https://www.iso.org/standard/60610.html) | Mechanical property classes for carbon-steel and alloy-steel fasteners |
| [ISO 12944-5:2019](https://www.iso.org/standard/77795.html) | Indoor steel corrosion category and coating-system selection |
| SolidWorks 2026 documentation | CAD feature, assembly, rebuild, API, and verification behavior |

## Gate Acceptance

R1 reaches approval when every requirement has a unique ID, measurable acceptance criterion, verification method, evidence owner, and roadmap stage mapping. The R1 gate record provides formal approval evidence.

## Related Documents

- `RequirementsTraceabilityMatrix.md`
- `CriticalEngineeringRules.md`
- `DesignAndManufacturing.md`
- `../Workflow/R1-RequirementsBaselineApproval.md`
- `../Roadmap.md`
