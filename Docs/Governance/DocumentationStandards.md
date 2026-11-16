# Documentation Standards

- **Title:** Documentation Standards
- **Status:** Active
- **Owner:** Joint Software and Mechanical Engineering
- **Scope:** Language, structure, precision, decision quality, lifecycle, naming, review, and publishing standards for project documentation.

## Language Standard

- Every active document uses US English.
- Every sentence uses affirmative construction.
- Every statement carries one precise technical meaning.
- Terminology remains consistent across mechanical, software, workflow, verification, and release documents.
- Numeric values include explicit units.
- Requirement statements use measurable acceptance language.

## Audience Standard

- Documentation addresses a technical audience.
- Technical content describes product behavior, engineering intent, implementation contracts, evidence, and acceptance criteria.
- Repository documentation presents technical content, engineering decisions, implementation contracts, and verification evidence.
- Documentation presents approved technical outcomes and traceable engineering facts.

## Decision Standard

- Every decision record identifies one selected outcome.
- Every decision record identifies its owner, status, scope, effective date, impact, acceptance basis, and related documents.
- Every active document aligns with the Project Decision Register.
- Every changed decision updates the authoritative document, roadmap, requirements, verification scope, and decision register in one pull request.

## Document Header Standard

Each active document includes:

- Title
- Status
- Owner
- Scope

Engineering and workflow documents also include applicable inputs, outputs, acceptance criteria, and related documents.

## Naming Standard

- Folder names use PascalCase.
- Markdown files use descriptive PascalCase names.
- Decision records use stable identifiers such as `CR-001` and `DR-001`.
- Feature and stage records use the roadmap identifier in the filename.

## Lifecycle Standard

- **Active** identifies the current authoritative document.
- **Superseded** identifies a retained document with a named successor.
- **Archived** identifies preserved historical evidence.
- Historical records preserve their original execution evidence and receive an explicit lifecycle classification through the documentation hub or document header.

## Content Quality Standard

Each document provides:

- Purpose and scope
- Clear heading structure
- Explicit inputs and outputs
- Defined ownership
- Measurable acceptance criteria
- Traceable related documents
- Consistent terminology
- Complete decision outcomes

## Product Alignment Standard

Active workflow and architecture documents align with this sequence:

```text
Settings
  -> Input Validation
  -> Build
  -> Build Verification Gate
  -> Final Output
  -> Release
```

## Review and Update Workflow

1. Identify the authoritative source.
2. Update the decision register when decision content changes.
3. Update affected requirements, contracts, roadmap stages, and verification scope.
4. Validate internal links and terminology.
5. Run documentation quality checks.
6. Review through a pull request.
7. Publish the approved content through GitHub Pages.
