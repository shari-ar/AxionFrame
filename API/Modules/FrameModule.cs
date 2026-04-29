using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class FrameModule
    {
        private const string DomainFrame = "FRM";
        private const string ComponentLayout = "LAYOUT";
        private const string ComponentProfile = "PROFILE";
        private const string DescriptorPrimary = "PRIMARY";
        private const string DescriptorMain = "MAIN";

        private readonly DeterministicNamingService _naming;

        public FrameModule()
            : this(new DeterministicNamingService())
        {
        }

        public FrameModule(DeterministicNamingService naming)
        {
            if (naming == null)
            {
                throw new ArgumentNullException(nameof(naming));
            }

            _naming = naming;
        }

        public string GetLayoutPrimaryFeatureName()
        {
            return _naming.CreateFeatureName(DomainFrame, ComponentLayout, DescriptorPrimary);
        }

        public string GetProfileMainFeatureName()
        {
            return _naming.CreateFeatureName(DomainFrame, ComponentProfile, DescriptorMain);
        }

        public IList<string> GetDeterministicFeatureNames()
        {
            return new List<string>
            {
                GetLayoutPrimaryFeatureName(),
                GetProfileMainFeatureName()
            };
        }
    }
}
