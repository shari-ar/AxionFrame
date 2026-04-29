using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class PivotModule
    {
        private readonly DeterministicNamingService _naming;

        public PivotModule()
            : this(new DeterministicNamingService())
        {
        }

        public PivotModule(DeterministicNamingService naming)
        {
            if (naming == null)
            {
                throw new ArgumentNullException("naming");
            }

            _naming = naming;
        }

        public string GetJointPrimaryFeatureName()
        {
            return _naming.CreateFeatureName("PVT", "JOINT", "PRIMARY");
        }

        public string GetHolePatternFeatureName()
        {
            return _naming.CreateFeatureName("PVT", "HOLE", "PATTERN");
        }

        public string GetPrimaryMateName()
        {
            return _naming.CreateMateName("PVT", "PRIMARY");
        }

        public IList<string> GetDeterministicIdentifiers()
        {
            return new List<string>
            {
                GetJointPrimaryFeatureName(),
                GetHolePatternFeatureName(),
                GetPrimaryMateName()
            };
        }
    }
}
