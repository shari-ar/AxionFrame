using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class FrameModule
    {
        private readonly DeterministicNamingService _naming;

        public FrameModule()
            : this(new DeterministicNamingService())
        {
        }

        public FrameModule(DeterministicNamingService naming)
        {
            if (naming == null)
            {
                throw new ArgumentNullException("naming");
            }

            _naming = naming;
        }

        public string GetLayoutPrimaryFeatureName()
        {
            return _naming.CreateFeatureName("FRM", "LAYOUT", "PRIMARY");
        }

        public string GetProfileMainFeatureName()
        {
            return _naming.CreateFeatureName("FRM", "PROFILE", "MAIN");
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
