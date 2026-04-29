using System;
using System.Collections.Generic;

namespace AxionFrame
{
    public sealed class FrameBuildOutput
    {
        public FrameBuildOutput(
            IList<string> featureNames,
            decimal memberExtentMin,
            decimal memberExtentMax,
            decimal placementTolerance,
            IList<string> allowedProfiles,
            decimal profileDimensionTolerance,
            string namingRuleSet,
            IList<string> tracePoints)
        {
            FeatureNames = featureNames ?? new List<string>();
            MemberExtentMin = memberExtentMin;
            MemberExtentMax = memberExtentMax;
            PlacementTolerance = placementTolerance;
            AllowedProfiles = allowedProfiles ?? new List<string>();
            ProfileDimensionTolerance = profileDimensionTolerance;
            NamingRuleSet = namingRuleSet ?? string.Empty;
            TracePoints = tracePoints ?? new List<string>();
        }

        public IList<string> FeatureNames { get; private set; }
        public decimal MemberExtentMin { get; private set; }
        public decimal MemberExtentMax { get; private set; }
        public decimal PlacementTolerance { get; private set; }
        public IList<string> AllowedProfiles { get; private set; }
        public decimal ProfileDimensionTolerance { get; private set; }
        public string NamingRuleSet { get; private set; }
        public IList<string> TracePoints { get; private set; }
    }

    public sealed class FrameModule
    {
        private const string DomainFrame = "FRM";
        private const string ComponentLayout = "LAYOUT";
        private const string ComponentProfile = "PROFILE";
        private const string DescriptorPrimary = "PRIMARY";
        private const string DescriptorMain = "MAIN";
        private const string ConfigLayoutMemberExtentMin = "frame.layout.primary.memberExtentMin";
        private const string ConfigLayoutMemberExtentMax = "frame.layout.primary.memberExtentMax";
        private const string ConfigLayoutPlacementTolerance = "frame.layout.primary.placementTolerance";
        private const string ConfigAllowedProfiles = "frame.profile.selection.allowedProfiles";
        private const string ConfigProfileDimensionTolerance = "frame.profile.selection.dimensionTolerance";
        private const string ConfigNamingRuleSet = "frame.naming.ruleSet";
        private const string ReportTraceFrameLayout = "frame.layout";
        private const string ReportTraceFrameMembers = "frame.members";
        private const string ReportTraceFrameNaming = "traceability.naming.frame";

        private static readonly string[] AllowedBaselineProfiles =
        {
            "40x40x2.0_SHS",
            "60x30x2.0_RHS"
        };

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

        public FrameBuildOutput CreateBuildOutput(IDictionary<string, object> normalizedConfig)
        {
            if (normalizedConfig == null)
            {
                throw new ArgumentNullException(nameof(normalizedConfig));
            }

            decimal memberExtentMin = GetRequiredDecimal(normalizedConfig, ConfigLayoutMemberExtentMin);
            decimal memberExtentMax = GetRequiredDecimal(normalizedConfig, ConfigLayoutMemberExtentMax);
            decimal placementTolerance = GetRequiredDecimal(normalizedConfig, ConfigLayoutPlacementTolerance);
            List<string> allowedProfiles = GetRequiredStringList(normalizedConfig, ConfigAllowedProfiles);
            decimal profileDimensionTolerance = GetRequiredDecimal(normalizedConfig, ConfigProfileDimensionTolerance);
            string namingRuleSet = GetRequiredString(normalizedConfig, ConfigNamingRuleSet);

            if (memberExtentMin > memberExtentMax)
            {
                throw new InvalidOperationException("Frame member extent range is invalid: minimum exceeds maximum.");
            }

            if (placementTolerance < 0m)
            {
                throw new InvalidOperationException("Frame placement tolerance cannot be negative.");
            }

            if (profileDimensionTolerance < 0m)
            {
                throw new InvalidOperationException("Frame profile dimension tolerance cannot be negative.");
            }

            if (!string.Equals(namingRuleSet, DeterministicNamingService.RuleSetStandardV1, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unsupported frame naming rule set: " + namingRuleSet + ".");
            }

            // Frame rules are already schema-validated; this guard keeps module behavior constrained to the documented baseline profile family.
            ValidateAllowedProfiles(allowedProfiles);

            List<string> featureNames = new List<string>
            {
                GetLayoutPrimaryFeatureName(),
                GetProfileMainFeatureName()
            };

            List<string> tracePoints = new List<string>
            {
                ReportTraceFrameLayout,
                ReportTraceFrameMembers,
                ReportTraceFrameNaming
            };

            return new FrameBuildOutput(
                featureNames,
                memberExtentMin,
                memberExtentMax,
                placementTolerance,
                new List<string>(allowedProfiles),
                profileDimensionTolerance,
                namingRuleSet,
                tracePoints);
        }

        private static decimal GetRequiredDecimal(IDictionary<string, object> normalizedConfig, string key)
        {
            object value;
            if (!normalizedConfig.TryGetValue(key, out value))
            {
                throw new InvalidOperationException("Required frame configuration key is missing: " + key + ".");
            }

            if (!(value is decimal))
            {
                throw new InvalidOperationException("Frame configuration key '" + key + "' is not a decimal. Actual type: " + (value == null ? "<null>" : value.GetType().Name) + ".");
            }

            return (decimal)value;
        }

        private static List<string> GetRequiredStringList(IDictionary<string, object> normalizedConfig, string key)
        {
            object value;
            if (!normalizedConfig.TryGetValue(key, out value))
            {
                throw new InvalidOperationException("Required frame configuration key is missing: " + key + ".");
            }

            List<string> values = value as List<string>;
            if (values == null)
            {
                throw new InvalidOperationException("Frame configuration key '" + key + "' is not an array<string>. Actual type: " + (value == null ? "<null>" : value.GetType().Name) + ".");
            }

            return values;
        }

        private static string GetRequiredString(IDictionary<string, object> normalizedConfig, string key)
        {
            object value;
            if (!normalizedConfig.TryGetValue(key, out value))
            {
                throw new InvalidOperationException("Required frame configuration key is missing: " + key + ".");
            }

            string stringValue = value as string;
            if (stringValue == null)
            {
                throw new InvalidOperationException("Frame configuration key '" + key + "' is not a string. Actual type: " + (value == null ? "<null>" : value.GetType().Name) + ".");
            }

            return stringValue;
        }

        private static void ValidateAllowedProfiles(IList<string> configuredProfiles)
        {
            if (configuredProfiles == null || configuredProfiles.Count == 0)
            {
                throw new InvalidOperationException("Frame allowed profile set cannot be empty.");
            }

            for (int i = 0; i < configuredProfiles.Count; i++)
            {
                if (!IsBaselineProfile(configuredProfiles[i]))
                {
                    throw new InvalidOperationException("Unsupported frame profile for baseline S3.2 behavior: " + configuredProfiles[i] + ".");
                }
            }
        }

        private static bool IsBaselineProfile(string value)
        {
            for (int i = 0; i < AllowedBaselineProfiles.Length; i++)
            {
                if (string.Equals(AllowedBaselineProfiles[i], value, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
