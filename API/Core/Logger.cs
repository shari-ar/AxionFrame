using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace AxionFrame
{
    public enum BuildStageKind
    {
        Load = 0,
        Validate = 1,
        GenerateParts = 2,
        GenerateAssembly = 3,
        Summary = 4
    }

    public enum BuildStageStatus
    {
        Succeeded = 0,
        Failed = 1,
        Skipped = 2
    }

    public sealed class BuildStageRecord
    {
        internal BuildStageRecord(int sequence, BuildStageKind stageKind, DateTime startedAtUtc)
        {
            Sequence = sequence;
            StageKind = stageKind;
            StageName = GetStageName(stageKind);
            StartedAtUtc = startedAtUtc;
            CompletedAtUtc = startedAtUtc;
            Status = BuildStageStatus.Skipped;
            Details = string.Empty;
        }

        public int Sequence { get; private set; }
        public BuildStageKind StageKind { get; private set; }
        public string StageName { get; private set; }
        public DateTime StartedAtUtc { get; private set; }
        public DateTime CompletedAtUtc { get; private set; }
        public BuildStageStatus Status { get; private set; }
        public string Details { get; private set; }

        internal void SetOutcome(DateTime completedAtUtc, BuildStageStatus status, string details)
        {
            CompletedAtUtc = completedAtUtc;
            Status = status;
            Details = details ?? string.Empty;
        }

        private static string GetStageName(BuildStageKind stageKind)
        {
            switch (stageKind)
            {
                case BuildStageKind.Load:
                    return "load";
                case BuildStageKind.Validate:
                    return "validate";
                case BuildStageKind.GenerateParts:
                    return "generate parts";
                case BuildStageKind.GenerateAssembly:
                    return "generate assembly";
                case BuildStageKind.Summary:
                    return "summary";
                default:
                    return "unknown";
            }
        }
    }

    public sealed class BuildRunMetadata
    {
        public BuildRunMetadata(string runId, DateTime timestampUtc, string configPath, string configHash)
        {
            RunId = runId ?? string.Empty;
            TimestampUtc = timestampUtc;
            ConfigPath = configPath ?? string.Empty;
            ConfigHash = configHash ?? string.Empty;
        }

        public string RunId { get; private set; }
        public DateTime TimestampUtc { get; private set; }
        public string ConfigPath { get; private set; }
        public string ConfigHash { get; private set; }
    }

    public sealed class BuildExecutionResult
    {
        public BuildExecutionResult(
            BuildRunMetadata metadata,
            ConfigurationProcessingResult validationResult,
            IList<BuildStageRecord> stageRecords,
            IList<string> partArtifacts,
            IList<string> assemblyArtifacts)
        {
            Metadata = metadata ?? new BuildRunMetadata(string.Empty, DateTime.UtcNow, string.Empty, string.Empty);
            ValidationResult = validationResult;
            StageRecords = stageRecords ?? new List<BuildStageRecord>();
            PartArtifacts = partArtifacts ?? new List<string>();
            AssemblyArtifacts = assemblyArtifacts ?? new List<string>();
            RunDirectoryPath = string.Empty;
            LogFilePath = string.Empty;
            SummaryFilePath = string.Empty;
        }

        public BuildRunMetadata Metadata { get; private set; }
        public ConfigurationProcessingResult ValidationResult { get; private set; }
        public IList<BuildStageRecord> StageRecords { get; private set; }
        public IList<string> PartArtifacts { get; private set; }
        public IList<string> AssemblyArtifacts { get; private set; }
        public string RunDirectoryPath { get; private set; }
        public string LogFilePath { get; private set; }
        public string SummaryFilePath { get; private set; }

        public bool HasBlockingFailures
        {
            get { return ValidationResult != null && ValidationResult.HasBlockingFailures; }
        }

        public bool HasFailedStages
        {
            get
            {
                for (int i = 0; i < StageRecords.Count; i++)
                {
                    if (StageRecords[i].Status == BuildStageStatus.Failed)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsSuccessful
        {
            get { return !HasBlockingFailures && !HasFailedStages; }
        }

        internal void SetArtifactPaths(string runDirectoryPath, string logFilePath, string summaryFilePath)
        {
            RunDirectoryPath = runDirectoryPath ?? string.Empty;
            LogFilePath = logFilePath ?? string.Empty;
            SummaryFilePath = summaryFilePath ?? string.Empty;
        }

        public string ToDisplaySummary()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Run ID: ").Append(Metadata.RunId);
            builder.AppendLine();
            builder.Append("Build Status: ").Append(IsSuccessful ? "SUCCESS" : "FAILED");
            builder.AppendLine();
            builder.Append("Config Hash: ").Append(Metadata.ConfigHash);
            builder.AppendLine();
            builder.Append("Stages: ").Append(StageRecords.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine();
            builder.Append("Blocking Validation Failures: ").Append(HasBlockingFailures ? "Yes" : "No");
            if (!string.IsNullOrWhiteSpace(LogFilePath))
            {
                builder.AppendLine();
                builder.Append("Log Path: ").Append(LogFilePath);
            }

            return builder.ToString();
        }
    }

    public sealed class BuildArtifactWriter
    {
        private readonly string _outputRootPath;

        public BuildArtifactWriter(string outputRootPath)
        {
            if (string.IsNullOrWhiteSpace(outputRootPath))
            {
                throw new ArgumentException("Output root path cannot be null or whitespace.", "outputRootPath");
            }

            _outputRootPath = outputRootPath;
        }

        public void Persist(BuildExecutionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            string runDirectoryPath = Path.Combine(_outputRootPath, result.Metadata.RunId);
            Directory.CreateDirectory(runDirectoryPath);

            string logFilePath = Path.Combine(runDirectoryPath, "build.log");
            string summaryFilePath = Path.Combine(runDirectoryPath, "build.summary.json");

            File.WriteAllText(logFilePath, BuildLogText(result));
            File.WriteAllText(summaryFilePath, BuildSummaryJson(result));

            result.SetArtifactPaths(runDirectoryPath, logFilePath, summaryFilePath);
        }

        private static string BuildLogText(BuildExecutionResult result)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("AXIONFRAME BUILD RUN");
            builder.Append("run-id=").Append(result.Metadata.RunId).AppendLine();
            builder.Append("timestamp-utc=").Append(FormatUtc(result.Metadata.TimestampUtc)).AppendLine();
            builder.Append("config-path=").Append(result.Metadata.ConfigPath).AppendLine();
            builder.Append("config-hash=").Append(result.Metadata.ConfigHash).AppendLine();
            builder.Append("status=").Append(result.IsSuccessful ? "success" : "failed").AppendLine();
            builder.AppendLine("stages:");

            for (int i = 0; i < result.StageRecords.Count; i++)
            {
                BuildStageRecord stage = result.StageRecords[i];
                builder.Append("  ").Append(stage.Sequence.ToString(CultureInfo.InvariantCulture));
                builder.Append(". ").Append(stage.StageName);
                builder.Append(" | status=").Append(stage.Status.ToString().ToLowerInvariant());
                builder.Append(" | started=").Append(FormatUtc(stage.StartedAtUtc));
                builder.Append(" | completed=").Append(FormatUtc(stage.CompletedAtUtc));
                if (!string.IsNullOrWhiteSpace(stage.Details))
                {
                    builder.Append(" | details=").Append(stage.Details);
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string BuildSummaryJson(BuildExecutionResult result)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> root = new Dictionary<string, object>(StringComparer.Ordinal);
            root["runId"] = result.Metadata.RunId;
            root["timestampUtc"] = FormatUtc(result.Metadata.TimestampUtc);
            root["configPath"] = result.Metadata.ConfigPath;
            root["configHash"] = result.Metadata.ConfigHash;
            root["isSuccessful"] = result.IsSuccessful;
            root["hasBlockingFailures"] = result.HasBlockingFailures;
            root["stageCount"] = result.StageRecords.Count;
            root["partArtifactCount"] = result.PartArtifacts.Count;
            root["assemblyArtifactCount"] = result.AssemblyArtifacts.Count;

            List<Dictionary<string, object> > stageEntries = new List<Dictionary<string, object> >();
            for (int i = 0; i < result.StageRecords.Count; i++)
            {
                BuildStageRecord stage = result.StageRecords[i];
                Dictionary<string, object> stageEntry = new Dictionary<string, object>(StringComparer.Ordinal);
                stageEntry["sequence"] = stage.Sequence;
                stageEntry["stage"] = stage.StageName;
                stageEntry["status"] = stage.Status.ToString().ToLowerInvariant();
                stageEntry["startedUtc"] = FormatUtc(stage.StartedAtUtc);
                stageEntry["completedUtc"] = FormatUtc(stage.CompletedAtUtc);
                stageEntry["details"] = stage.Details;
                stageEntries.Add(stageEntry);
            }

            root["stages"] = stageEntries;
            root["partArtifacts"] = result.PartArtifacts;
            root["assemblyArtifacts"] = result.AssemblyArtifacts;

            if (result.ValidationResult != null)
            {
                root["validationMessageCount"] = result.ValidationResult.Messages.Count;
                root["validationIsValid"] = result.ValidationResult.IsValid;
            }

            return serializer.Serialize(root);
        }

        private static string FormatUtc(DateTime value)
        {
            return value.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}
