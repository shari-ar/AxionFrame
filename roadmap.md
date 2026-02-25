# AxionFrame Integrated Delivery Roadmap

- **Title:** AxionFrame Integrated Delivery Roadmap
- **Version:** 2
- **Scope:** End-to-end execution plan for software developers and mechanical engineers to deliver AxionFrame Build and Final Output capabilities with measurable, auditable progress.
- **Baseline Sources:** `Docs/Workflow/ProjectLifecycle.md`, `Docs/Software/Architecture.md`, `Docs/Software/DevelopmentGuide.md`, `Docs/Mechanical/DesignAndManufacturing.md`, `Docs/Governance/DocumentationStandards.md`.

## 1) Delivery Standard

This roadmap uses a **stage-gate model** with explicit dependencies and measurable completion evidence.

### 1.1 Completion rule (applies to every step)
A step is complete only when all three conditions are met:
1. **Artifact exists** (file, model, report, CI result, package).
2. **Verification evidence exists** (log, screenshot, URL, path, dashboard, checklist).
3. **Acceptance metric passes** (numeric/boolean criterion below).

### 1.2 Complexity scale
- **H (High):** Cross-discipline dependency and high rework risk.
- **M (Medium):** Moderate scope, bounded dependencies.
- **L (Low):** Administrative/documentation task.

### 1.3 Required evidence locations
- `Docs/` for design/decision records.
- CI pipeline URL(s) and run IDs.
- `Output/<run-id>/` for generated manufacturing deliverables.
- Release note/changelog commit and release artifact path.

---

## 2) Sequenced Roadmap (dependency-validated)

## Stage A — Requirements Baseline (Gate A)

| ID | Step | Owner(s) | Complexity | Depends On |
|---|---|---|---|---|
| A1 | Confirm lifecycle scope and outputs | SW + ME | M | — |
| A2 | Build mechanical rule matrix (traceable) | ME (lead), SW (support) | H | A1 |
| A3 | Define feature Definition of Ready (DoR) template | SW | M | A1, A2 |

### A1. Confirm lifecycle scope and outputs
- **Developer actions**
  1. Validate Build scope (parameter load/validate, part/assembly generation).
  2. Validate Final Output scope (STEP, DXF, BOM, validation report).
- **Mechanical actions**
  1. Confirm manufacturing deliverables are sufficient for production handoff.
- **Output artifact:** `Docs/Workflow/LifecycleBaseline.md` (or equivalent decision record).
- **Verification:**
  - Reviewable file in repo with stakeholder sign-off section.
  - Optional screenshot of approved board/ticket state.
- **Acceptance metric:** 100% of required outputs explicitly listed and approved by both SW and ME.

### A2. Build mechanical rule matrix (traceable) **(decomposed high-weight step)**
- **Developer actions**
  1. Define configuration key schema placeholders for each mechanical rule.
  2. Define expected feature/mate naming hooks for implementation traceability.
- **Mechanical actions**
  1. Specify rules for frame, pivot, height indexing, and plate/brace manufacturability.
  2. Add measurable values/ranges/tolerances and criticality level.
- **Output artifact:** `Docs/Mechanical/RequirementTraceMatrix.md` with columns:
  `Rule ID | Requirement | Config Key | CAD Feature/Mate Name | Validation Report Section | Criticality`.
- **Verification:**
  - Markdown table committed and review-approved.
  - Issue tracker link showing SW+ME review complete.
- **Acceptance metric:** No empty cells for traceability columns on critical rules.

### A3. Define DoR template
- **Developer actions**
  1. Create DoR checklist requiring requirement reference, config keys, expected outputs, and tests.
- **Mechanical actions**
  1. Add manufacturability criteria section to DoR checklist.
- **Output artifact:** `Docs/Workflow/FeatureDoRTemplate.md`.
- **Verification:**
  - 2 active feature tickets include populated DoR sections.
- **Acceptance metric:** All new in-scope features blocked from implementation unless DoR is complete.

**Gate A pass condition:** A1–A3 complete and traceability matrix approved.

---

## Stage B — Architecture and Contracts (Gate B)

| ID | Step | Owner(s) | Complexity | Depends On |
|---|---|---|---|---|
| B1 | Validate layer ownership map | SW | M | A3 |
| B2 | Implement configuration schema + validators | SW | H | A2, B1 |
| B3 | Freeze deterministic naming standard | SW + ME | M | A2, B2 |

### B1. Validate layer ownership map
- **Developer actions**
  1. Map components to Host/Core/Modules/Shared/Tests.
  2. Assign owner per module.
- **Mechanical actions**
  1. Confirm module boundaries preserve mechanical intent separation.
- **Output artifact:** `Docs/Software/LayerOwnership.md`.
- **Verification:** Repo file + codeowner/team mapping reference.
- **Acceptance metric:** Every module has one accountable owner.

### B2. Implement configuration schema + validators **(decomposed high-weight step)**
- **Developer actions**
  1. Create/extend configuration keys from trace matrix.
  2. Define type/range/default/required rules.
  3. Implement validator logic and deterministic error messages.
  4. Add unit tests for valid and invalid combinations.
- **Mechanical actions**
  1. Review limits/tolerances encoded in validators.
- **Output artifact:** updated config + validator tests under `API/`.
- **Verification:**
  - CI unit test output (URL + run ID).
  - Test logs showing boundary failures and valid passes.
- **Acceptance metric:** 100% validator tests pass; all critical mechanical limits have tests.

### B3. Freeze deterministic naming standard
- **Developer actions**
  1. Implement naming grammar for features/mates/references.
- **Mechanical actions**
  1. Confirm naming maps to mechanical rule IDs.
- **Output artifact:** naming section in software docs + sample mapping table.
- **Verification:**
  - Diff review demonstrates naming convention use.
  - Spot-check generated model tree names in SolidWorks.
- **Acceptance metric:** Repeated runs produce identical names for same input.

**Gate B pass condition:** configuration contracts, validators, and naming are test-backed and approved.

---

## Stage C — Build Flow Implementation (Gate C)

| ID | Step | Owner(s) | Complexity | Depends On |
|---|---|---|---|---|
| C1 | Wire Build command lifecycle and logging | SW | H | B2, B3 |
| C2 | Implement Frame/Pivot/Height modules | SW (lead), ME (review) | H | C1 |
| C3 | Run multi-configuration build validation | SW + ME | M | C2 |

### C1. Wire Build command lifecycle and logging **(decomposed high-weight step)**
- **Developer actions**
  1. Verify command registration and event wiring.
  2. Add run metadata (`run-id`, timestamp, config hash).
  3. Emit stage logs: load → validate → parts → assembly → summary.
- **Mechanical actions**
  1. Confirm logs include mechanical checkpoints required for review.
- **Output artifact:** executable Build path with run-structured logs.
- **Verification:**
  - Build run log file path and excerpt.
  - Screenshot of run summary in add-in UI (if available).
- **Acceptance metric:** 3 consecutive runs complete without unhandled exceptions.

### C2. Implement Frame/Pivot/Height modules **(decomposed high-weight step)**
- **Developer actions**
  1. Implement frame geometry from matrix rules.
  2. Implement pivot subsystem geometry/mates.
  3. Implement height-indexed configurations.
- **Mechanical actions**
  1. Review generated baseline dimensions and fit.
  2. Record approved dimension checks.
- **Output artifact:** generated parts + assemblies in baseline configurations.
- **Verification:**
  - SolidWorks model tree screenshot(s).
  - Validation checklist with measured values and tolerances.
- **Acceptance metric:** All critical dimension and mate checks pass for low/mid/high configs.

### C3. Run multi-configuration build validation
- **Developer actions**
  1. Execute representative builds (min/mid/max parameter sets).
- **Mechanical actions**
  1. Sign off dimensional and kinematic integrity.
- **Output artifact:** `Docs/Mechanical/BuildValidationRecord.md`.
- **Verification:** linked logs + checklist + issue tracker references.
- **Acceptance metric:** No unresolved critical defects.

**Gate C pass condition:** Build is stable, traceable, and mechanically signed off.

---

## Stage D — Final Output Pipeline (Gate D)

| ID | Step | Owner(s) | Complexity | Depends On |
|---|---|---|---|---|
| D1 | Implement STEP/DXF export pipeline | SW | H | C3 |
| D2 | Generate BOM + validation report | SW + ME | M | D1 |
| D3 | Package run outputs + manifest | SW | M | D2 |

### D1. Implement STEP/DXF export pipeline **(decomposed high-weight step)**
- **Developer actions**
  1. Export assembly/part STEP outputs.
  2. Export plate-component DXFs.
  3. Enforce deterministic naming/pathing in `Output/<run-id>/`.
  4. Add explicit export failure logs.
- **Mechanical actions**
  1. Open random sample files in downstream viewer/CAD tool.
- **Output artifact:** STEP/DXF deliverables per run.
- **Verification:**
  - Folder listing with expected file counts.
  - Viewer-open evidence screenshot(s).
- **Acceptance metric:** 100% required files present and readable.

### D2. Generate BOM + validation report
- **Developer actions**
  1. Generate BOM with identifiers, quantities, and attributes.
  2. Generate pass/fail validation report mapped to Rule IDs.
- **Mechanical actions**
  1. Verify BOM completeness and rule coverage.
- **Output artifact:** BOM + validation report in run folder.
- **Verification:**
  - Report sections map to matrix Rule IDs.
  - BOM quantity spot-check against assembly.
- **Acceptance metric:** No unmapped critical rule; BOM quantity variance = 0 for sampled checks.

### D3. Package run outputs + manifest
- **Developer actions**
  1. Generate run manifest with checksums and metadata.
  2. Surface end-of-run summary.
- **Mechanical actions**
  1. Confirm package includes all manufacturing-required files.
- **Output artifact:** release-ready run package + manifest.
- **Verification:** manifest-to-files parity check script/log.
- **Acceptance metric:** Manifest coverage = 100% required artifacts.

**Gate D pass condition:** Final Output package is complete, readable, and traceable.

---

## Stage E — Quality and Release (Gate E)

| ID | Step | Owner(s) | Complexity | Depends On |
|---|---|---|---|---|
| E1 | Execute unit/integration/regression + CI | SW | H | D3 |
| E2 | Perform Definition of Done audit | SW + ME | M | E1 |
| E3 | Promote release + update docs | SW (lead), ME (approval) | M | E2 |

### E1. Execute unit/integration/regression + CI **(decomposed high-weight step)**
- **Developer actions**
  1. Run unit tests for parser/validator/naming/path logic.
  2. Run integration tests for Build + Final Output flow.
  3. Run regression tests against previous baselines.
  4. Publish CI results.
- **Mechanical actions**
  1. Review regression differences that affect manufacturability.
- **Output artifact:** passing test suites + CI run records.
- **Verification:** CI dashboard URLs + artifact attachments.
- **Acceptance metric:** all required pipelines green per branch policy.

### E2. Perform Definition of Done audit
- **Developer actions**
  1. Confirm architecture and documentation alignment.
- **Mechanical actions**
  1. Confirm manufacturing acceptance criteria met.
- **Output artifact:** signed DoD checklist.
- **Verification:** checklist attached to release candidate.
- **Acceptance metric:** 0 mandatory DoD items open.

### E3. Promote release + update docs
- **Developer actions**
  1. Tag revision and promote artifacts.
  2. Update changelog/references.
- **Mechanical actions**
  1. Final approval for manufacturing handoff.
- **Output artifact:** release bundle with immutable version tag.
- **Verification:** release URL/path + artifact manifest + commit hash.
- **Acceptance metric:** release reproducible from recorded run metadata.

**Gate E pass condition:** quality gates passed and release artifacts published.

---

## 3) High-Complexity Steps Explicitly Broken Down

The following weighted steps were decomposed for risk control and visibility:
- A2 (mechanical rule traceability)
- B2 (configuration contracts + validators)
- C1 (Build lifecycle wiring + logging)
- C2 (domain CAD generation)
- D1 (STEP/DXF export pipeline)
- E1 (multi-level automated validation)

Each decomposed step includes independent verification evidence and measurable acceptance metrics.

---

## 4) Dependency Integrity Check

Execution order is intentionally constrained:
1. **Requirements before contracts** (A → B)
2. **Contracts before generation** (B → C)
3. **Validated generation before exports** (C → D)
4. **Verified outputs before release** (D → E)

If any gate fails, re-open the owning stage and re-run downstream validation steps.

---

## 5) Operational Progress Board (minimum fields)

Track each roadmap step with:
- `Step ID`
- `Owner`
- `Status` (Not Started / In Progress / Blocked / Done)
- `Dependency`
- `Artifact Path or URL`
- `Verification Evidence` (log/screenshot/CI URL)
- `Acceptance Metric Result` (Pass/Fail + value)
- `Blocker` and `Target Date`

**Enforcement rule:** no step may move to **Done** without evidence and a passing metric.
