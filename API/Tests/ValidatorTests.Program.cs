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
