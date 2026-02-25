# AxionFrame Delivery Roadmap

- **Title:** AxionFrame Delivery Roadmap
- **Version:** 1
- **Scope:** This roadmap defines a dependency-validated, step-by-step execution plan for software developers and mechanical engineers to deliver the AxionFrame SolidWorks add-in workflow (**Build** + **Final Output**) and its manufacturing outputs.
- **Source Basis:** `Docs/Workflow/ProjectLifecycle.md`, `Docs/Software/Architecture.md`, `Docs/Software/DevelopmentGuide.md`, `Docs/Mechanical/DesignAndManufacturing.md`, and `Docs/Governance/DocumentationStandards.md`.

## 1) Execution Model and Standards

### Roles
- **Software (SW):** Add-in host, orchestration, validation, export, and test automation.
- **Mechanical (ME):** Design intent, dimensioning rules, manufacturability, and output traceability.
- **Joint (SW+ME):** Validation criteria, release gating, and documentation alignment.

### Progress Measurement Standard
Each step below must include:
1. **Observable artifact** (file, build output, CAD output, report, CI result, or release package).
2. **Verification method** (exact command, path, or UI check).
3. **Completion criterion** (pass/fail condition).

### Complexity Legend
- **H (High):** Multi-team dependency and/or high technical risk.
- **M (Medium):** Moderate implementation scope with bounded dependencies.
- **L (Low):** Documentation or low-risk implementation task.

---

## 2) Dependency-Validated Roadmap (Categorized, Step-by-Step)

## Phase A — Foundation and Requirements (Gate A)

### A1. Confirm Product and Lifecycle Baseline
- **Owner:** SW+ME
- **Complexity:** M
- **Goal:** Align team on lifecycle stages, build scope, and final-output scope.
- **Actions:**
  1. Review lifecycle and runtime flow for **Build** and **Final Output**.
  2. Confirm expected release deliverables (STEP, DXF, BOM, validation reports).
- **Output:** Approved baseline summary in project notes.
- **How to verify:**
  - Evidence: meeting note or decision log entry that explicitly lists lifecycle stages and outputs.
  - Completion: all stakeholders acknowledge the same output list and sequence.

### A2. Convert Design Intent into Implementable Requirement Set
- **Owner:** ME (primary), SW (support)
- **Complexity:** H
- **Goal:** Translate mechanical intent into implementable, testable requirements.
- **Actions (decomposed):**
  1. Define frame, pivot, height-indexing, and plate/brace requirement statements.
  2. Add measurable tolerances/dimensions and allowed configuration ranges.
  3. Map each rule to a configuration key and expected naming/report trace.
  4. Mark critical-to-manufacture rules requiring strict validation.
- **Output:** Requirement matrix (`Rule → Config Key → CAD Feature/Mate Name → Report Section`).
- **How to verify:**
  - Evidence: requirement matrix document committed.
  - Completion: 100% of listed mechanical rules have traceability fields populated.

### A3. Define Definition of Ready (DoR) Checklist per Feature
- **Owner:** SW
- **Complexity:** M
- **Goal:** Ensure no implementation begins without requirements, schema keys, and expected outputs.
- **Actions:**
  1. Create feature template with DoR checks.
  2. Attach required references to workflow/mechanical documents.
- **Output:** DoR checklist template used for all new tasks.
- **How to verify:**
  - Evidence: template present in planning workflow/tool.
  - Completion: active feature tickets include completed DoR sections.

**Gate A Exit Criteria:** A1–A3 completed with documented traceability and DoR adoption.

---

## Phase B — Architecture and Configuration Contracts (Gate B)

### B1. Confirm Layer Boundaries and Ownership
- **Owner:** SW
- **Complexity:** M
- **Goal:** Keep implementation aligned with add-in host/core/domain layer separation.
- **Actions:**
  1. Map components to layers (Host/Core/Modules/Shared/Tests).
  2. Assign code ownership by module.
- **Output:** Architecture ownership map.
- **How to verify:**
  - Evidence: ownership map in repo docs or team board.
  - Completion: every active component has exactly one owning team/person.

### B2. Implement/Update Configuration Schema and Validators
- **Owner:** SW
- **Complexity:** H
- **Goal:** Enforce strong contracts for parameter loading and validation.
- **Actions (decomposed):**
  1. Enumerate all required keys from A2 matrix.
  2. Define key types, ranges, defaults, and required/optional flags.
  3. Implement validation rules and deterministic error messages.
  4. Add unit tests for valid/invalid scenarios.
- **Output:** Updated config schema + validator tests.
- **How to verify:**
  - Evidence: test outputs showing pass on validator suite.
  - Completion: invalid configs fail with specific, documented errors; valid configs pass.

### B3. Standardize Deterministic Naming Rules
- **Owner:** SW+ME
- **Complexity:** M
- **Goal:** Guarantee reproducible feature/mate naming for traceability.
- **Actions:**
  1. Define naming grammar for features, mates, and references.
  2. Align naming with mechanical trace matrix.
- **Output:** Naming convention specification.
- **How to verify:**
  - Evidence: sample generated names documented and validated by both teams.
  - Completion: no ambiguous naming patterns remain.

**Gate B Exit Criteria:** Configuration, validation, and deterministic naming are documented and test-backed.

---

## Phase C — Build Command Delivery (Gate C)

### C1. Host Command and Session Orchestration
- **Owner:** SW
- **Complexity:** H
- **Goal:** Ensure Build is correctly wired from UI command to orchestration.
- **Actions (decomposed):**
  1. Validate add-in command registration and event wiring.
  2. Implement orchestration start/end lifecycle with run metadata.
  3. Log each major stage (load, validate, generate parts, generate assembly).
- **Output:** Executable Build command flow with run logs.
- **How to verify:**
  - Evidence: console/log output includes stage-by-stage status and run identifier.
  - Completion: Build command runs end-to-end without unhandled errors.

### C2. Domain Generation Modules (Frame, Pivot, Height Adjust)
- **Owner:** SW (primary), ME (review)
- **Complexity:** H
- **Goal:** Produce correct parametric CAD parts and assembly behavior.
- **Actions (decomposed):**
  1. Implement frame geometry rules.
  2. Implement pivot subsystem geometry and mate behavior.
  3. Implement height-index configuration states.
  4. Validate generated model against requirement matrix.
- **Output:** Generated CAD parts/assemblies for baseline configurations.
- **How to verify:**
  - Evidence: SolidWorks model tree with expected deterministic names and configurations.
  - Completion: baseline configurations open successfully and pass dimensional checks.

### C3. Build Validation Checkpoint
- **Owner:** SW+ME
- **Complexity:** M
- **Goal:** Confirm Build outputs are mechanically valid before export implementation.
- **Actions:**
  1. Run build for representative low/mid/high configurations.
  2. Execute dimensional and mate integrity review.
- **Output:** Build validation record.
- **How to verify:**
  - Evidence: signed checklist + issue log (if any).
  - Completion: all critical checks pass or are explicitly waived with rationale.

**Gate C Exit Criteria:** Build flow is stable, traceable, and mechanically reviewed.

---

## Phase D — Final Output Delivery (Gate D)

### D1. Export Pipeline (STEP + DXF)
- **Owner:** SW
- **Complexity:** H
- **Goal:** Generate manufacturing exchange files from validated CAD output.
- **Actions (decomposed):**
  1. Implement STEP export for assembly and required parts.
  2. Implement DXF export for plate components.
  3. Enforce deterministic output paths and filenames.
  4. Add export error handling and logging.
- **Output:** Exported STEP/DXF files in run folder.
- **How to verify:**
  - Evidence: file system output in `Output/<run-id>/` with expected file counts.
  - Completion: exports are readable in downstream tools/CAD viewers.

### D2. BOM and Validation Report Generation
- **Owner:** SW+ME
- **Complexity:** M
- **Goal:** Produce consumable manufacturing and QA documentation.
- **Actions:**
  1. Generate BOM with part identifiers, counts, and key attributes.
  2. Generate validation report with pass/fail checks tied to rules.
- **Output:** BOM and validation report artifacts.
- **How to verify:**
  - Evidence: report sections map to requirement matrix IDs.
  - Completion: BOM totals and report statuses match generated assembly state.

### D3. Run Summary and Packaging
- **Owner:** SW
- **Complexity:** M
- **Goal:** Present complete run summary and package outputs for release.
- **Actions:**
  1. Assemble per-run manifest (all generated files + metadata).
  2. Surface summary in UI/log.
- **Output:** Packaged run folder + manifest.
- **How to verify:**
  - Evidence: run summary references every file in package.
  - Completion: no missing required deliverables for the selected run.

**Gate D Exit Criteria:** Final Output produces complete, readable, and traceable manufacturing artifacts.

---

## Phase E — Quality, Regression, and Release (Gate E)

### E1. Test Suite Completion (Unit/Integration/Regression)
- **Owner:** SW
- **Complexity:** H
- **Goal:** Ensure stability across parser/validator logic and Build/Final Output workflows.
- **Actions (decomposed):**
  1. Unit tests for config parsing, validators, naming, and path logic.
  2. Integration tests for Build and export flow.
  3. Regression suite against prior revision baselines.
  4. CI pipeline execution and result review.
- **Output:** Passing test runs and CI status.
- **How to verify:**
  - Evidence: CI dashboard/job URLs and test summaries.
  - Completion: required suites pass per branch policy.

### E2. Definition of Done (DoD) Compliance Review
- **Owner:** SW+ME
- **Complexity:** M
- **Goal:** Validate architecture compliance, tests, docs, and output expectations.
- **Actions:**
  1. Review implementation against architecture contracts.
  2. Confirm updated documentation and output expectations.
- **Output:** DoD sign-off checklist.
- **How to verify:**
  - Evidence: completed checklist attached to release candidate.
  - Completion: all mandatory DoD items marked pass.

### E3. Release Promotion and Documentation Update
- **Owner:** SW (release), ME (approval)
- **Complexity:** M
- **Goal:** Publish versioned outputs and update project records.
- **Actions:**
  1. Update revision metadata.
  2. Promote run artifacts to release folder.
  3. Update documentation references/changelog.
- **Output:** Versioned release bundle and updated docs.
- **How to verify:**
  - Evidence: release folder path, artifact list, and commit hash.
  - Completion: release package is reproducible from recorded run metadata.

**Gate E Exit Criteria:** All quality gates pass and release artifacts are published with traceability.

---

## 3) Weighted/Complex Steps That Were Decomposed

The following higher-weight activities were explicitly broken down to reduce risk and increase observability:
- **A2:** Mechanical requirements-to-traceability conversion.
- **B2:** Configuration schema and validator implementation.
- **C1:** Build orchestration and logging lifecycle.
- **C2:** Domain CAD generation modules.
- **D1:** Export pipeline for STEP and DXF.
- **E1:** Full test/CI strategy across unit, integration, and regression.

These are high-complexity because they involve cross-discipline coupling, correctness risk, and downstream dependency impact.

---

## 4) Sequence Validation (Dependency Check)

The roadmap ordering is intentionally constrained as:
1. **Requirements before schema** (A before B) to avoid invalid config contracts.
2. **Schema/naming before CAD generation** (B before C) to ensure deterministic and testable build behavior.
3. **Build validation before exports** (C before D) so manufacturing outputs are generated only from validated geometry.
4. **Testing/DoD before release** (E after D) to prevent unverified deliverables from promotion.

If any gate fails, execution returns to the prior phase owning the dependency.

---

## 5) Progress Visibility Dashboard (Suggested)

Track roadmap progress in a project board with these required fields per step:
- `Step ID` (A1…E3)
- `Owner`
- `Status` (Not Started / In Progress / Blocked / Done)
- `Evidence Link` (CI URL, output folder path, screenshot, log path, or checklist)
- `Verification Result` (Pass/Fail)
- `Blocker` and `ETA`

Minimum acceptance: a step cannot be marked **Done** without evidence link + pass result.
