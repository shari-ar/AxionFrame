using AxionFrame;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AxionFrame.Tests
{
    internal static class ValidatorTestsProgram
    {
        private static int _runCount;
        private static int _failureCount;

        private static int Main(string[] args)
        {
            Run("BaselineConfig_IsValidAndNormalized", BaselineConfig_IsValidAndNormalized);
            Run("SchemaValidation_InvalidTypeForRequiredCount_ReportsDeterministicMessage", SchemaValidation_InvalidTypeForRequiredCount_ReportsDeterministicMessage);
            Run("CrossFieldValidation_FrameMinGreaterThanMax_ReportsCritical", CrossFieldValidation_FrameMinGreaterThanMax_ReportsCritical);
            Run("CrossFieldValidation_StrictReleaseRequiresStopOnCriticalFailure", CrossFieldValidation_StrictReleaseRequiresStopOnCriticalFailure);
            Run("CrossFieldValidation_HeightSupportedSetMustMatch", CrossFieldValidation_HeightSupportedSetMustMatch);
            Run("NamingRules_PrefixSelection_UsesCanonicalPrefixes", NamingRules_PrefixSelection_UsesCanonicalPrefixes);
            Run("NamingRules_DomainTokenMapping_ResolvesDocumentedDomains", NamingRules_DomainTokenMapping_ResolvesDocumentedDomains);
            Run("NamingRules_Normalization_ReplacesPunctuationAndCollapsesUnderscores", NamingRules_Normalization_ReplacesPunctuationAndCollapsesUnderscores);
            Run("NamingRules_Normalization_UppercasesTokens", NamingRules_Normalization_UppercasesTokens);
            Run("NamingRules_HeightConfigurations_UseDecimalFreeTokens", NamingRules_HeightConfigurations_UseDecimalFreeTokens);
            Run("NamingRules_RepeatRuns_AreStableForIdenticalInputs", NamingRules_RepeatRuns_AreStableForIdenticalInputs);
            Run("NamingRules_ComplianceValidation_VerifiesEntityGrammar", NamingRules_ComplianceValidation_VerifiesEntityGrammar);
            Run("NamingRules_RequiredHooks_IncludeDocumentedTraceabilityHooks", NamingRules_RequiredHooks_IncludeDocumentedTraceabilityHooks);
            Run("NamingRules_ModuleSurface_ProducesDocumentedStableHooks", NamingRules_ModuleSurface_ProducesDocumentedStableHooks);
            Run("NamingRules_ModuleSurface_RepeatedOutputsRemainStable", NamingRules_ModuleSurface_RepeatedOutputsRemainStable);
            Run("FrameBehavior_BuildOutput_UsesDocumentedBaselineRules", FrameBehavior_BuildOutput_UsesDocumentedBaselineRules);
            Run("FrameBehavior_BuildOutput_RepeatedOutputsRemainStable", FrameBehavior_BuildOutput_RepeatedOutputsRemainStable);
            Run("BuildLifecycle_Metadata_IsPopulated", BuildLifecycle_Metadata_IsPopulated);
            Run("BuildLifecycle_ConfigHash_IsStableForIdenticalInputs", BuildLifecycle_ConfigHash_IsStableForIdenticalInputs);
            Run("BuildLifecycle_StageOrder_IsDeterministic", BuildLifecycle_StageOrder_IsDeterministic);
            Run("BuildLifecycle_GeneratePartsStage_RecordsFrameTraceabilityDetails", BuildLifecycle_GeneratePartsStage_RecordsFrameTraceabilityDetails);
            Run("BuildLifecycle_SummaryArtifacts_AreWritten", BuildLifecycle_SummaryArtifacts_AreWritten);
            Run("BuildLifecycle_Regression_ThreeConsecutiveRuns_NoUnhandledExceptions", BuildLifecycle_Regression_ThreeConsecutiveRuns_NoUnhandledExceptions);

            if (_failureCount > 0)
            {
                Console.WriteLine("FAILED: " + _failureCount.ToString(CultureInfo.InvariantCulture) + " of " + _runCount.ToString(CultureInfo.InvariantCulture) + " tests failed.");
                return 1;
            }

            Console.WriteLine("PASSED: " + _runCount.ToString(CultureInfo.InvariantCulture) + " tests.");
            return 0;
        }

        private static void BaselineConfig_IsValidAndNormalized()
        {
            ConfigurationProcessingResult result = Execute(BaselineJson());

            AssertTrue(result.IsValid, "Baseline config should be valid.");
            AssertEqual(30, result.NormalizedConfig.Count, "Normalized key count mismatch.");
            AssertFalse(result.HasBlockingFailures, "Baseline config should not have blocking failures.");
            AssertKeyExists(result.NormalizedConfig, "frame.layout.primary.memberExtentMin");
            AssertKeyExists(result.NormalizedConfig, "height.validation.supportedSet");
            AssertKeyExists(result.NormalizedConfig, "validation.mode");
        }

        private static void SchemaValidation_InvalidTypeForRequiredCount_ReportsDeterministicMessage()
        {
            string json = BaselineJson().Replace("\"requiredCount\": 3", "\"requiredCount\": \"three\"");
            ConfigurationProcessingResult result = Execute(json);

            AssertFalse(result.IsValid, "Invalid requiredCount should fail validation.");
            ValidationMessage message = FindMessage(result.Messages, "VAL-HGT-002-ACTIVATION", "height.indexing.activation.requiredCount");
            AssertTrue(message != null, "Expected schema validation message not found.");
            AssertEqual(ValidationSeverity.Critical, message.Severity, "Unexpected severity for requiredCount message.");
            AssertTrue(message.Blocking, "requiredCount type failure must be blocking.");
            AssertEqual("HGT-002", message.RuleId, "Unexpected rule id for requiredCount message.");
            AssertHasContractFields(message);

            int normalizedRequiredCount = (int)result.NormalizedConfig["height.indexing.activation.requiredCount"];
            AssertEqual(3, normalizedRequiredCount, "requiredCount should fall back to default value.");
        }

        private static void CrossFieldValidation_FrameMinGreaterThanMax_ReportsCritical()
        {
            string json = BaselineJson()
                .Replace("\"memberExtentMin\": 620.0", "\"memberExtentMin\": 970.0")
                .Replace("\"memberExtentMax\": 980.0", "\"memberExtentMax\": 960.0");
            ConfigurationProcessingResult result = Execute(json);

            AssertFalse(result.IsValid, "Frame min/max inversion should fail validation.");
            ValidationMessage message = FindMessage(result.Messages, "VAL-FRM-001-LAYOUT", "frame.layout.primary.memberExtentMin / frame.layout.primary.memberExtentMax");
            AssertTrue(message != null, "Expected frame min/max cross-field message not found.");
            AssertEqual(ValidationSeverity.Critical, message.Severity, "Unexpected severity for frame min/max message.");
            AssertTrue(message.Blocking, "Frame min/max cross-field error must be blocking.");
            AssertEqual("frame.layout.primary", message.ArtifactScope, "Unexpected artifact scope.");
            AssertHasContractFields(message);
        }

        private static void CrossFieldValidation_StrictReleaseRequiresStopOnCriticalFailure()
        {
            string json = BaselineJson().Replace("\"stopOnCriticalFailure\": true", "\"stopOnCriticalFailure\": false");
            ConfigurationProcessingResult result = Execute(json);

            AssertFalse(result.IsValid, "StrictRelease with stopOnCriticalFailure=false should fail.");
            ValidationMessage message = FindMessage(result.Messages, "VAL-CFG-STRICT-RELEASE", "validation.stopOnCriticalFailure");
            AssertTrue(message != null, "Expected StrictRelease cross-field message not found.");
            AssertEqual("CFG-REL-001", message.RuleId, "Unexpected rule for StrictRelease cross-field message.");
            AssertEqual(ValidationSeverity.Critical, message.Severity, "Unexpected severity for StrictRelease cross-field message.");
            AssertTrue(message.Blocking, "StrictRelease cross-field message should be blocking.");
            AssertHasContractFields(message);
        }

        private static void CrossFieldValidation_HeightSupportedSetMustMatch()
        {
            string json = BaselineJson().Replace(
                "\"supportedSet\": [\n        680.0,\n        730.0,\n        780.0",
                "\"supportedSet\": [\n        680.0,\n        731.0,\n        780.0");
            ConfigurationProcessingResult result = Execute(json);

            AssertFalse(result.IsValid, "Height supportedSet mismatch should fail.");
            ValidationMessage message = FindMessage(result.Messages, "VAL-HGT-003-HEIGHT-VALIDITY", "height.validation.supportedSet");
            AssertTrue(message != null, "Expected height supported set cross-field message not found.");
            AssertEqual("HGT-003", message.RuleId, "Unexpected rule for height supported set cross-field message.");
            AssertEqual(ValidationSeverity.Critical, message.Severity, "Unexpected severity for height supported set cross-field message.");
            AssertTrue(message.Blocking, "Height supported set cross-field message should be blocking.");
            AssertHasContractFields(message);
        }

        private static void NamingRules_PrefixSelection_UsesCanonicalPrefixes()
        {
            DeterministicNamingService naming = new DeterministicNamingService();

            AssertEqual("AXF_FRM_LAYOUT_PRIMARY", naming.CreateFeatureName("frame", "layout", "primary"), "Unexpected frame feature naming prefix.");
            AssertEqual("AXF_PVT_JOINT_PRIMARY", naming.CreateFeatureName("pivot", "joint", "primary"), "Unexpected pivot feature naming prefix.");
            AssertEqual("AXF_PLT_BRACE_PRIMARY", naming.CreateFeatureName("plate brace", "brace", "primary"), "Unexpected plate/brace feature naming prefix.");
            AssertEqual("AXF_MATE_PVT_PRIMARY", naming.CreateMateName("pivot", "primary"), "Unexpected mate naming prefix.");
            AssertEqual("AXF_CFG_HGT_680", naming.CreateHeightConfigurationName(680.0m), "Unexpected configuration naming prefix.");
            AssertEqual("AXF_EXP_DXF_PLATE_SET", naming.CreateExportArtifactName("dxf", "plate set"), "Unexpected export naming prefix.");
            AssertEqual("AXF_VAL_PLT_TRACEABILITY", naming.CreateValidationSectionIdentifier("plate brace", "traceability"), "Unexpected validation naming prefix.");
        }

        private static void NamingRules_DomainTokenMapping_ResolvesDocumentedDomains()
        {
            DeterministicNamingService naming = new DeterministicNamingService();

            AssertEqual("FRM", naming.ResolveDomainToken("Frame"), "Frame domain token mismatch.");
            AssertEqual("PVT", naming.ResolveDomainToken("pivot"), "Pivot domain token mismatch.");
            AssertEqual("HGT", naming.ResolveDomainToken("height indexing"), "Height domain token mismatch.");
            AssertEqual("PLT", naming.ResolveDomainToken("plate and brace"), "Plate/brace domain token mismatch.");
            AssertThrows<ArgumentException>(
                delegate { naming.ResolveDomainToken("unsupported-domain"); },
                "Unsupported naming domains must throw ArgumentException.");
        }

        private static void NamingRules_Normalization_ReplacesPunctuationAndCollapsesUnderscores()
        {
            DeterministicNamingService naming = new DeterministicNamingService();

            AssertEqual("LAYOUT_PRIMARY_MAIN", naming.NormalizeToken("layout...primary---main"), "Unexpected punctuation normalization result.");
            AssertEqual("PIVOT_JOINT_PRIMARY", naming.NormalizeToken("__pivot///joint___primary__"), "Unexpected duplicate underscore normalization result.");
            AssertEqual("AXF_FRM_LAYOUT_PRIMARY", naming.CreateFeatureName("frame", "layout", "_primary_"), "Feature names should not emit duplicate underscores.");
        }

        private static void NamingRules_Normalization_UppercasesTokens()
        {
            DeterministicNamingService naming = new DeterministicNamingService();

            AssertEqual("PROFILE_MAIN", naming.NormalizeToken("profile main"), "Normalized token should be uppercase.");
            AssertEqual("AXF_MATE_PVT_PRIMARY_LOCK", naming.CreateMateName("pivot", "primary lock"), "Mate naming tokens should be uppercase.");
            AssertEqual("AXF_EXP_STEP_FRAME_LAYOUT", naming.CreateExportArtifactName("step", "Frame Layout"), "Export naming tokens should be uppercase.");
        }

        private static void NamingRules_HeightConfigurations_UseDecimalFreeTokens()
        {
            DeterministicNamingService naming = new DeterministicNamingService();

            AssertEqual("680", naming.NormalizeDiscreteNumericToken(680.0m), "Discrete height token should be decimal-free.");
            AssertEqual("730", naming.NormalizeDiscreteNumericToken(730m), "Discrete height token should remain decimal-free.");
            AssertEqual("AXF_CFG_HGT_780", naming.CreateHeightConfigurationName(780.0m), "Height configuration naming should use decimal-free token.");
        }

        private static void NamingRules_RepeatRuns_AreStableForIdenticalInputs()
        {
            DeterministicNamingService naming = new DeterministicNamingService();

            string[] firstRun =
            {
                naming.CreateFeatureName("frame", "layout", "primary"),
                naming.CreateMateName("pivot", "primary"),
                naming.CreateHeightConfigurationName(680.0m),
                naming.CreateExportArtifactName("dxf", "plate set"),
                naming.CreateValidationSectionIdentifier("plate brace", "traceability")
            };

            string[] secondRun =
            {
                naming.CreateFeatureName("frame", "layout", "primary"),
                naming.CreateMateName("pivot", "primary"),
                naming.CreateHeightConfigurationName(680.0m),
                naming.CreateExportArtifactName("dxf", "plate set"),
                naming.CreateValidationSectionIdentifier("plate brace", "traceability")
            };

            for (int i = 0; i < firstRun.Length; i++)
            {
                AssertEqual(firstRun[i], secondRun[i], "Deterministic naming mismatch between repeated runs at index " + i.ToString(CultureInfo.InvariantCulture) + ".");
            }
        }

        private static void NamingRules_ComplianceValidation_VerifiesEntityGrammar()
        {
            DeterministicNamingService naming = new DeterministicNamingService();

            AssertTrue(naming.IsCompliantName("AXF_FRM_LAYOUT_PRIMARY", NamingEntityType.Feature), "Expected compliant feature name.");
            AssertTrue(naming.IsCompliantName("AXF_MATE_PVT_PRIMARY", NamingEntityType.Mate), "Expected compliant mate name.");
            AssertTrue(naming.IsCompliantName("AXF_CFG_HGT_680", NamingEntityType.Configuration), "Expected compliant configuration name.");
            AssertTrue(naming.IsCompliantName("AXF_EXP_DXF_PLATE_SET", NamingEntityType.ExportArtifact), "Expected compliant export name.");
            AssertTrue(naming.IsCompliantName("AXF_VAL_PLT_TRACEABILITY", NamingEntityType.ValidationSection), "Expected compliant validation name.");

            AssertFalse(naming.IsCompliantName("axf_frm_layout_primary", NamingEntityType.Feature), "Lowercase names should be rejected.");
            AssertFalse(naming.IsCompliantName("AXF_FRM_LAYOUT__PRIMARY", NamingEntityType.Feature), "Duplicate underscores should be rejected.");
            AssertFalse(naming.IsCompliantName("AXF_MATE_PVT_PRIMARY", NamingEntityType.Feature), "Entity-type mismatch should be rejected.");
        }

        private static void NamingRules_RequiredHooks_IncludeDocumentedTraceabilityHooks()
        {
            DeterministicNamingService naming = new DeterministicNamingService();
            IDictionary<string, string> hooks = naming.GetRequiredStableHooks();

            AssertEqual(12, hooks.Count, "Required stable hook count mismatch.");
            AssertTrue(hooks.ContainsKey("FRM-001"), "Required hook FRM-001 is missing.");
            AssertTrue(hooks.ContainsKey("PVT-003"), "Required hook PVT-003 is missing.");
            AssertTrue(hooks.ContainsKey("HGT-002"), "Required hook HGT-002 is missing.");
            AssertTrue(hooks.ContainsKey("PLT-003"), "Required hook PLT-003 is missing.");
            AssertEqual("AXF_FRM_LAYOUT_PRIMARY", hooks["FRM-001"], "Unexpected FRM-001 hook value.");
            AssertEqual("AXF_MATE_PVT_PRIMARY", hooks["PVT-003"], "Unexpected PVT-003 hook value.");
            AssertEqual("AXF_CFG_HEIGHT_INDEXED", hooks["HGT-002"], "Unexpected HGT-002 hook value.");
            AssertEqual("AXF_PLT_*", hooks["PLT-003"], "Unexpected PLT-003 hook value.");

            AssertEqual("AXF_FRM_LAYOUT_PRIMARY", naming.GetRequiredStableHook("FRM-001"), "Unexpected direct stable-hook lookup for FRM-001.");
            AssertThrows<ArgumentException>(
                delegate { naming.GetRequiredStableHook("UNKNOWN-RULE"); },
                "Unknown stable-hook ids must throw ArgumentException.");
        }

        private static void NamingRules_ModuleSurface_ProducesDocumentedStableHooks()
        {
            DeterministicNamingService naming = new DeterministicNamingService();
            FrameModule frameModule = new FrameModule(naming);
            PivotModule pivotModule = new PivotModule(naming);
            HeightAdjustModule heightModule = new HeightAdjustModule(naming);
            PlateBraceModule plateBraceModule = new PlateBraceModule(naming);

            IList<string> frameNames = frameModule.GetDeterministicFeatureNames();
            AssertEqual(2, frameNames.Count, "Unexpected frame deterministic name count.");
            AssertEqual("AXF_FRM_LAYOUT_PRIMARY", frameNames[0], "Unexpected frame layout hook.");
            AssertEqual("AXF_FRM_PROFILE_MAIN", frameNames[1], "Unexpected frame profile hook.");
            AssertTrue(naming.IsCompliantName(frameNames[0], NamingEntityType.Feature), "Frame layout hook should be grammar compliant.");
            AssertTrue(naming.IsCompliantName(frameNames[1], NamingEntityType.Feature), "Frame profile hook should be grammar compliant.");

            IList<string> pivotNames = pivotModule.GetDeterministicIdentifiers();
            AssertEqual(3, pivotNames.Count, "Unexpected pivot deterministic name count.");
            AssertEqual("AXF_PVT_JOINT_PRIMARY", pivotNames[0], "Unexpected pivot joint hook.");
            AssertEqual("AXF_PVT_HOLE_PATTERN", pivotNames[1], "Unexpected pivot hole hook.");
            AssertEqual("AXF_MATE_PVT_PRIMARY", pivotNames[2], "Unexpected pivot mate hook.");
            AssertTrue(naming.IsCompliantName(pivotNames[0], NamingEntityType.Feature), "Pivot joint hook should be grammar compliant.");
            AssertTrue(naming.IsCompliantName(pivotNames[1], NamingEntityType.Feature), "Pivot hole hook should be grammar compliant.");
            AssertTrue(naming.IsCompliantName(pivotNames[2], NamingEntityType.Mate), "Pivot mate hook should be grammar compliant.");

            IList<string> heightConfigNames = heightModule.CreateSupportedConfigurationNames(new List<decimal> { 680.0m, 730.0m, 780.0m });
            AssertEqual(3, heightConfigNames.Count, "Unexpected supported-height configuration name count.");
            AssertEqual("AXF_CFG_HGT_680", heightConfigNames[0], "Unexpected 680mm configuration hook.");
            AssertEqual("AXF_CFG_HGT_730", heightConfigNames[1], "Unexpected 730mm configuration hook.");
            AssertEqual("AXF_CFG_HGT_780", heightConfigNames[2], "Unexpected 780mm configuration hook.");
            AssertTrue(naming.IsCompliantName(heightConfigNames[0], NamingEntityType.Configuration), "680mm configuration hook should be grammar compliant.");
            AssertTrue(naming.IsCompliantName(heightConfigNames[1], NamingEntityType.Configuration), "730mm configuration hook should be grammar compliant.");
            AssertTrue(naming.IsCompliantName(heightConfigNames[2], NamingEntityType.Configuration), "780mm configuration hook should be grammar compliant.");
            AssertEqual("AXF_CFG_HEIGHT_INDEXED", heightModule.GetIndexedActivationHook(), "Unexpected indexed activation hook.");
            AssertEqual("AXF_HGT_VALIDATION_SET", heightModule.GetValidationSetHook(), "Unexpected height validation hook.");

            IList<string> plateBraceNames = plateBraceModule.GetDeterministicIdentifiers();
            AssertEqual(4, plateBraceNames.Count, "Unexpected plate/brace deterministic name count.");
            AssertEqual("AXF_PLT_BRACE_PRIMARY", plateBraceNames[0], "Unexpected plate/brace primary hook.");
            AssertEqual("AXF_PLT_EXPORT_DXF", plateBraceNames[1], "Unexpected plate/brace DXF traceability hook.");
            AssertEqual("AXF_EXP_DXF_PLATE_SET", plateBraceNames[2], "Unexpected DXF export artifact hook.");
            AssertEqual("AXF_VAL_PLT_TRACEABILITY", plateBraceNames[3], "Unexpected plate/brace validation section hook.");
            AssertTrue(naming.IsCompliantName(plateBraceNames[0], NamingEntityType.Feature), "Plate/brace primary hook should be grammar compliant.");
            AssertTrue(naming.IsCompliantName(plateBraceNames[1], NamingEntityType.Feature), "Plate/brace DXF traceability hook should be grammar compliant.");
            AssertTrue(naming.IsCompliantName(plateBraceNames[2], NamingEntityType.ExportArtifact), "DXF export artifact hook should be grammar compliant.");
            AssertTrue(naming.IsCompliantName(plateBraceNames[3], NamingEntityType.ValidationSection), "Plate/brace validation section hook should be grammar compliant.");
        }

        private static void NamingRules_ModuleSurface_RepeatedOutputsRemainStable()
        {
            DeterministicNamingService naming = new DeterministicNamingService();

            string[] firstRun = BuildModuleSurfaceNamingSnapshot(naming);
            string[] secondRun = BuildModuleSurfaceNamingSnapshot(naming);

            AssertEqual(firstRun.Length, secondRun.Length, "Repeated naming snapshots must have identical lengths.");
            for (int i = 0; i < firstRun.Length; i++)
            {
                AssertEqual(firstRun[i], secondRun[i], "Repeated naming snapshots differ at index " + i.ToString(CultureInfo.InvariantCulture) + ".");
            }
        }

        private static void FrameBehavior_BuildOutput_UsesDocumentedBaselineRules()
        {
            ConfigurationProcessingResult validationResult = Execute(BaselineJson());
            FrameModule frameModule = new FrameModule(new DeterministicNamingService());

            FrameBuildOutput output = frameModule.CreateBuildOutput(validationResult.NormalizedConfig);
            AssertEqual(2, output.FeatureNames.Count, "Unexpected frame feature count.");
            AssertEqual("AXF_FRM_LAYOUT_PRIMARY", output.FeatureNames[0], "Unexpected frame layout feature.");
            AssertEqual("AXF_FRM_PROFILE_MAIN", output.FeatureNames[1], "Unexpected frame profile feature.");

            AssertEqual(620.0m, output.MemberExtentMin, "Unexpected frame member extent minimum.");
            AssertEqual(980.0m, output.MemberExtentMax, "Unexpected frame member extent maximum.");
            AssertEqual(0.5m, output.PlacementTolerance, "Unexpected frame placement tolerance.");
            AssertEqual(0.2m, output.ProfileDimensionTolerance, "Unexpected frame profile dimension tolerance.");
            AssertEqual(DeterministicNamingService.RuleSetStandardV1, output.NamingRuleSet, "Unexpected frame naming rule set.");

            AssertEqual(2, output.AllowedProfiles.Count, "Unexpected frame profile count.");
            AssertEqual("40x40x2.0_SHS", output.AllowedProfiles[0], "Unexpected first baseline profile.");
            AssertEqual("60x30x2.0_RHS", output.AllowedProfiles[1], "Unexpected second baseline profile.");
            AssertEqual("40x40x2.0_SHS", output.SelectedProfileCode, "Unexpected selected frame profile.");
            AssertFalse(output.GeometryCreated, "Frame geometry should be disabled in non-SolidWorks test runtime.");
            AssertEqual(string.Empty, output.ActiveDocumentName, "Active SolidWorks document name should be empty when geometry is disabled.");
            AssertEqual("Frame geometry executor is not configured for this runtime.", output.GeometryExecutionNote, "Unexpected geometry execution note.");

            AssertEqual(3, output.TracePoints.Count, "Unexpected frame trace-point count.");
            AssertEqual("frame.layout", output.TracePoints[0], "Unexpected frame layout trace point.");
            AssertEqual("frame.members", output.TracePoints[1], "Unexpected frame members trace point.");
            AssertEqual("traceability.naming.frame", output.TracePoints[2], "Unexpected frame naming trace point.");
        }

        private static void FrameBehavior_BuildOutput_RepeatedOutputsRemainStable()
        {
            ConfigurationProcessingResult validationResult = Execute(BaselineJson());
            FrameModule frameModule = new FrameModule(new DeterministicNamingService());

            FrameBuildOutput firstOutput = frameModule.CreateBuildOutput(validationResult.NormalizedConfig);
            FrameBuildOutput secondOutput = frameModule.CreateBuildOutput(validationResult.NormalizedConfig);

            AssertEqual(firstOutput.FeatureNames.Count, secondOutput.FeatureNames.Count, "Frame feature count should remain stable across repeated runs.");
            AssertEqual(firstOutput.AllowedProfiles.Count, secondOutput.AllowedProfiles.Count, "Frame profile count should remain stable across repeated runs.");
            AssertEqual(firstOutput.TracePoints.Count, secondOutput.TracePoints.Count, "Frame trace-point count should remain stable across repeated runs.");
            AssertEqual(firstOutput.MemberExtentMin, secondOutput.MemberExtentMin, "Frame member extent minimum should remain stable across repeated runs.");
            AssertEqual(firstOutput.MemberExtentMax, secondOutput.MemberExtentMax, "Frame member extent maximum should remain stable across repeated runs.");
            AssertEqual(firstOutput.PlacementTolerance, secondOutput.PlacementTolerance, "Frame placement tolerance should remain stable across repeated runs.");
            AssertEqual(firstOutput.ProfileDimensionTolerance, secondOutput.ProfileDimensionTolerance, "Frame profile tolerance should remain stable across repeated runs.");
            AssertEqual(firstOutput.NamingRuleSet, secondOutput.NamingRuleSet, "Frame naming rule-set should remain stable across repeated runs.");
            AssertEqual(firstOutput.SelectedProfileCode, secondOutput.SelectedProfileCode, "Selected frame profile should remain stable across repeated runs.");
            AssertEqual(firstOutput.GeometryCreated, secondOutput.GeometryCreated, "Frame geometry enabled flag should remain stable across repeated runs.");
            AssertEqual(firstOutput.ActiveDocumentName, secondOutput.ActiveDocumentName, "Frame geometry active-document output should remain stable across repeated runs.");
            AssertEqual(firstOutput.GeometryExecutionNote, secondOutput.GeometryExecutionNote, "Frame geometry execution note should remain stable across repeated runs.");

            for (int i = 0; i < firstOutput.FeatureNames.Count; i++)
            {
                AssertEqual(firstOutput.FeatureNames[i], secondOutput.FeatureNames[i], "Frame feature output mismatch at index " + i.ToString(CultureInfo.InvariantCulture) + ".");
            }

            for (int i = 0; i < firstOutput.AllowedProfiles.Count; i++)
            {
                AssertEqual(firstOutput.AllowedProfiles[i], secondOutput.AllowedProfiles[i], "Frame profile output mismatch at index " + i.ToString(CultureInfo.InvariantCulture) + ".");
            }

            for (int i = 0; i < firstOutput.TracePoints.Count; i++)
            {
                AssertEqual(firstOutput.TracePoints[i], secondOutput.TracePoints[i], "Frame trace-point output mismatch at index " + i.ToString(CultureInfo.InvariantCulture) + ".");
            }
        }

        private static void BuildLifecycle_Metadata_IsPopulated()
        {
            string outputRootPath = CreateTempOutputRootPath();
            try
            {
                BuildExecutionResult result = ExecuteBuildWorkflow(BaselineJson(), outputRootPath);

                AssertTrue(result != null, "Build result should not be null.");
                AssertTrue(!string.IsNullOrWhiteSpace(result.Metadata.RunId), "run-id must be populated.");
                AssertTrue(result.Metadata.TimestampUtc != default(DateTime), "timestamp must be populated.");
                AssertTrue(!string.IsNullOrWhiteSpace(result.Metadata.ConfigPath), "config path must be populated.");
                AssertTrue(!string.IsNullOrWhiteSpace(result.Metadata.ConfigHash), "config hash must be populated.");
                AssertTrue(result.StageRecords.Count == 5, "Expected five Build stages.");
            }
            finally
            {
                DeleteDirectoryIfExists(outputRootPath);
            }
        }

        private static void BuildLifecycle_ConfigHash_IsStableForIdenticalInputs()
        {
            string outputRootPath = CreateTempOutputRootPath();
            try
            {
                BuildExecutionResult firstResult = ExecuteBuildWorkflow(BaselineJson(), outputRootPath);
                BuildExecutionResult secondResult = ExecuteBuildWorkflow(BaselineJson(), outputRootPath);

                AssertEqual(firstResult.Metadata.ConfigHash, secondResult.Metadata.ConfigHash, "Config hash must remain stable for identical normalized inputs.");
            }
            finally
            {
                DeleteDirectoryIfExists(outputRootPath);
            }
        }

        private static void BuildLifecycle_StageOrder_IsDeterministic()
        {
            string outputRootPath = CreateTempOutputRootPath();
            try
            {
                BuildExecutionResult result = ExecuteBuildWorkflow(BaselineJson(), outputRootPath);
                AssertBuildStageOrder(result.StageRecords);
            }
            finally
            {
                DeleteDirectoryIfExists(outputRootPath);
            }
        }

        private static void BuildLifecycle_GeneratePartsStage_RecordsFrameTraceabilityDetails()
        {
            string outputRootPath = CreateTempOutputRootPath();
            try
            {
                BuildExecutionResult result = ExecuteBuildWorkflow(BaselineJson(), outputRootPath);
                BuildStageRecord generatePartsStage = FindStage(result.StageRecords, "generate parts");

                AssertTrue(generatePartsStage != null, "Generate-parts stage record should exist.");
                AssertTrue(!string.IsNullOrWhiteSpace(generatePartsStage.Details), "Generate-parts stage details should be populated.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameExtentMin=620", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame extent minimum.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameExtentMax=980", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame extent maximum.");
                AssertTrue(generatePartsStage.Details.IndexOf("framePlacementTolerance=0.5", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame placement tolerance.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameProfileTolerance=0.2", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame profile tolerance.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameProfiles=40x40x2.0_SHS|60x30x2.0_RHS", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include baseline frame profiles.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameSelectedProfile=40x40x2.0_SHS", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include deterministic frame-profile selection.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameNamingRuleSet=AXF_STANDARD_V1", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame naming rule-set.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameTracePoints=frame.layout|frame.members|traceability.naming.frame", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame trace points.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameGeometryCreated=false", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame geometry generation status.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameGeometryDoc=<null>", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame geometry document placeholder.");
                AssertTrue(generatePartsStage.Details.IndexOf("frameGeometryNote=Frame geometry executor is not configured for this runtime.", StringComparison.Ordinal) >= 0, "Generate-parts stage details should include frame geometry execution note.");
            }
            finally
            {
                DeleteDirectoryIfExists(outputRootPath);
            }
        }

        private static void BuildLifecycle_SummaryArtifacts_AreWritten()
        {
            string outputRootPath = CreateTempOutputRootPath();
            try
            {
                BuildExecutionResult result = ExecuteBuildWorkflow(BaselineJson(), outputRootPath);

                AssertTrue(result.IsSuccessful, "Baseline Build should complete successfully.");
                AssertTrue(!string.IsNullOrWhiteSpace(result.LogFilePath), "Log file path should be populated.");
                AssertTrue(!string.IsNullOrWhiteSpace(result.SummaryFilePath), "Summary file path should be populated.");
                AssertTrue(File.Exists(result.LogFilePath), "Build log should be written to disk.");
                AssertTrue(File.Exists(result.SummaryFilePath), "Build summary should be written to disk.");

                string logContent = File.ReadAllText(result.LogFilePath);
                AssertTrue(logContent.IndexOf("run-id=" + result.Metadata.RunId, StringComparison.Ordinal) >= 0, "Build log should contain run-id.");
                AssertTrue(logContent.IndexOf("1. load", StringComparison.Ordinal) >= 0, "Build log should contain load stage.");
                AssertTrue(logContent.IndexOf("5. summary", StringComparison.Ordinal) >= 0, "Build log should contain summary stage.");
            }
            finally
            {
                DeleteDirectoryIfExists(outputRootPath);
            }
        }

        private static void BuildLifecycle_Regression_ThreeConsecutiveRuns_NoUnhandledExceptions()
        {
            string outputRootPath = CreateTempOutputRootPath();
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    BuildExecutionResult result = ExecuteBuildWorkflow(BaselineJson(), outputRootPath);
                    AssertTrue(result != null, "Build result should not be null in regression run " + i.ToString(CultureInfo.InvariantCulture) + ".");
                    AssertTrue(!result.HasFailedStages, "Build stages should not fail in regression run " + i.ToString(CultureInfo.InvariantCulture) + ".");
                    AssertTrue(result.IsSuccessful, "Build should complete successfully in regression run " + i.ToString(CultureInfo.InvariantCulture) + ".");
                    AssertBuildStageOrder(result.StageRecords);
                }
            }
            finally
            {
                DeleteDirectoryIfExists(outputRootPath);
            }
        }

        private static string[] BuildModuleSurfaceNamingSnapshot(DeterministicNamingService naming)
        {
            FrameModule frameModule = new FrameModule(naming);
            PivotModule pivotModule = new PivotModule(naming);
            HeightAdjustModule heightModule = new HeightAdjustModule(naming);
            PlateBraceModule plateBraceModule = new PlateBraceModule(naming);

            List<string> snapshot = new List<string>();
            AddRange(snapshot, frameModule.GetDeterministicFeatureNames());
            AddRange(snapshot, pivotModule.GetDeterministicIdentifiers());
            AddRange(snapshot, heightModule.CreateSupportedConfigurationNames(new List<decimal> { 680.0m, 730.0m, 780.0m }));
            snapshot.Add(heightModule.GetIndexedActivationHook());
            snapshot.Add(heightModule.GetValidationSetHook());
            AddRange(snapshot, plateBraceModule.GetDeterministicIdentifiers());
            return snapshot.ToArray();
        }

        private static void AddRange(IList<string> target, IList<string> source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                target.Add(source[i]);
            }
        }

        private static BuildExecutionResult ExecuteBuildWorkflow(string configJson, string outputRootPath)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), "axionframe-build-config-" + Guid.NewGuid().ToString("N") + ".json");
            File.WriteAllText(tempFilePath, configJson);

            try
            {
                BuildWorkflowEngine workflowEngine = new BuildWorkflowEngine();
                return workflowEngine.ExecuteBuild(tempFilePath, outputRootPath);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        private static string CreateTempOutputRootPath()
        {
            string path = Path.Combine(Path.GetTempPath(), "axionframe-build-output-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return path;
        }

        private static void DeleteDirectoryIfExists(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static void AssertBuildStageOrder(IList<BuildStageRecord> stages)
        {
            AssertEqual(5, stages.Count, "Unexpected Build stage count.");
            AssertEqual("load", stages[0].StageName, "Unexpected stage at index 0.");
            AssertEqual("validate", stages[1].StageName, "Unexpected stage at index 1.");
            AssertEqual("generate parts", stages[2].StageName, "Unexpected stage at index 2.");
            AssertEqual("generate assembly", stages[3].StageName, "Unexpected stage at index 3.");
            AssertEqual("summary", stages[4].StageName, "Unexpected stage at index 4.");
        }

        private static BuildStageRecord FindStage(IList<BuildStageRecord> stages, string stageName)
        {
            for (int i = 0; i < stages.Count; i++)
            {
                if (string.Equals(stages[i].StageName, stageName, StringComparison.Ordinal))
                {
                    return stages[i];
                }
            }

            return null;
        }

        private static ConfigurationProcessingResult Execute(string json)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), "axionframe-config-test-" + Guid.NewGuid().ToString("N") + ".json");
            File.WriteAllText(tempFilePath, json);

            try
            {
                FeatureManager manager = new FeatureManager();
                return manager.LoadAndValidate(tempFilePath);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        private static void Run(string name, Action test)
        {
            _runCount++;
            try
            {
                test();
                Console.WriteLine("[PASS] " + name);
            }
            catch (Exception ex)
            {
                _failureCount++;
                Console.WriteLine("[FAIL] " + name + " :: " + ex.Message);
            }
        }

        private static ValidationMessage FindMessage(IList<ValidationMessage> messages, string validatorId, string configKey)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                ValidationMessage message = messages[i];
                if (string.Equals(message.ValidatorId, validatorId, StringComparison.Ordinal) &&
                    string.Equals(message.ConfigKey, configKey, StringComparison.Ordinal))
                {
                    return message;
                }
            }

            return null;
        }

        private static void AssertHasContractFields(ValidationMessage message)
        {
            AssertTrue(!string.IsNullOrWhiteSpace(message.RuleId), "ruleId must be set.");
            AssertTrue(!string.IsNullOrWhiteSpace(message.ValidatorId), "validatorId must be set.");
            AssertTrue(!string.IsNullOrWhiteSpace(message.Message), "message must be set.");
            AssertTrue(!string.IsNullOrWhiteSpace(message.Expected), "expected must be set.");
            AssertTrue(!string.IsNullOrWhiteSpace(message.Actual), "actual must be set.");
            AssertTrue(!string.IsNullOrWhiteSpace(message.RecommendedAction), "recommendedAction must be set.");
        }

        private static void AssertKeyExists(IDictionary<string, object> dictionary, string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                throw new InvalidOperationException("Expected key not found: " + key);
            }
        }

        private static void AssertTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void AssertFalse(bool condition, string message)
        {
            AssertTrue(!condition, message);
        }

        private static void AssertEqual<T>(T expected, T actual, string message)
        {
            if (!Equals(expected, actual))
            {
                throw new InvalidOperationException(message + " Expected: " + expected + " Actual: " + actual);
            }
        }

        private static void AssertThrows<TException>(Action action, string message) where TException : Exception
        {
            bool thrown = false;

            try
            {
                action();
            }
            catch (TException)
            {
                thrown = true;
            }

            if (!thrown)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static string BaselineJson()
        {
            return
@"{
  ""frame"": {
    ""layout"": {
      ""primary"": {
        ""memberExtentMin"": 620.0,
        ""memberExtentMax"": 980.0,
        ""placementTolerance"": 0.5
      }
    },
    ""profile"": {
      ""selection"": {
        ""allowedProfiles"": [
          ""40x40x2.0_SHS"",
          ""60x30x2.0_RHS""
        ],
        ""dimensionTolerance"": 0.2
      }
    },
    ""naming"": {
      ""ruleSet"": ""AXF_STANDARD_V1""
    }
  },
  ""pivot"": {
    ""geometry"": {
      ""primary"": {
        ""axisLocationMin"": 300.0,
        ""axisLocationMax"": 450.0,
        ""alignmentTolerance"": 0.25
      }
    },
    ""hole"": {
      ""strategy"": {
        ""diameterMin"": 10.5,
        ""diameterMax"": 11.0,
        ""positionTolerance"": 0.2
      }
    },
    ""naming"": {
      ""mates"": ""AXF_STANDARD_V1""
    }
  },
  ""height"": {
    ""supportedConfigurations"": {
      ""values"": [
        680.0,
        730.0,
        780.0
      ]
    },
    ""indexing"": {
      ""activation"": {
        ""requiredCount"": 3,
        ""strictDeterminism"": true
      }
    },
    ""validation"": {
      ""supportedSet"": [
        680.0,
        730.0,
        780.0
      ],
      ""dimensionTolerance"": 1.0
    }
  },
  ""plateBrace"": {
    ""dimensions"": {
      ""primary"": {
        ""thicknessMin"": 5.0,
        ""thicknessMax"": 8.0,
        ""dimensionTolerance"": 0.2
      }
    },
    ""export"": {
      ""dxfEligible"": true
    },
    ""naming"": {
      ""ruleSet"": ""AXF_STANDARD_V1""
    }
  },
  ""exports"": {
    ""step"": {
      ""enabled"": true
    },
    ""dxf"": {
      ""enabled"": true
    },
    ""bom"": {
      ""enabled"": true
    },
    ""validationReport"": {
      ""enabled"": true
    }
  },
  ""validation"": {
    ""mode"": ""StrictRelease"",
    ""stopOnCriticalFailure"": true
  },
  ""run"": {
    ""packageOutputs"": true
  }
}";
        }
    }
}
