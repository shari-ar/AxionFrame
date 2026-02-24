# AxionFrame Engine — Project Workflow (SolidWorks 2026)

This document describes the end-to-end workflow for developing, running, and maintaining the AxionFrame Engine as a SolidWorks 2026 add-in that programmatically generates a modular, height-adjustable X-frame table (non-welded, bolted joints) and exports manufacturing deliverables.

---

## 1) Roles of Each Top-Level Folder (Operational View)

- **API/**: All add-in source code, supporting utilities, and test projects.
- **Config/**: External parameter sets (e.g., heights 900/1100, profile dimensions, hardware sizing).
- **CAD/**: Canonical CAD sources (templates, library parts, optional seed models) used as inputs or references.
- **Library/**: Shared reusable resources (weldment profiles, standard hardware, CNC plate templates).
- **Output/**: Generated outputs for manufacturing and review (STEP/DXF/PDF/BOM/Reports).
- **Docs/**: Requirements, standards, revision history, and engineering notes.

---

## 2) High-Level Lifecycle

1. **Define requirements** → dimensions, height options, joinery approach, manufacturing constraints.
2. **Define parameter schema** → JSON configuration files and validation rules.
3. **Implement add-in engines** → geometry, features, mates, exports.
4. **Generate CAD** → parts and assemblies via SolidWorks API.
5. **Validate** → geometry rules, interference, stability, (optional) simulation.
6. **Export** → production deliverables.
7. **Release** → versioned outputs and documentation.

---

## 3) Development Workflow (How You Build the System)

### 3.1 Configure the Add-in Project

1. Open the Visual Studio solution under **API/Addin/**.
2. Confirm SolidWorks 2026 references:
   - Interop: `SolidWorks.Interop.sldworks`, `SolidWorks.Interop.swconst`
   - SolidWorks Tools (if used): `SolidWorksTools`
3. Set platform target and registration strategy (e.g., x64 if SolidWorks is x64).
4. Add a consistent logging mechanism early (file + debug output).

### 3.2 Establish Coding Contracts

1. Define naming conventions for:
   - Files and configurations
   - Feature names (sketch/feature IDs)
   - Planes, axes, mate references
2. Define stable “public IDs” for generated entities:
   - Example: `SK_LAYOUT_FRONT`, `FT_WELDMENT_FRAME`, `MATE_PIVOT_HINGE`
3. Implement a single source of truth for parameters:
   - `ConfigLoader` → strongly typed `DesignParameters` object
   - Validation + defaults

### 3.3 Implement Engines (Core Layer)

Build the add-in in layers:

- **SolidWorks session layer**
  - Connect to `ISldWorks`, active doc, document creation
  - Open/close documents safely

- **Geometry/layout engine**
  - Creates reference planes/axes
  - Creates master layout sketches
  - Computes derived values (angles, offsets) from target heights

- **Feature engine**
  - Builds structural members / bodies
  - Builds plates, holes, cuts, fillets, chamfers
  - Applies materials and custom properties

- **Assembly engine**
  - Inserts parts
  - Applies mates
  - Builds configurations (900/1100)
  - Suppresses/unsuppresses features or mates per configuration

- **Export engine**
  - STEP export for parts/assemblies
  - DXF export for plate parts
  - Drawing export to PDF (if drawings are generated)
  - BOM generation (CSV/XLSX/PDF depending on pipeline)

- **Validation engine**
  - Interference checks
  - Missing reference checks
  - Config sanity checks
  - Dimensional checks

---

## 4) Runtime Workflow (What Happens When the User Clicks “Generate”)

This is the operational sequence your add-in should follow.

### 4.1 Start: Launch Command

1. User triggers **Generate Table** from toolbar / task pane.
2. Add-in initializes:
   - Logger
   - Environment checks (SolidWorks version, paths, permissions)

### 4.2 Load Parameters

1. Read **Config/GlobalParams.json**.
2. Select configuration set:
   - `Height900` or `Height1100` or “Generate both”.
3. Validate parameters:
   - Range checks (e.g., thickness > 0)
   - Required fields present
   - Folder paths exist or are created

### 4.3 Prepare Workspace

1. Create a unique run folder inside **Output/** (timestamped / revisioned):
   - `Output/Run_YYYYMMDD_HHMM/…`
2. Copy the effective configuration snapshot into the run folder:
   - `effective_params.json`

### 4.4 Generate Parts

For each required part:

1. Create new part document using **CAD/Templates/Part_Template**.
2. Create reference geometry:
   - Planes/axes used for stable feature placement
3. Build geometry:
   - Frame members (based on parametric layout)
   - Pivot plates / blocks
   - Tie beam / braces
   - Mounting plates (top/bottom)
4. Create holes and interfaces:
   - Bolt holes, pivot hole(s), height indexing holes
5. Apply metadata:
   - Material
   - Custom properties (PartNumber, Description, Revision, Finish)
6. Save into **CAD/Parts/** (canonical) and/or **Output/Generated_Parts/** (run artifacts)

### 4.5 Generate Assemblies

1. Create new assembly using **CAD/Templates/Assembly_Template**.
2. Insert generated parts.
3. Apply mates:
   - Pivot/hinge mates
   - Coincident/parallel mates for plates
   - Distance mates for controlled positioning
4. Create configurations:
   - `TABLE_900`
   - `TABLE_1100`
5. Per configuration:
   - Activate correct height indexing mate set
   - Suppress unused mates/features
6. Save into **CAD/Assemblies/** and/or **Output/Generated_Assemblies/**.

### 4.6 Validate

1. Run interference detection.
2. Run reference validation:
   - Missing faces/edges used for mates
   - Suppressed features unexpectedly
3. Optional: mass properties report.
4. Write a validation summary into **Output/Reports/**.

### 4.7 Export Manufacturing Deliverables

1. Export STEP:
   - Parts and/or assembly into **Output/STEP/**
2. Export DXF for plates:
   - Flat patterns or face exports into **Output/DXF/**
3. Export drawings to PDF (if drawings exist or are generated):
   - **Output/Drawings_PDF/**
4. Export BOM:
   - CSV/XLSX into **Output/BOM/**
5. Copy log file into the run folder.

### 4.8 Finish

1. Display a results summary:
   - Files generated
   - Validation status
   - Any warnings
2. Provide quick-open links to the run folder.

---

## 5) Configuration Workflow (How Heights Are Managed)

### Option A — Single Assembly with Configurations (Recommended)

- Create one assembly with multiple configurations.
- Height is controlled by:
  - Alternate mates
  - Alternate hole alignment
  - Suppression states

### Option B — Two Separate Assemblies

- Generate `ASM_Table_H900` and `ASM_Table_H1100` separately.
- Easier exports, heavier file duplication.

The recommended approach is **Option A** unless manufacturing requires separate assemblies.

---

## 6) Revision & Release Workflow

1. Increment revision in:
   - Config (or a dedicated `Version.json`)
   - Part/assembly custom properties
2. Generate outputs into a versioned run folder.
3. Freeze outputs:
   - Copy finalized STEP/DXF/PDF/BOM into **Output/Release/Rev_X/**
4. Update **Docs/RevisionHistory/** with:
   - What changed
   - Why
   - Compatibility notes

---

## 7) Testing Workflow

1. Unit tests for:
   - Parameter parsing and validation
   - Naming conventions
   - Path management
2. Integration tests for:
   - End-to-end generation in a controlled environment
   - Export correctness
3. Regression checks:
   - Compare mass properties
   - Detect mate failures
   - Confirm DXF layer/scale rules

---

## 8) Operational Best Practices

- Keep feature and mate naming deterministic.
- Avoid selecting by “random face”; prefer named reference geometry.
- Always store the effective configuration used in each output run.
- Log every document create/open/save/export call.
- Fail fast on missing dependencies (templates, profiles, libraries).

---

## 9) Canonical Execution Flow (Reference)

1. Initialize add-in
2. Load + validate config
3. Create run output folder
4. Generate parts
5. Generate assembly + configurations
6. Validate (interference + integrity)
7. Export deliverables
8. Write reports + logs
9. Present summary

---

## Appendix — Suggested Deliverables per Run

- `effective_params.json`
- `GenerationLog.txt`
- Assembly file(s)
- Parts file(s)
- STEP export(s)
- DXF plate export(s)
- BOM (CSV/XLSX)
- Validation report (TXT/PDF)

