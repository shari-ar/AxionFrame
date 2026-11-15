# AxionFrame Agent Instructions

## Authority

These instructions govern every repository task. Active documents listed in `Docs/DocumentationHub.md` provide the product and engineering baseline.

## Documentation Rules

- Write every documentation file in US English.
- Use affirmative sentence construction.
- Give every statement one precise technical meaning.
- Record one selected outcome for every decision.
- Write for a technical audience.
- Keep project-owner communication in the chat or issue tracker.
- Include status, scope, inputs, outputs, acceptance criteria, and related references where applicable.
- Update documentation before implementation when product behavior, engineering rules, or workflow behavior changes.

## SolidWorks Engineering Rules

- Use SolidWorks 2026 documentation as the authoritative API and feature reference.
- Model each CAD operation as a fully defined professional engineering workflow.
- Use current SolidWorks 2026 capabilities that support deterministic, maintainable, and manufacturable results.
- Define geometry, references, features, mates, configurations, tolerances, and output intent explicitly.
- Place a `README.md` file in every code directory.
- Describe directory purpose, responsibilities, inputs, outputs, dependencies, invariants, engineering context, and verification approach in each code-directory README.

## Delivery Rules

- Use short-lived feature branches and pull requests.
- Keep `main` protected through required checks.
- Run fresh dependency restoration and fresh builds in every CI execution.
- Publish the complete `Docs/` content through GitHub Pages.
- Use GitHub-hosted Windows runners for compilation and SolidWorks-independent tests.
- Use the external SolidWorks 2026 verification workstation for CAD execution and manual verification.
- Produce a formal verification package for every SolidWorks verification cycle.
- Preserve build evidence, configuration hashes, manifests, checksums, and verification reports.

## Product Workflow

1. Record approved decisions.
2. Update the active roadmap and engineering baseline.
3. Implement the approved scope.
4. Run automated verification.
5. Produce the SolidWorks verification package.
6. Record SolidWorks verification evidence.
7. Authorize Final Output after the Build Verification Gate passes.
8. Publish versioned release assets.
