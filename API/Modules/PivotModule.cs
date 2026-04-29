using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class PivotModule
    {
        private const string DomainPivot = "PVT";
        private const string ComponentJoint = "JOINT";
        private const string ComponentHole = "HOLE";
        private const string DescriptorPattern = "PATTERN";
        private const string DescriptorPrimary = "PRIMARY";

        private readonly DeterministicNamingService _naming;

        public PivotModule()
            : this(new DeterministicNamingService())
        {
        }

        public PivotModule(DeterministicNamingService naming)
        {
            if (naming == null)
            {
                throw new ArgumentNullException(nameof(naming));
            }

            _naming = naming;
        }

        public string GetJointPrimaryFeatureName()
        {
            return _naming.CreateFeatureName(DomainPivot, ComponentJoint, DescriptorPrimary);
        }

        public string GetHolePatternFeatureName()
        {
            return _naming.CreateFeatureName(DomainPivot, ComponentHole, DescriptorPattern);
        }

        public string GetPrimaryMateName()
        {
            return _naming.CreateMateName(DomainPivot, DescriptorPrimary);
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
