using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class PlateBraceModule
    {
        private const string DomainPlateBrace = "PLT";
        private const string ComponentBrace = "BRACE";
        private const string DescriptorPrimary = "PRIMARY";
        private const string RequiredHookDxfTraceability = "PLT-002";
        private const string ExportTypeDxf = "DXF";
        private const string ExportDescriptorPlateSet = "PLATE_SET";
        private const string ValidationDescriptorTraceability = "TRACEABILITY";

        private readonly DeterministicNamingService _naming;

        public PlateBraceModule()
            : this(new DeterministicNamingService())
        {
        }

        public PlateBraceModule(DeterministicNamingService naming)
        {
            if (naming == null)
            {
                throw new ArgumentNullException(nameof(naming));
            }

            _naming = naming;
        }

        public string GetBracePrimaryFeatureName()
        {
            return _naming.CreateFeatureName(DomainPlateBrace, ComponentBrace, DescriptorPrimary);
        }

        public string GetDxfTraceabilityFeatureName()
        {
            return _naming.GetRequiredStableHook(RequiredHookDxfTraceability);
        }

        public string GetDxfExportArtifactName()
        {
            return _naming.CreateExportArtifactName(ExportTypeDxf, ExportDescriptorPlateSet);
        }

        public string GetTraceabilityValidationSectionIdentifier()
        {
            return _naming.CreateValidationSectionIdentifier(DomainPlateBrace, ValidationDescriptorTraceability);
        }

        public IList<string> GetDeterministicIdentifiers()
        {
            return new List<string>
            {
                GetBracePrimaryFeatureName(),
                GetDxfTraceabilityFeatureName(),
                GetDxfExportArtifactName(),
                GetTraceabilityValidationSectionIdentifier()
            };
        }
    }
}
