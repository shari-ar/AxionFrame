using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AxionFrame
{
    public sealed class BuildWorkflowEngine
    {
        private const string DefaultConfigDirectoryName = "Config";
        private const string DefaultConfigFileName = "GlobalParams.json";
        private const string DefaultOutputDirectoryName = "Output";
        private const string SupportedHeightsConfigKey = "height.supportedConfigurations.values";
        private const string ConfigHashUnavailable = "UNAVAILABLE";
        private const string NullValueToken = "<null>";
        private const string RunIdPrefix = "AXF-RUN";
        private const int RunIdRandomTokenLength = 8;
        private const int ConfigDiscoveryMaxDepth = 8;

        private readonly FeatureManager _featureManager;
        private readonly FrameModule _frameModule;
        private readonly PivotModule _pivotModule;
        private readonly HeightAdjustModule _heightAdjustModule;
        private readonly PlateBraceModule _plateBraceModule;

        public BuildWorkflowEngine()
            : this(
                  new FeatureManager(),
                  new FrameModule(),
                  new PivotModule(),
                  new HeightAdjustModule(),
                  new PlateBraceModule())
        {
        }

        public BuildWorkflowEngine(
            FeatureManager featureManager,
            FrameModule frameModule,
            PivotModule pivotModule,
            HeightAdjustModule heightAdjustModule,
            PlateBraceModule plateBraceModule)
        {
            if (featureManager == null)
            {
                throw new ArgumentNullException(nameof(featureManager));
            }

            if (frameModule == null)
            {
                throw new ArgumentNullException(nameof(frameModule));
            }

            if (pivotModule == null)
            {
                throw new ArgumentNullException(nameof(pivotModule));
            }

            if (heightAdjustModule == null)
            {
                throw new ArgumentNullException(nameof(heightAdjustModule));
            }

            if (plateBraceModule == null)
            {
                throw new ArgumentNullException(nameof(plateBraceModule));
            }

            _featureManager = featureManager;
            _frameModule = frameModule;
            _pivotModule = pivotModule;
            _heightAdjustModule = heightAdjustModule;
            _plateBraceModule = plateBraceModule;
        }

        public BuildExecutionResult ExecuteBuild(string configPath, string outputRootPath)
        {
            // Keep stage execution order fixed for deterministic audit logs and repeatable validation evidence.
            string resolvedConfigPath = ResolveConfigurationPath(configPath);
            string resolvedOutputRoot = ResolveOutputRoot(outputRootPath, resolvedConfigPath);
            DateTime runTimestampUtc = DateTime.UtcNow;
            string runId = CreateRunId(runTimestampUtc);

            ConfigurationProcessingResult validationResult = null;
            string configHash = ConfigHashUnavailable;
            List<BuildStageRecord> stages = new List<BuildStageRecord>();
            List<string> partArtifacts = new List<string>();
            List<string> assemblyArtifacts = new List<string>();

            ExecuteStage(
                stages,
                BuildStageKind.Load,
                delegate(StringBuilder details)
                {
                    details.Append("configPath=").Append(resolvedConfigPath);
                    details.Append("; source=").Append(File.Exists(resolvedConfigPath) ? "file" : "defaults");
                });

            Exception validationException = ExecuteStage(
                stages,
                BuildStageKind.Validate,
                delegate(StringBuilder details)
                {
                    validationResult = _featureManager.LoadAndValidate(resolvedConfigPath);
                    if (validationResult != null)
                    {
                        configHash = ComputeConfigHash(validationResult.NormalizedConfig);
                        details.Append("isValid=").Append(validationResult.IsValid ? "true" : "false");
                        details.Append("; hasBlockingFailures=").Append(validationResult.HasBlockingFailures ? "true" : "false");
                        details.Append("; messageCount=").Append(validationResult.Messages.Count.ToString(CultureInfo.InvariantCulture));
                    }
                });

            bool canGenerate = validationException == null && validationResult != null && !validationResult.HasBlockingFailures;
            if (canGenerate)
            {
                ExecuteStage(
                    stages,
                    BuildStageKind.GenerateParts,
                    delegate(StringBuilder details)
                    {
                        AddRange(partArtifacts, _frameModule.GetDeterministicFeatureNames());
                        partArtifacts.Add(_pivotModule.GetJointPrimaryFeatureName());
                        partArtifacts.Add(_pivotModule.GetHolePatternFeatureName());
                        partArtifacts.Add(_plateBraceModule.GetBracePrimaryFeatureName());
                        partArtifacts.Add(_plateBraceModule.GetDxfTraceabilityFeatureName());

                        details.Append("partArtifacts=").Append(partArtifacts.Count.ToString(CultureInfo.InvariantCulture));
                    });

                ExecuteStage(
                    stages,
                    BuildStageKind.GenerateAssembly,
                    delegate(StringBuilder details)
                    {
                        assemblyArtifacts.Add(_pivotModule.GetPrimaryMateName());

                        IList<decimal> supportedHeights = GetSupportedHeights(validationResult);
                        AddRange(assemblyArtifacts, _heightAdjustModule.CreateSupportedConfigurationNames(supportedHeights));
                        assemblyArtifacts.Add(_heightAdjustModule.GetIndexedActivationHook());
                        assemblyArtifacts.Add(_plateBraceModule.GetTraceabilityValidationSectionIdentifier());

                        details.Append("assemblyArtifacts=").Append(assemblyArtifacts.Count.ToString(CultureInfo.InvariantCulture));
                    });
            }
            else
            {
                AppendSkippedStage(
                    stages,
                    BuildStageKind.GenerateParts,
                    validationException != null ? "Validation stage failed before parts generation." : "Blocking validation failures prevented parts generation.");
                AppendSkippedStage(
                    stages,
                    BuildStageKind.GenerateAssembly,
                    validationException != null ? "Validation stage failed before assembly generation." : "Blocking validation failures prevented assembly generation.");
            }

            BuildRunMetadata metadata = new BuildRunMetadata(runId, runTimestampUtc, resolvedConfigPath, configHash);
            BuildExecutionResult result = new BuildExecutionResult(metadata, validationResult, stages, partArtifacts, assemblyArtifacts);
            BuildStageRecord summaryStage = StartStage(stages, BuildStageKind.Summary);

            string plannedRunDirectory = Path.Combine(resolvedOutputRoot, runId);
            string plannedLogFilePath = Path.Combine(plannedRunDirectory, BuildArtifactConstants.BuildLogFileName);
            string plannedSummaryFilePath = Path.Combine(plannedRunDirectory, BuildArtifactConstants.BuildSummaryFileName);

            BuildArtifactWriter artifactWriter = new BuildArtifactWriter(resolvedOutputRoot);
            try
            {
                summaryStage.SetOutcome(
                    DateTime.UtcNow,
                    BuildStageStatus.Succeeded,
                    "outputRoot=" + resolvedOutputRoot + "; runDirectory=" + plannedRunDirectory + "; logFile=" + plannedLogFilePath + "; summaryFile=" + plannedSummaryFilePath);
                artifactWriter.Persist(result);
            }
            catch (Exception ex)
            {
                summaryStage.SetOutcome(
                    DateTime.UtcNow,
                    BuildStageStatus.Failed,
                    "outputRoot=" + resolvedOutputRoot + "; errorType=" + ex.GetType().Name + "; errorMessage=" + ex.Message);
            }

            return result;
        }

        public static string ResolveConfigurationPath(string configPath)
        {
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                return Path.GetFullPath(configPath);
            }

            string discoveredPath = FindConfigurationPathFromRoot(AppDomain.CurrentDomain.BaseDirectory);
            if (!string.IsNullOrWhiteSpace(discoveredPath))
            {
                return discoveredPath;
            }

            discoveredPath = FindConfigurationPathFromRoot(Environment.CurrentDirectory);
            if (!string.IsNullOrWhiteSpace(discoveredPath))
            {
                return discoveredPath;
            }

            return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, DefaultConfigDirectoryName, DefaultConfigFileName));
        }

        public static string ResolveOutputRoot(string outputRootPath, string resolvedConfigPath)
        {
            if (!string.IsNullOrWhiteSpace(outputRootPath))
            {
                return Path.GetFullPath(outputRootPath);
            }

            if (!string.IsNullOrWhiteSpace(resolvedConfigPath))
            {
                string configDirectory = Path.GetDirectoryName(resolvedConfigPath);
                if (!string.IsNullOrWhiteSpace(configDirectory))
                {
                    DirectoryInfo directory = Directory.GetParent(configDirectory);
                    if (directory != null)
                    {
                        return Path.GetFullPath(Path.Combine(directory.FullName, DefaultOutputDirectoryName));
                    }
                }
            }

            return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, DefaultOutputDirectoryName));
        }

        public static string ComputeConfigHash(IDictionary<string, object> normalizedConfig)
        {
            string canonicalPayload = BuildCanonicalConfigPayload(normalizedConfig);
            using (SHA256 hashAlgorithm = SHA256.Create())
            {
                byte[] payloadBytes = Encoding.UTF8.GetBytes(canonicalPayload);
                byte[] hashBytes = hashAlgorithm.ComputeHash(payloadBytes);
                StringBuilder hashText = new StringBuilder(hashBytes.Length * 2);
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashText.Append(hashBytes[i].ToString("x2", CultureInfo.InvariantCulture));
                }

                return hashText.ToString();
            }
        }

        private static Exception ExecuteStage(IList<BuildStageRecord> stages, BuildStageKind stageKind, Action<StringBuilder> action)
        {
            BuildStageRecord stage = StartStage(stages, stageKind);
            StringBuilder details = new StringBuilder();

            try
            {
                action(details);
                stage.SetOutcome(DateTime.UtcNow, BuildStageStatus.Succeeded, details.ToString());
                return null;
            }
            catch (Exception ex)
            {
                if (details.Length > 0)
                {
                    details.Append("; ");
                }

                details.Append("errorType=").Append(ex.GetType().Name);
                details.Append("; errorMessage=").Append(ex.Message);
                stage.SetOutcome(DateTime.UtcNow, BuildStageStatus.Failed, details.ToString());
                return ex;
            }
        }

        private static BuildStageRecord StartStage(IList<BuildStageRecord> stages, BuildStageKind stageKind)
        {
            BuildStageRecord stage = new BuildStageRecord(stages.Count + 1, stageKind, DateTime.UtcNow);
            stages.Add(stage);
            return stage;
        }

        private static void AppendSkippedStage(IList<BuildStageRecord> stages, BuildStageKind stageKind, string reason)
        {
            BuildStageRecord stage = StartStage(stages, stageKind);
            stage.SetOutcome(DateTime.UtcNow, BuildStageStatus.Skipped, reason);
        }

        private static void AddRange(IList<string> target, IList<string> source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                target.Add(source[i]);
            }
        }

        private static IList<decimal> GetSupportedHeights(ConfigurationProcessingResult validationResult)
        {
            List<decimal> supportedHeights = new List<decimal>();
            if (validationResult == null || validationResult.NormalizedConfig == null)
            {
                return supportedHeights;
            }

            object values;
            if (!validationResult.NormalizedConfig.TryGetValue(SupportedHeightsConfigKey, out values))
            {
                return supportedHeights;
            }

            List<decimal> configuredValues = values as List<decimal>;
            if (configuredValues == null)
            {
                return supportedHeights;
            }

            for (int i = 0; i < configuredValues.Count; i++)
            {
                supportedHeights.Add(configuredValues[i]);
            }

            return supportedHeights;
        }

        private static string BuildCanonicalConfigPayload(IDictionary<string, object> normalizedConfig)
        {
            if (normalizedConfig == null)
            {
                return string.Empty;
            }

            List<string> keys = new List<string>();
            foreach (string key in normalizedConfig.Keys)
            {
                keys.Add(key);
            }

            keys.Sort(StringComparer.Ordinal);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                object value = normalizedConfig[key];
                builder.Append(key);
                builder.Append('=');
                builder.Append(FormatConfigValue(value));
                builder.Append(';');
            }

            return builder.ToString();
        }

        private static string FormatConfigValue(object value)
        {
            if (value == null)
            {
                return NullValueToken;
            }

            if (value is string)
            {
                return (string)value;
            }

            if (value is bool)
            {
                return ((bool)value) ? "true" : "false";
            }

            if (value is int)
            {
                return ((int)value).ToString(CultureInfo.InvariantCulture);
            }

            if (value is decimal)
            {
                return ((decimal)value).ToString("0.############################", CultureInfo.InvariantCulture);
            }

            List<string> stringValues = value as List<string>;
            if (stringValues != null)
            {
                return "[" + string.Join("|", stringValues.ToArray()) + "]";
            }

            List<decimal> decimalValues = value as List<decimal>;
            if (decimalValues != null)
            {
                string[] items = new string[decimalValues.Count];
                for (int i = 0; i < decimalValues.Count; i++)
                {
                    items[i] = decimalValues[i].ToString("0.############################", CultureInfo.InvariantCulture);
                }

                return "[" + string.Join("|", items) + "]";
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static string CreateRunId(DateTime timestampUtc)
        {
            string timestampToken = timestampUtc.ToString("yyyyMMddTHHmmssfff", CultureInfo.InvariantCulture);
            string randomToken = Guid.NewGuid().ToString("N").Substring(0, RunIdRandomTokenLength).ToUpperInvariant();
            return RunIdPrefix + "-" + timestampToken + "Z-" + randomToken;
        }

        private static string FindConfigurationPathFromRoot(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                return null;
            }

            DirectoryInfo directory = new DirectoryInfo(rootPath);
            if (!directory.Exists)
            {
                return null;
            }

            for (int i = 0; i < ConfigDiscoveryMaxDepth && directory != null; i++)
            {
                string candidatePath = Path.Combine(directory.FullName, DefaultConfigDirectoryName, DefaultConfigFileName);
                if (File.Exists(candidatePath))
                {
                    return candidatePath;
                }

                directory = directory.Parent;
            }

            return null;
        }
    }
}
