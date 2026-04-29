using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AxionFrame
{
    public enum NamingEntityType
    {
        Feature = 0,
        Mate = 1,
        Configuration = 2,
        ExportArtifact = 3,
        ValidationSection = 4
    }

    public sealed class DeterministicNamingService
    {
        public const string RuleSetStandardV1 = "AXF_STANDARD_V1";

        private static readonly IDictionary<string, string> DomainTokenByNormalizedDomain = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "FRAME", "FRM" },
            { "FRM", "FRM" },
            { "PIVOT", "PVT" },
            { "PVT", "PVT" },
            { "HEIGHT", "HGT" },
            { "HEIGHT_INDEX", "HGT" },
            { "HEIGHT_INDEXING", "HGT" },
            { "HGT", "HGT" },
            { "PLATE_BRACE", "PLT" },
            { "PLATE_AND_BRACE", "PLT" },
            { "PLATEBRACE", "PLT" },
            { "PLT", "PLT" }
        };

        private static readonly string[] AllowedDomainTokens =
        {
            "FRM",
            "PVT",
            "HGT",
            "PLT"
        };

        private static readonly IDictionary<string, string> RequiredStableHooks = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "FRM-001", "AXF_FRM_LAYOUT_PRIMARY" },
            { "FRM-002", "AXF_FRM_PROFILE_MAIN" },
            { "FRM-003", "AXF_FRM_*" },
            { "PVT-001", "AXF_PVT_JOINT_PRIMARY" },
            { "PVT-002", "AXF_PVT_HOLE_PATTERN" },
            { "PVT-003", "AXF_MATE_PVT_PRIMARY" },
            { "HGT-001", "AXF_CFG_HEIGHT_*" },
            { "HGT-002", "AXF_CFG_HEIGHT_INDEXED" },
            { "HGT-003", "AXF_HGT_VALIDATION_SET" },
            { "PLT-001", "AXF_PLT_BRACE_PRIMARY" },
            { "PLT-002", "AXF_PLT_EXPORT_DXF" },
            { "PLT-003", "AXF_PLT_*" }
        };

        public string CreateFeatureName(string domain, string component, string descriptor)
        {
            return JoinTokens("AXF", ResolveDomainToken(domain), NormalizeToken(component), NormalizeToken(descriptor));
        }

        public string CreateMateName(string domain, string descriptor)
        {
            return JoinTokens("AXF", "MATE", ResolveDomainToken(domain), NormalizeToken(descriptor));
        }

        public string CreateConfigurationName(string domain, string descriptor)
        {
            return JoinTokens("AXF", "CFG", ResolveDomainToken(domain), NormalizeToken(descriptor));
        }

        public string CreateHeightConfigurationName(decimal heightMillimeters)
        {
            return CreateConfigurationName("HGT", NormalizeDiscreteNumericToken(heightMillimeters));
        }

        public string CreateExportArtifactName(string exportType, string descriptor)
        {
            return JoinTokens("AXF", "EXP", NormalizeToken(exportType), NormalizeToken(descriptor));
        }

        public string CreateValidationSectionIdentifier(string domain, string descriptor)
        {
            return JoinTokens("AXF", "VAL", ResolveDomainToken(domain), NormalizeToken(descriptor));
        }

        public string ResolveDomainToken(string domain)
        {
            string normalizedDomain = NormalizeToken(domain);
            string token;
            if (!DomainTokenByNormalizedDomain.TryGetValue(normalizedDomain, out token))
            {
                throw new ArgumentException("Unsupported naming domain: " + normalizedDomain + ".", "domain");
            }

            return token;
        }

        public IDictionary<string, string> GetRequiredStableHooks()
        {
            return new Dictionary<string, string>(RequiredStableHooks, StringComparer.Ordinal);
        }

        public string GetRequiredStableHook(string ruleId)
        {
            if (string.IsNullOrWhiteSpace(ruleId))
            {
                throw new ArgumentException("Rule id cannot be null or whitespace.", "ruleId");
            }

            string hook;
            if (!RequiredStableHooks.TryGetValue(ruleId, out hook))
            {
                throw new ArgumentException("Unsupported required stable-hook rule id: " + ruleId + ".", "ruleId");
            }

            return hook;
        }

        public string NormalizeToken(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Token value cannot be null or whitespace.", "input");
            }

            StringBuilder builder = new StringBuilder(input.Length);
            bool previousWasUnderscore = false;
            string upperInvariant = input.ToUpperInvariant();
            for (int i = 0; i < upperInvariant.Length; i++)
            {
                char current = upperInvariant[i];
                if (IsAsciiLetter(current) || IsDigit(current))
                {
                    builder.Append(current);
                    previousWasUnderscore = false;
                    continue;
                }

                if (!previousWasUnderscore)
                {
                    builder.Append('_');
                    previousWasUnderscore = true;
                }
            }

            string normalized = TrimUnderscores(builder.ToString());
            if (normalized.Length == 0)
            {
                throw new ArgumentException("Token does not contain any supported alphanumeric characters.", "input");
            }

            return normalized;
        }

        public string NormalizeDiscreteNumericToken(decimal value)
        {
            if (decimal.Truncate(value) == value)
            {
                return value.ToString("0", CultureInfo.InvariantCulture);
            }

            string valueText = value.ToString("0.############################", CultureInfo.InvariantCulture);
            string withUnderscoreDecimalSeparator = valueText.Replace(".", "_");
            return NormalizeToken(withUnderscoreDecimalSeparator);
        }

        public bool IsCompliantName(string value, NamingEntityType entityType)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (value.IndexOf("__", StringComparison.Ordinal) >= 0)
            {
                return false;
            }

            string[] parts = value.Split('_');
            if (parts.Length < 4)
            {
                return false;
            }

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length == 0)
                {
                    return false;
                }
            }

            if (!string.Equals(parts[0], "AXF", StringComparison.Ordinal))
            {
                return false;
            }

            switch (entityType)
            {
                case NamingEntityType.Feature:
                    return ValidateFeatureName(parts);
                case NamingEntityType.Mate:
                    return ValidateMateName(parts);
                case NamingEntityType.Configuration:
                    return ValidateConfigurationName(parts);
                case NamingEntityType.ExportArtifact:
                    return ValidateExportName(parts);
                case NamingEntityType.ValidationSection:
                    return ValidateValidationName(parts);
                default:
                    return false;
            }
        }

        private static bool ValidateFeatureName(string[] parts)
        {
            if (!IsAllowedDomainToken(parts[1]))
            {
                return false;
            }

            return ValidateTokens(parts, 2);
        }

        private static bool ValidateMateName(string[] parts)
        {
            if (!string.Equals(parts[1], "MATE", StringComparison.Ordinal))
            {
                return false;
            }

            if (!IsAllowedDomainToken(parts[2]))
            {
                return false;
            }

            return ValidateTokens(parts, 3);
        }

        private static bool ValidateConfigurationName(string[] parts)
        {
            if (!string.Equals(parts[1], "CFG", StringComparison.Ordinal))
            {
                return false;
            }

            if (!IsAllowedDomainToken(parts[2]))
            {
                return false;
            }

            return ValidateTokens(parts, 3);
        }

        private static bool ValidateExportName(string[] parts)
        {
            if (!string.Equals(parts[1], "EXP", StringComparison.Ordinal))
            {
                return false;
            }

            return ValidateTokens(parts, 2);
        }

        private static bool ValidateValidationName(string[] parts)
        {
            if (!string.Equals(parts[1], "VAL", StringComparison.Ordinal))
            {
                return false;
            }

            if (!IsAllowedDomainToken(parts[2]))
            {
                return false;
            }

            return ValidateTokens(parts, 3);
        }

        private static bool ValidateTokens(string[] parts, int startIndex)
        {
            for (int i = startIndex; i < parts.Length; i++)
            {
                if (!IsAsciiToken(parts[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAllowedDomainToken(string token)
        {
            for (int i = 0; i < AllowedDomainTokens.Length; i++)
            {
                if (string.Equals(AllowedDomainTokens[i], token, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsAsciiToken(string token)
        {
            for (int i = 0; i < token.Length; i++)
            {
                char current = token[i];
                if (!IsAsciiLetter(current) && !IsDigit(current))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAsciiLetter(char value)
        {
            return value >= 'A' && value <= 'Z';
        }

        private static bool IsDigit(char value)
        {
            return value >= '0' && value <= '9';
        }

        private static string TrimUnderscores(string value)
        {
            int start = 0;
            int end = value.Length - 1;
            while (start <= end && value[start] == '_')
            {
                start++;
            }

            while (end >= start && value[end] == '_')
            {
                end--;
            }

            if (end < start)
            {
                return string.Empty;
            }

            return value.Substring(start, end - start + 1);
        }

        private static string JoinTokens(params string[] parts)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append('_');
                }

                builder.Append(parts[i]);
            }

            return builder.ToString();
        }
    }
}
