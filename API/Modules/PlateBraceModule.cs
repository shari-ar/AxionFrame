using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class PlateBraceModule
    {
        private readonly DeterministicNamingService _naming;

        public PlateBraceModule()
            : this(new DeterministicNamingService())
        {
        }

        public PlateBraceModule(DeterministicNamingService naming)
        {
            if (naming == null)
            {
                throw new ArgumentNullException("naming");
            }

            _naming = naming;
        }

        public string GetBracePrimaryFeatureName()
        {
            return _naming.CreateFeatureName("PLT", "BRACE", "PRIMARY");
        }

        public string GetDxfTraceabilityFeatureName()
        {
            return _naming.GetRequiredStableHook("PLT-002");
        }

        public string GetDxfExportArtifactName()
        {
            return _naming.CreateExportArtifactName("DXF", "PLATE_SET");
        }

        public string GetTraceabilityValidationSectionIdentifier()
        {
            return _naming.CreateValidationSectionIdentifier("PLT", "TRACEABILITY");
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
