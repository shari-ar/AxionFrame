# AxionFrame Program Roadmap (Software + Mechanical)

- **Title:** AxionFrame Program Roadmap
- **Version:** 3
- **Scope:** Integrated delivery roadmap for software developers and mechanical engineers to deliver AxionFrame Build and Final Output capabilities with auditable, measurable progress.
- **Standards Basis:** Stage-gate product delivery, requirements traceability, test-gated release, and evidence-based completion criteria.
- **Source Documents:**
  - `Docs/Workflow/ProjectLifecycle.md`
  - `Docs/Software/Architecture.md`
  - `Docs/Software/DevelopmentGuide.md`
  - `Docs/Mechanical/DesignAndManufacturing.md`
  - `Docs/Governance/DocumentationStandards.md`

---

## 1) Delivery Governance Model

## 1.1 Roles
- **SW (Software Engineering):** Add-in host behavior, orchestration, validation, export pipeline, automated tests, release packaging.
- **ME (Mechanical Engineering):** Mechanical design rules, tolerance and manufacturability criteria, output validity for production.
- **Joint (SW+ME):** Rule traceability, quality gates, release approval.

## 1.2 Step Completion Standard (applies to every step)
A step is complete only if all items are present:
1. **Deliverable artifact** exists (document, CAD output, test run, package).
2. **Verification evidence** exists (screenshot, log, URL, dashboard, checklist).
3. **Acceptance metric** is met (explicit pass/fail threshold).

## 1.3 Complexity Weighting
- **High (H):** Cross-team dependency, high failure impact, or major implementation risk.
- **Medium (M):** Bounded implementation with moderate dependency.
- **Low (L):** Documentation/admin process with low technical risk.

## 1.4 Evidence Registry (single source of truth)
For each step, record evidence in the project board with:
- Step ID
- Artifact path/URL
- Verification evidence link
- Acceptance metric result
- Reviewer(s)
- Date/time

---

## 2) Dependency-Validated Stage Plan

> Sequence rule: **Requirements -> Contracts -> Build -> Final Output -> Validation -> Release**. No downstream stage starts until upstream gate is passed.

## Stage 0 — Project Initialization (Gate 0)

| ID | Step | Owner | Complexity | Dependencies |
|---|---|---|---|---|
| S0.1 | Confirm baseline scope and output set | SW+ME | M | None |
| S0.2 | Create roadmap tracking board and evidence registry | SW | L | S0.1 |

### S0.1 Confirm baseline scope and output set
- **Software tasks**
  1. Confirm Build scope: parameter load/validate, CAD generation.
  2. Confirm Final Output scope: STEP, DXF, BOM, validation report.
- **Mechanical tasks**
  1. Confirm outputs satisfy manufacturing handoff requirements.
- **Deliverable:** approved scope baseline note (project board entry or markdown decision record).
- **Verification method:**
  - Screenshot of approved ticket/decision log.
  - Link to source documents used for confirmation.
- **Acceptance metric:** 100% of required outputs acknowledged by SW and ME leads.

### S0.2 Create roadmap tracking board and evidence registry
- **Software tasks**
  1. Create board columns: Not Started / In Progress / Blocked / In Review / Done.
  2. Add mandatory evidence fields defined in §1.4.
- **Mechanical tasks**
  1. Validate manufacturability review fields exist in the board template.
- **Deliverable:** operational tracking board.
- **Verification method:** board URL + screenshot showing required fields.
- **Acceptance metric:** all roadmap steps preloaded with IDs and dependency links.

**Gate 0 pass condition:** S0.1 and S0.2 complete with visible board and evidence schema.

---

## Stage 1 — Requirements and Traceability (Gate 1)

| ID | Step | Owner | Complexity | Dependencies |
|---|---|---|---|---|
| S1.1 | Build requirement traceability matrix | ME (lead), SW (support) | H | Gate 0 |
| S1.2 | Define Definition of Ready (DoR) template | SW+ME | M | S1.1 |
| S1.3 | Classify critical rules and risk priorities | SW+ME | M | S1.1 |

### S1.1 Build requirement traceability matrix **(High, decomposed)**
- **Software tasks**
  1. Create config-key placeholders for each rule.
  2. Define expected feature/mate naming hooks for traceability.
- **Mechanical tasks**
  1. Define rules for frame, pivot, height indexing, and plate/brace geometry.
  2. Add measurable ranges, limits, and tolerances.
- **Deliverable:** `Docs/Mechanical/RequirementTraceMatrix.md`.
- **Verification method:**
  - PR diff link showing complete matrix table.
  - Reviewer sign-off from SW and ME.
- **Acceptance metric:** 0 empty traceability cells for critical rules.

### S1.2 Define Definition of Ready (DoR) template
- **Software tasks**
  1. Add mandatory DoR fields: requirement references, config keys, expected outputs, planned tests.
- **Mechanical tasks**
  1. Add manufacturability readiness checks.
- **Deliverable:** `Docs/Workflow/S1.2-FeatureDoRTemplate.md`.
- **Verification method:** 2 sampled feature tickets show completed DoR template.
- **Acceptance metric:** no implementation ticket may enter In Progress without DoR.

### S1.3 Classify critical rules and risk priorities
- **Software tasks**
  1. Tag rules that require strict validator enforcement.
- **Mechanical tasks**
  1. Assign criticality (Safety/Function/Manufacturing).
- **Deliverable:** critical-rule index attached to traceability matrix.
- **Verification method:** traceability matrix view filtered by criticality tag.
- **Acceptance metric:** all critical rules mapped to at least one verification test/report check.

**Gate 1 pass condition:** requirements traceability and DoR process are complete and approved.

---

## Stage 2 — Architecture and Contract Readiness (Gate 2)

| ID | Step | Owner | Complexity | Dependencies |
|---|---|---|---|---|
| S2.1 | Validate architecture ownership map | SW | M | Gate 1 |
| S2.2 | Implement configuration schema + validators | SW (lead), ME (review) | H | S2.1 |
| S2.3 | Standardize deterministic naming rules | SW+ME | M | S2.2 |

### S2.1 Validate architecture ownership map
- **Software tasks**
  1. Map components to Host/Core/Modules/Shared/Tests.
  2. Assign accountable owner for each module.
- **Mechanical tasks**
  1. Validate domain boundaries preserve mechanical intent.
- **Deliverable:** architecture ownership note (doc or board reference).
- **Verification method:** ownership table screenshot + review link.
- **Acceptance metric:** 100% modules have a named owner.

### S2.2 Implement configuration schema + validators **(High, decomposed)**
- **Software tasks**
  1. Implement key definitions (type/default/range/required).
  2. Implement validators and deterministic error messages.
  3. Add unit tests for valid, invalid, and boundary configurations.
- **Mechanical tasks**
  1. Review encoded tolerance/range limits against matrix.
- **Deliverable:** updated config contracts and validator test coverage.
- **Verification method:**
  - CI unit-test URL.
  - Test output screenshot/log showing boundary pass/fail behavior.
- **Acceptance metric:**
  - Unit test pass rate = 100% for validator suite.
  - 100% critical rules have at least one validator test.

### S2.3 Standardize deterministic naming rules
- **Software tasks**
  1. Define naming grammar for features/mates/references.
  2. Apply rules to generation modules.
- **Mechanical tasks**
  1. Confirm names map back to rule IDs.
- **Deliverable:** naming standard section in docs and sample mapping table.
- **Verification method:**
  - Diff review of naming implementation.
  - Model-tree screenshot from repeated runs.
- **Acceptance metric:** identical inputs produce identical names across 2 repeated runs.

**Gate 2 pass condition:** contracts, validators, and deterministic naming are approved and test-evidenced.

---

## Stage 3 — Build Workflow Delivery (Gate 3)

| ID | Step | Owner | Complexity | Dependencies |
|---|---|---|---|---|
| S3.1 | Implement Build command lifecycle and instrumentation | SW | H | Gate 2 |
| S3.2 | Deliver Frame module behavior | SW (lead), ME (review) | H | S3.1 |
| S3.3 | Deliver Pivot module behavior | SW (lead), ME (review) | H | S3.2 |
| S3.4 | Deliver Height-adjust module behavior | SW (lead), ME (review) | H | S3.3 |
| S3.5 | Run multi-configuration Build validation | SW+ME | M | S3.2, S3.3, S3.4 |

### S3.1 Implement Build command lifecycle and instrumentation **(High, decomposed)**
- **Software tasks**
  1. Verify command registration/event wiring.
  2. Add run metadata (`run-id`, timestamp, config hash).
  3. Emit stage logs: load -> validate -> generate parts -> generate assembly -> summary.
- **Mechanical tasks**
  1. Confirm logs include checkpoints needed for ME review.
- **Deliverable:** executable Build flow with structured run logs.
- **Verification method:**
  - log file path and excerpt.
  - screenshot of Build completion summary.
- **Acceptance metric:** 3 consecutive Build runs complete without unhandled exceptions.

### S3.2 Deliver Frame module behavior **(High, decomposed)**
- **Software tasks**
  1. Implement frame geometry rules from matrix.
  2. Add checks for deterministic feature naming.
- **Mechanical tasks**
  1. Validate dimensions and manufacturable constraints.
- **Deliverable:** frame geometry output for baseline configs.
- **Verification method:** model tree screenshot + dimension checklist.
- **Acceptance metric:** 100% critical frame dimensions within tolerance.

### S3.3 Deliver Pivot module behavior **(High, decomposed)**
- **Software tasks**
  1. Implement pivot geometry and mate definitions.
  2. Add checks for mate naming and integrity.
- **Mechanical tasks**
  1. Validate motion/fit behavior and hole strategy.
- **Deliverable:** pivot subsystem outputs and mate integrity results.
- **Verification method:** motion check screenshot/video + mate integrity log.
- **Acceptance metric:** no critical mate conflicts in baseline configurations.

### S3.4 Deliver Height-adjust module behavior **(High, decomposed)**
- **Software tasks**
  1. Implement supported height-index configurations.
  2. Ensure deterministic state activation across configs.
- **Mechanical tasks**
  1. Confirm configured heights align to approved range table.
- **Deliverable:** generated configurations across min/nominal/max heights.
- **Verification method:** configuration list screenshot + measured values log.
- **Acceptance metric:** all required heights generated and validated.

### S3.5 Run multi-configuration Build validation
- **Software tasks**
  1. Execute Build for min/nominal/max and one stress-case config.
- **Mechanical tasks**
  1. Sign off dimensional and mate integrity checklist.
- **Deliverable:** `Docs/Mechanical/BuildValidationRecord.md`.
- **Verification method:** linked run logs, checklist, and issue tracker references.
- **Acceptance metric:** zero unresolved critical defects.

**Gate 3 pass condition:** Build flow is stable, instrumented, and ME-approved for baseline and stress-case configurations.

---

## Stage 4 — Final Output Workflow Delivery (Gate 4)

| ID | Step | Owner | Complexity | Dependencies |
|---|---|---|---|---|
| S4.1 | Implement STEP export workflow | SW | H | Gate 3 |
| S4.2 | Implement DXF export workflow | SW | H | S4.1 |
| S4.3 | Implement BOM generation | SW+ME | M | S4.1, S4.2 |
| S4.4 | Implement validation report generation | SW+ME | M | S4.3 |
| S4.5 | Package outputs with manifest and checksums | SW | M | S4.4 |

### S4.1 Implement STEP export workflow **(High, decomposed)**
- **Software tasks**
  1. Export required assemblies/parts to STEP.
  2. Enforce deterministic filenames and folder structure.
- **Mechanical tasks**
  1. Open STEP samples in downstream viewer/CAD.
- **Deliverable:** STEP files in `Output/<run-id>/`.
- **Verification method:** folder listing + openability evidence screenshot.
- **Acceptance metric:** 100% required STEP files generated and readable.

### S4.2 Implement DXF export workflow **(High, decomposed)**
- **Software tasks**
  1. Export plate components to DXF.
  2. Log export failures with actionable error detail.
- **Mechanical tasks**
  1. Validate plate profile and hole outputs from DXF samples.
- **Deliverable:** DXF files in `Output/<run-id>/`.
- **Verification method:** folder listing + viewer screenshot + export logs.
- **Acceptance metric:** 100% required DXF files generated and readable.

### S4.3 Implement BOM generation
- **Software tasks**
  1. Generate BOM with part IDs, quantities, and key attributes.
- **Mechanical tasks**
  1. Spot-check BOM counts against generated assembly.
- **Deliverable:** BOM artifact in run folder.
- **Verification method:** BOM screenshot + sampled reconciliation table.
- **Acceptance metric:** sampled BOM variance = 0.

### S4.4 Implement validation report generation
- **Software tasks**
  1. Emit rule-level pass/fail report mapped to matrix Rule IDs.
- **Mechanical tasks**
  1. Confirm every critical rule appears in report.
- **Deliverable:** validation report in run folder.
- **Verification method:** report excerpt screenshot and rule coverage summary.
- **Acceptance metric:** no unmapped critical rule IDs.

### S4.5 Package outputs with manifest and checksums
- **Software tasks**
  1. Generate manifest including file paths, checksums, run metadata.
  2. Publish end-of-run summary.
- **Mechanical tasks**
  1. Confirm package completeness for manufacturing handoff.
- **Deliverable:** complete release-ready run package.
- **Verification method:** manifest-to-folder parity check log.
- **Acceptance metric:** manifest coverage = 100% required artifacts.

**Gate 4 pass condition:** Final Output artifacts are complete, readable, and traceable.

---

## Stage 5 — Integrated Quality and Release (Gate 5)

| ID | Step | Owner | Complexity | Dependencies |
|---|---|---|---|---|
| S5.1 | Execute unit/integration/regression suites | SW | H | Gate 4 |
| S5.2 | Conduct Definition of Done (DoD) audit | SW+ME | M | S5.1 |
| S5.3 | Promote release and update documentation | SW (lead), ME (approval) | M | S5.2 |

### S5.1 Execute unit/integration/regression suites **(High, decomposed)**
- **Software tasks**
  1. Run unit tests for parser/validator/naming/path logic.
  2. Run integration tests for Build and Final Output.
  3. Run regression tests against baseline runs.
- **Mechanical tasks**
  1. Review regressions that impact manufacturability.
- **Deliverable:** CI run records + test reports.
- **Verification method:** CI dashboard URL(s), run IDs, and artifact attachments.
- **Acceptance metric:** all required pipelines green.

### S5.2 Conduct Definition of Done (DoD) audit
- **Software tasks**
  1. Confirm architecture, tests, and docs are aligned.
- **Mechanical tasks**
  1. Confirm manufacturing acceptance criteria met.
- **Deliverable:** signed DoD checklist.
- **Verification method:** checklist link in release candidate record.
- **Acceptance metric:** 0 open mandatory DoD items.

### S5.3 Promote release and update documentation
- **Software tasks**
  1. Tag release revision and promote artifact set.
  2. Update documentation references/changelog.
- **Mechanical tasks**
  1. Provide final manufacturing handoff approval.
- **Deliverable:** versioned release bundle and updated docs.
- **Verification method:** release path/URL, artifact manifest, commit hash.
- **Acceptance metric:** release reproducible using recorded run metadata.

**Gate 5 pass condition:** release is fully validated, approved, and reproducible.

---

## 3) Explicit Decomposition of High-Weight Steps

The following steps carry the highest delivery risk and have been intentionally decomposed:
- S1.1 Requirements traceability matrix construction.
- S2.2 Configuration contracts and validator implementation.
- S3.1 Build lifecycle wiring and instrumentation.
- S3.2/S3.3/S3.4 Domain module deliveries (Frame/Pivot/Height).
- S4.1/S4.2 Export pipelines (STEP/DXF).
- S5.1 End-to-end automated quality verification.

Rationale: each has cross-functional dependencies, downstream impact, and high rework cost if executed incorrectly.

---

## 4) Sequence Integrity Validation

### Validated dependency order
1. Requirements are finalized before contracts are implemented.
2. Contracts are validated before Build logic scales across modules.
3. Build outputs are validated before manufacturing exports are generated.
4. Export artifacts are validated before release promotion.

### Rework rule
If any gate fails, reopen the owning stage and rerun all impacted downstream validation steps before advancing.

---

## 5) Progress Verification Checklist (for every step)

A step can be marked **Done** only if all checks are true:
- [ ] Deliverable artifact is available at documented path/URL.
- [ ] Verification evidence is attached (screenshot/log/CI URL/dashboard).
- [ ] Acceptance metric value is recorded and passes threshold.
- [ ] SW reviewer approval recorded.
- [ ] ME reviewer approval recorded for mechanical-impacting steps.

This checklist enforces visible, measurable, and auditable progress throughout delivery.
