using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class HeightAdjustModule
    {
        private const string RequiredHookHeightIndexed = "HGT-002";
        private const string RequiredHookHeightValidationSet = "HGT-003";

        private readonly DeterministicNamingService _naming;

        public HeightAdjustModule()
            : this(new DeterministicNamingService())
        {
        }

        public HeightAdjustModule(DeterministicNamingService naming)
        {
            if (naming == null)
            {
                throw new ArgumentNullException(nameof(naming));
            }

            _naming = naming;
        }

        public IList<string> CreateSupportedConfigurationNames(IList<decimal> supportedHeightsMillimeters)
        {
            if (supportedHeightsMillimeters == null)
            {
                throw new ArgumentNullException(nameof(supportedHeightsMillimeters));
            }

            List<string> names = new List<string>();
            for (int i = 0; i < supportedHeightsMillimeters.Count; i++)
            {
                names.Add(_naming.CreateHeightConfigurationName(supportedHeightsMillimeters[i]));
            }

            return names;
        }

        public string GetIndexedActivationHook()
        {
            return _naming.GetRequiredStableHook(RequiredHookHeightIndexed);
        }

        public string GetValidationSetHook()
        {
            return _naming.GetRequiredStableHook(RequiredHookHeightValidationSet);
        }
    }
}
