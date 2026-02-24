# AxionFrame Documentation

This documentation hub defines a professional structure for software engineering, mechanical engineering, workflow operations, and architecture decisions.

## Product Outcome

AxionFrame delivers a SolidWorks Add-in. The user updates default settings, clicks **Build**, and generates CAD files. The user then clicks **Final Output** and generates STEP, DXF, BOM, and related deliverables in the `Output/` folder.

## Structure

- **Software/**: Add-in architecture, implementation contracts, and development guidance.
- **Mechanical/**: Mechanical design intent and manufacturing-oriented engineering guidance.
- **Workflow/**: Runtime and release workflow for Build and Final Output operations.
- **Governance/**: Documentation standards, naming, versioning, and ownership.
- **ADR/**: Architecture Decision Records for key technical choices.

## Recommended Reading Order

1. `Workflow/Project-Lifecycle.md`
2. `Software/Architecture.md`
3. `Software/Development-Guide.md`
4. `Mechanical/Design-and-Manufacturing.md`
5. `Governance/Documentation-Standards.md`

## Ownership Model

- Software Lead owns `Software/`.
- Mechanical Lead owns `Mechanical/`.
- Technical Lead owns `Workflow/` and `ADR/`.
- Project Lead owns `Governance/`.

## Change Policy

Every documentation update includes:

- A concise purpose statement.
- Version and date in the document header.
- Links to related documents when relevant.
