using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class HeightAdjustModule
    {
        private readonly DeterministicNamingService _naming;

        public HeightAdjustModule()
            : this(new DeterministicNamingService())
        {
        }

        public HeightAdjustModule(DeterministicNamingService naming)
        {
            if (naming == null)
            {
                throw new ArgumentNullException("naming");
            }

            _naming = naming;
        }

        public IList<string> CreateSupportedConfigurationNames(IList<decimal> supportedHeightsMillimeters)
        {
            if (supportedHeightsMillimeters == null)
            {
                throw new ArgumentNullException("supportedHeightsMillimeters");
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
            return _naming.GetRequiredStableHook("HGT-002");
        }

        public string GetValidationSetHook()
        {
            return _naming.GetRequiredStableHook("HGT-003");
        }
    }
}
