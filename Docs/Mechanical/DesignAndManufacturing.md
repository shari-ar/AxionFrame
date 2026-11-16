# Mechanical Design and Manufacturing Guide

- **Title:** Mechanical Design and Manufacturing Guide
- **Status:** Active Design Intent
- **Owner:** Mechanical Engineering
- **Scope:** Selected product concept, baseline parameters, frame topology, joint architecture, work-surface intent, portability, engineering proof, and manufacturing deliverables.

## Product Mission

AxionFrame generates a professional workbench for fabrication and assembly activities involving wood, steel, drilling, grinding, clamping, table construction, cabinet construction, and similarly scaled workpieces.

Each generated workbench has a fixed operating height. The configuration controls the generated height before Build execution.

## Selected Concept

The product uses one approved X-frame concept. Engineering development matures this concept through requirements, feasibility, analysis, verification, and baseline approval.

## Baseline Configuration Parameters

| Parameter | Default | Unit | Meaning |
|---|---:|---|---|
| `Height` | 1020 | mm | Finished generated workbench height |
| `Width` | 750 | mm | Workbench transverse width |
| `Length` | 1500 | mm | Workbench longitudinal length |
| `Top-Plate-Thk` | 2 | mm | Perforated work-surface plate thickness |
| `RHS-W` | 60 | mm | RHS section width |
| `RHS-H` | 40 | mm | RHS section height |
| `RHS-Thk` | 2 | mm | RHS wall thickness |
| `Polygonal-Plate-Thk` | 10 | mm | Polygonal node-plate thickness |

The configuration schema supports these parameters as editable values. Controlled engineering changes can introduce additional parameters through the roadmap contract process.

## End-Frame Topology

The assembly uses two identical transverse end frames, one at each longitudinal end of the workbench.

Each end frame uses these reference points:

- `A`: upper-left corner
- `B`: upper-right corner
- `C`: lower-left corner
- `D`: lower-right corner
- `N1`: first internal node along the primary diagonal
- `N2`: second internal node along the primary diagonal

The member topology uses:

- Upper horizontal member `A-B`
- Lower horizontal member `C-D`
- Primary diagonal sequence `B-N1-N2-C`, formed by three separate RHS members
- Opposing upper half-diagonal `A-N1`
- Opposing lower half-diagonal `N2-D`

This topology creates two distinct internal nodes and a visually identifiable broken-X architecture.

## Node-Joint Architecture

- Each node uses a polygonal steel plate.
- Each connected RHS member receives a centered axial slot at its end.
- The node plate enters the member slot and remains centered between the opposing RHS walls.
- Two internal spacer elements occupy the bolt stack on opposite sides of the node plate.
- Two fasteners secure each member end to the node plate.
- Local reinforcement protects the connection region from wall crushing, bearing deformation, and repeated assembly loads.
- The remaining member length uses the configured RHS wall thickness.
- Exposed fastener heads contribute to the industrial visual identity.

The engineering proof stage defines slot depth, fastener diameter, fastener grade, hole diameter, edge distance, spacer geometry, preload method, contact condition, and local reinforcement geometry.

## Fastener Retention Sequence

Procurement evaluates a commercially available integrated threaded solution as the primary retention choice. Qualification covers structural capacity, geometry, availability, cost, serviceability, repeatable assembly, and visual integration. The qualified secondary choice uses a locknut assembly.

## Work Surface

- The work surface uses a perforated steel plate.
- The perimeter provides continuous clamp access on all four sides.
- The support structure maintains flatness under approved load cases.
- The perimeter geometry supports common fabrication and cabinet-assembly clamps.
- The engineering proof stage defines perforation diameter, pitch, edge distance, support spacing, perimeter clearance, and flatness tolerance.

## Portability and Operating Form

- The complete workbench separates into transportable components.
- Bolted interfaces support repeated assembly and disassembly.
- The operating footprint remains constant.
- Stationary floor contact defines the operating condition.
- Component identification and deterministic assembly references support repeatable setup.

## Engineering Proof Deliverables

Mechanical Baseline approval requires:

- Defined operational load cases
- Distributed and concentrated load targets
- Lateral and torsional stability targets
- Deflection and flatness criteria
- Joint slip and bearing criteria
- Fastener preload and retention criteria
- RHS local-wall verification
- Longitudinal-structure definition
- Work-surface support definition
- Assembly and disassembly sequence
- Mass and maximum component-lift values
- Tolerance stack analysis
- SolidWorks Simulation evidence
- Representative joint verification evidence
- Manufacturing review

## Manufacturing Deliverables

Each approved Final Output run creates:

- SolidWorks assembly files
- SolidWorks part files
- STEP exports
- DXF exports for plate components
- Bill of Materials
- Cut list
- Fastener schedule
- Validation report
- Manifest
- SHA-256 checksums

## Traceability

Every mechanical rule maps to:

- Requirement ID
- Configuration key
- Deterministic SolidWorks feature or mate name
- Verification case
- Output artifact
- Validation report section

## Related Documents

- `ProductRequirementsSpecification.md`
- `RequirementsTraceabilityMatrix.md`
- `CriticalEngineeringRules.md`
- `../Governance/DecisionRegister.md`
- `../Workflow/CR-001-MechanicalRebaselineDecision.md`
- `../Workflow/ProjectLifecycle.md`
- `../Verification/SolidWorksVerificationPolicy.md`
- `../../roadmap.md`
