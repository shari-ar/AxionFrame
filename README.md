# AxionFrame

AxionFrame is a SolidWorks add-in that helps engineering teams generate a modular, height-adjustable X-frame table from a guided workflow. Users choose project settings, run **Build** to create parametric CAD parts and assemblies, and run **Final Output** to produce manufacturing deliverables such as STEP, DXF, BOM, and validation reports in the `Output/` directory.

## Documentation Overview (`Docs/`)

The `Docs/` folder provides a complete project guide from product workflow to engineering governance.

### 1. Documentation Hub
`Docs/DocumentationHub.md`

- Defines the overall documentation structure.
- Describes the product outcome and recommended reading order.
- Connects software, mechanical, workflow, and governance documents.

### 2. Workflow
`Docs/Workflow/ProjectLifecycle.md`

- Describes the full project lifecycle from requirements to release outputs.
- Explains runtime user flow in SolidWorks for **Build** and **Final Output**.
- Defines the scope of build operations and final-output operations.

### 3. Software Architecture
`Docs/Software/Architecture.md`

- Describes the layered add-in architecture.
- Maps responsibilities across host, core engine, domain modules, shared contracts, and testing.
- Establishes design principles for deterministic naming, validation, reproducibility, and traceability.

### 4. Software Development Guide
`Docs/Software/DevelopmentGuide.md`

- Defines repository conventions and command model expectations.
- Summarizes testing strategy across unit, integration, and regression suites.
- Provides delivery criteria through Definition of Ready and Definition of Done.

### 5. Mechanical Design and Manufacturing
`Docs/Mechanical/DesignAndManufacturing.md`

- Captures the mechanical design intent for the modular X-frame platform.
- Summarizes frame, pivot, height-indexing, and manufacturable plate/brace considerations.
- Defines expected manufacturing deliverables and traceability points.

### 6. Documentation Governance
`Docs/Governance/DocumentationStandards.md`

- Establishes naming, header, and quality standards for project docs.
- Aligns documentation with product behavior and output expectations.
- Defines the documentation update workflow.

## Developer Setup

Contributors can run AxionFrame in a local SolidWorks development workflow.

### Prerequisites

- Windows environment with .NET Framework 4.8 toolchain.
- SolidWorks 2026 installed.
- SolidWorks API installed.
- SolidWorks interop assemblies available in the SolidWorks installation path.
- Visual Studio with C# and MSBuild support.

### Get the Project

```bash
git clone <axion-frame-repo>
cd AxionFrame
```

### Open and Build

Before opening the project, create a local override file for your SolidWorks installation path:

1. Copy `API/Addin/AxionFrame.user.props.example` to `API/Addin/AxionFrame.user.props`.
2. Update `<SolidWorksInstallDir>` in `API/Addin/AxionFrame.user.props` so it matches your local SolidWorks install folder.

Example:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <SolidWorksInstallDir>C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS</SolidWorksInstallDir>
  </PropertyGroup>
</Project>
```

1. Open `API/Addin/AxionFrame.csproj` in Visual Studio.
2. Select `Debug | AnyCPU` for development.
3. Build the project to produce the add-in assembly.
4. Launch SolidWorks from the debug profile and load the AxionFrame add-in.

### Run the Add-in Flow

1. Open the AxionFrame interface in SolidWorks.
2. Update default parameters for your target table configuration.
3. Execute **Build** to generate CAD parts and assemblies.
4. Execute **Final Output** to generate STEP, DXF, BOM, and reports.
5. Review outputs in the run folder under `Output/`.
