using System;
using System.Collections.Generic;
using System.Globalization;

namespace AxionFrame
{
    public sealed class FrameProfileDefinition
    {
        public FrameProfileDefinition(string profileCode, decimal widthMillimeters, decimal heightMillimeters, decimal wallThicknessMillimeters, string profileFamily)
        {
            ProfileCode = profileCode ?? string.Empty;
            WidthMillimeters = widthMillimeters;
            HeightMillimeters = heightMillimeters;
            WallThicknessMillimeters = wallThicknessMillimeters;
            ProfileFamily = profileFamily ?? string.Empty;
        }

        public string ProfileCode { get; private set; }
        public decimal WidthMillimeters { get; private set; }
        public decimal HeightMillimeters { get; private set; }
        public decimal WallThicknessMillimeters { get; private set; }
        public string ProfileFamily { get; private set; }
    }

    public sealed class FrameGeometryRequest
    {
        public FrameGeometryRequest(
            string layoutFeatureName,
            string profileFeatureName,
            decimal memberExtentMin,
            decimal memberExtentMax,
            decimal placementTolerance,
            decimal tableWidth,
            decimal tableHeight,
            decimal profileDimensionTolerance,
            string selectedProfileCode,
            FrameProfileDefinition selectedProfile,
            string namingRuleSet)
        {
            LayoutFeatureName = layoutFeatureName ?? string.Empty;
            ProfileFeatureName = profileFeatureName ?? string.Empty;
            MemberExtentMin = memberExtentMin;
            MemberExtentMax = memberExtentMax;
            PlacementTolerance = placementTolerance;
            TableWidth = tableWidth;
            TableHeight = tableHeight;
            ProfileDimensionTolerance = profileDimensionTolerance;
            SelectedProfileCode = selectedProfileCode ?? string.Empty;
            SelectedProfile = selectedProfile ?? new FrameProfileDefinition(string.Empty, 0m, 0m, 0m, string.Empty);
            NamingRuleSet = namingRuleSet ?? string.Empty;
        }

        public string LayoutFeatureName { get; private set; }
        public string ProfileFeatureName { get; private set; }
        public decimal MemberExtentMin { get; private set; }
        public decimal MemberExtentMax { get; private set; }
        public decimal PlacementTolerance { get; private set; }
        public decimal TableWidth { get; private set; }
        public decimal TableHeight { get; private set; }
        public decimal ProfileDimensionTolerance { get; private set; }
        public string SelectedProfileCode { get; private set; }
        public FrameProfileDefinition SelectedProfile { get; private set; }
        public string NamingRuleSet { get; private set; }
    }

    public sealed class FrameGeometryResult
    {
        public FrameGeometryResult(bool geometryCreated, string activeDocumentName, string executionNote)
        {
            GeometryCreated = geometryCreated;
            ActiveDocumentName = activeDocumentName ?? string.Empty;
            ExecutionNote = executionNote ?? string.Empty;
        }

        public bool GeometryCreated { get; private set; }
        public string ActiveDocumentName { get; private set; }
        public string ExecutionNote { get; private set; }

        public static FrameGeometryResult NotRequested(string note)
        {
            return new FrameGeometryResult(false, string.Empty, note ?? string.Empty);
        }
    }

    public interface IFrameGeometryExecutor
    {
        FrameGeometryResult Generate(FrameGeometryRequest request);
    }

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
            : this(
                featureNames,
                memberExtentMin,
                memberExtentMax,
                placementTolerance,
                allowedProfiles,
                profileDimensionTolerance,
                namingRuleSet,
                tracePoints,
                string.Empty,
                false,
                string.Empty,
                string.Empty)
        {
        }

        public FrameBuildOutput(
            IList<string> featureNames,
            decimal memberExtentMin,
            decimal memberExtentMax,
            decimal placementTolerance,
            IList<string> allowedProfiles,
            decimal profileDimensionTolerance,
            string namingRuleSet,
            IList<string> tracePoints,
            string selectedProfileCode,
            bool geometryCreated,
            string activeDocumentName,
            string geometryExecutionNote)
        {
            FeatureNames = featureNames ?? new List<string>();
            MemberExtentMin = memberExtentMin;
            MemberExtentMax = memberExtentMax;
            PlacementTolerance = placementTolerance;
            AllowedProfiles = allowedProfiles ?? new List<string>();
            ProfileDimensionTolerance = profileDimensionTolerance;
            NamingRuleSet = namingRuleSet ?? string.Empty;
            TracePoints = tracePoints ?? new List<string>();
            SelectedProfileCode = selectedProfileCode ?? string.Empty;
            GeometryCreated = geometryCreated;
            ActiveDocumentName = activeDocumentName ?? string.Empty;
            GeometryExecutionNote = geometryExecutionNote ?? string.Empty;
        }

        public IList<string> FeatureNames { get; private set; }
        public decimal MemberExtentMin { get; private set; }
        public decimal MemberExtentMax { get; private set; }
        public decimal PlacementTolerance { get; private set; }
        public IList<string> AllowedProfiles { get; private set; }
        public decimal ProfileDimensionTolerance { get; private set; }
        public string NamingRuleSet { get; private set; }
        public IList<string> TracePoints { get; private set; }
        public string SelectedProfileCode { get; private set; }
        public bool GeometryCreated { get; private set; }
        public string ActiveDocumentName { get; private set; }
        public string GeometryExecutionNote { get; private set; }
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
        private const string ConfigLayoutTableWidth = "frame.layout.primary.tableWidth";
        private const string ConfigLayoutTableHeight = "frame.layout.primary.tableHeight";
        private const string ConfigAllowedProfiles = "frame.profile.selection.allowedProfiles";
        private const string ConfigProfileDimensionTolerance = "frame.profile.selection.dimensionTolerance";
        private const string ConfigNamingRuleSet = "frame.naming.ruleSet";
        private const string ReportTraceFrameLayout = "frame.layout";
        private const string ReportTraceFrameMembers = "frame.members";
        private const string ReportTraceFrameNaming = "traceability.naming.frame";

        private readonly DeterministicNamingService _naming;
        private readonly IFrameGeometryExecutor _geometryExecutor;

        public FrameModule()
            : this(new DeterministicNamingService(), null)
        {
        }

        public FrameModule(DeterministicNamingService naming)
            : this(naming, null)
        {
        }

        public FrameModule(DeterministicNamingService naming, IFrameGeometryExecutor geometryExecutor)
        {
            if (naming == null)
            {
                throw new ArgumentNullException(nameof(naming));
            }

            _naming = naming;
            _geometryExecutor = geometryExecutor;
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
            decimal tableWidth = GetRequiredDecimal(normalizedConfig, ConfigLayoutTableWidth);
            decimal tableHeight = GetRequiredDecimal(normalizedConfig, ConfigLayoutTableHeight);
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

            if (tableWidth <= 0m)
            {
                throw new InvalidOperationException("Frame table width must be greater than zero.");
            }

            if (tableHeight <= 0m)
            {
                throw new InvalidOperationException("Frame table height must be greater than zero.");
            }

            if (!string.Equals(namingRuleSet, DeterministicNamingService.RuleSetStandardV1, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Unsupported frame naming rule set: " + namingRuleSet + ".");
            }

            // Frame rules are already schema-validated; this guard keeps module behavior constrained to the documented baseline profile family.
            ValidateAllowedProfiles(allowedProfiles);
            string selectedProfileCode = SelectActiveProfile(allowedProfiles);
            FrameProfileDefinition selectedProfile = ParseProfileDefinition(selectedProfileCode);

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

            FrameGeometryResult geometryResult = GenerateFrameGeometry(
                featureNames[0],
                featureNames[1],
                memberExtentMin,
                memberExtentMax,
                placementTolerance,
                tableWidth,
                tableHeight,
                profileDimensionTolerance,
                selectedProfileCode,
                selectedProfile,
                namingRuleSet);

            return new FrameBuildOutput(
                featureNames,
                memberExtentMin,
                memberExtentMax,
                placementTolerance,
                new List<string>(allowedProfiles),
                profileDimensionTolerance,
                namingRuleSet,
                tracePoints,
                selectedProfileCode,
                geometryResult.GeometryCreated,
                geometryResult.ActiveDocumentName,
                geometryResult.ExecutionNote);
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
                string profileCode = configuredProfiles[i];
                if (string.IsNullOrWhiteSpace(profileCode))
                {
                    throw new InvalidOperationException("Frame allowed profile contains an empty value.");
                }
            }
        }

        private FrameGeometryResult GenerateFrameGeometry(
            string layoutFeatureName,
            string profileFeatureName,
            decimal memberExtentMin,
            decimal memberExtentMax,
            decimal placementTolerance,
            decimal tableWidth,
            decimal tableHeight,
            decimal profileDimensionTolerance,
            string selectedProfileCode,
            FrameProfileDefinition selectedProfile,
            string namingRuleSet)
        {
            if (_geometryExecutor == null)
            {
                return FrameGeometryResult.NotRequested("Frame geometry executor is not configured for this runtime.");
            }

            FrameGeometryRequest request = new FrameGeometryRequest(
                layoutFeatureName,
                profileFeatureName,
                memberExtentMin,
                memberExtentMax,
                placementTolerance,
                tableWidth,
                tableHeight,
                profileDimensionTolerance,
                selectedProfileCode,
                selectedProfile,
                namingRuleSet);

            try
            {
                FrameGeometryResult result = _geometryExecutor.Generate(request);
                if (result == null)
                {
                    throw new InvalidOperationException("Frame geometry executor returned a null result.");
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Frame geometry generation failed for S3.2 baseline behavior: " + ex.Message, ex);
            }
        }

        private static string SelectActiveProfile(IList<string> configuredProfiles)
        {
            if (configuredProfiles == null || configuredProfiles.Count == 0)
            {
                throw new InvalidOperationException("Frame allowed profile set cannot be empty.");
            }

            return configuredProfiles[0];
        }

        private static FrameProfileDefinition ParseProfileDefinition(string profileCode)
        {
            if (string.IsNullOrWhiteSpace(profileCode))
            {
                throw new InvalidOperationException("Selected frame profile is empty.");
            }

            string[] profileAndFamily = profileCode.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (profileAndFamily.Length != 2)
            {
                throw new InvalidOperationException("Frame profile format is invalid: " + profileCode + ".");
            }

            string[] dimensions = profileAndFamily[0].Split(new[] { 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries);
            if (dimensions.Length != 3)
            {
                throw new InvalidOperationException("Frame profile dimensions are invalid: " + profileCode + ".");
            }

            decimal width = ParsePositiveDecimal(dimensions[0], "width", profileCode);
            decimal height = ParsePositiveDecimal(dimensions[1], "height", profileCode);
            decimal wallThickness = ParsePositiveDecimal(dimensions[2], "wall thickness", profileCode);

            decimal halfSmallestDimension = Math.Min(width, height) / 2m;
            if (wallThickness >= halfSmallestDimension)
            {
                throw new InvalidOperationException("Frame profile wall thickness is not physically valid for profile: " + profileCode + ".");
            }

            return new FrameProfileDefinition(profileCode, width, height, wallThickness, profileAndFamily[1]);
        }

        private static decimal ParsePositiveDecimal(string value, string tokenName, string profileCode)
        {
            decimal parsedValue;
            if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out parsedValue))
            {
                throw new InvalidOperationException("Frame profile " + tokenName + " token is invalid for profile: " + profileCode + ".");
            }

            if (parsedValue <= 0m)
            {
                throw new InvalidOperationException("Frame profile " + tokenName + " must be greater than zero for profile: " + profileCode + ".");
            }

            return parsedValue;
        }
    }
}
