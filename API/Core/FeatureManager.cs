using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;

namespace AxionFrame
{
    public enum ConfigValueType
    {
        String,
        Boolean,
        Integer,
        Decimal,
        Enum,
        ArrayString,
        ArrayDecimal,
        Object
    }

    public enum ValidationSeverity
    {
        Critical = 0,
        Error = 1,
        Warning = 2,
        Info = 3
    }

    public enum ValidatorGroup
    {
        Schema = 0,
        CrossField = 1
    }

    public sealed class ValidationMessage
    {
        public ValidationMessage(
            ValidationSeverity severity,
            string ruleId,
            string validatorId,
            string configKey,
            string artifactScope,
            string message,
            string expected,
            string actual,
            string recommendedAction,
            bool blocking,
            ValidatorGroup group)
        {
            Severity = severity;
            RuleId = ruleId ?? string.Empty;
            ValidatorId = validatorId ?? string.Empty;
            ConfigKey = configKey ?? string.Empty;
            ArtifactScope = artifactScope ?? string.Empty;
            Message = message ?? string.Empty;
            Expected = expected ?? string.Empty;
            Actual = actual ?? string.Empty;
            RecommendedAction = recommendedAction ?? string.Empty;
            Blocking = blocking;
            Group = group;
        }

        public ValidationSeverity Severity { get; private set; }
        public string RuleId { get; private set; }
        public string ValidatorId { get; private set; }
        public string ConfigKey { get; private set; }
        public string ArtifactScope { get; private set; }
        public string Message { get; private set; }
        public string Expected { get; private set; }
        public string Actual { get; private set; }
        public string RecommendedAction { get; private set; }
        public bool Blocking { get; private set; }
        internal ValidatorGroup Group { get; private set; }
    }

    public sealed class ConfigurationProcessingResult
    {
        public ConfigurationProcessingResult(
            string sourcePath,
            IDictionary<string, object> normalizedConfig,
            IList<ValidationMessage> messages)
        {
            SourcePath = sourcePath ?? string.Empty;
            NormalizedConfig = normalizedConfig ?? new Dictionary<string, object>(StringComparer.Ordinal);
            Messages = messages ?? new List<ValidationMessage>();

            bool hasBlockingFailures = false;
            ValidationSeverity highestSeverity = ValidationSeverity.Info;
            for (int i = 0; i < Messages.Count; i++)
            {
                ValidationMessage message = Messages[i];
                if (message.Blocking)
                {
                    hasBlockingFailures = true;
                }

                if ((int)message.Severity < (int)highestSeverity)
                {
                    highestSeverity = message.Severity;
                }
            }

            HasBlockingFailures = hasBlockingFailures;
            HighestSeverity = highestSeverity;
            IsValid = !hasBlockingFailures;
        }

        public string SourcePath { get; private set; }
        public IDictionary<string, object> NormalizedConfig { get; private set; }
        public IList<ValidationMessage> Messages { get; private set; }
        public bool HasBlockingFailures { get; private set; }
        public bool IsValid { get; private set; }
        public ValidationSeverity HighestSeverity { get; private set; }
    }

    internal sealed class SchemaFieldDefinition
    {
        public SchemaFieldDefinition(
            string key,
            ConfigValueType valueType,
            bool required,
            object defaultValue,
            string ruleId,
            string validatorId,
            ValidationSeverity severity,
            bool blocking)
        {
            Key = key;
            ValueType = valueType;
            Required = required;
            DefaultValue = defaultValue;
            RuleId = ruleId;
            ValidatorId = validatorId;
            Severity = severity;
            Blocking = blocking;
            AllowedStringValues = new List<string>();
            AllowedDecimalValues = new List<decimal>();
        }

        public string Key { get; private set; }
        public ConfigValueType ValueType { get; private set; }
        public bool Required { get; private set; }
        public object DefaultValue { get; private set; }
        public string RuleId { get; private set; }
        public string ValidatorId { get; private set; }
        public ValidationSeverity Severity { get; private set; }
        public bool Blocking { get; private set; }
        public decimal? MinDecimal { get; set; }
        public decimal? MaxDecimal { get; set; }
        public int? MinInteger { get; set; }
        public int? MaxInteger { get; set; }
        public IList<string> AllowedStringValues { get; private set; }
        public IList<decimal> AllowedDecimalValues { get; private set; }
    }

    public sealed class FeatureManager
    {
        private static readonly string[] BaselineProfiles =
        {
            "40x40x2.0_SHS",
            "60x30x2.0_RHS"
        };

        private static readonly decimal[] BaselineHeights =
        {
            680.0m,
            730.0m,
            780.0m
        };

        private readonly IList<SchemaFieldDefinition> _schema;

        public FeatureManager()
        {
            _schema = BuildDefaultSchema();
        }

        public ConfigurationProcessingResult LoadAndValidate(string configPath)
        {
            List<ValidationMessage> messages = new List<ValidationMessage>();
            Dictionary<string, object> sourceValues = LoadFlatConfiguration(configPath, messages);
            Dictionary<string, object> normalizedValues = new Dictionary<string, object>(StringComparer.Ordinal);

            for (int i = 0; i < _schema.Count; i++)
            {
                SchemaFieldDefinition field = _schema[i];
                ApplySchemaField(sourceValues, normalizedValues, field, messages);
            }

            ApplyCrossFieldRules(normalizedValues, messages);
            SortMessages(messages);

            return new ConfigurationProcessingResult(configPath, normalizedValues, messages);
        }

        private static IList<SchemaFieldDefinition> BuildDefaultSchema()
        {
            List<SchemaFieldDefinition> schema = new List<SchemaFieldDefinition>();

            schema.Add(DecimalField("frame.layout.primary.memberExtentMin", 620.0m, 620.0m, 980.0m, "FRM-001", "VAL-FRM-001-LAYOUT", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("frame.layout.primary.memberExtentMax", 980.0m, 620.0m, 980.0m, "FRM-001", "VAL-FRM-001-LAYOUT", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("frame.layout.primary.placementTolerance", 0.5m, 0.0m, 0.5m, "FRM-001", "VAL-FRM-001-LAYOUT", ValidationSeverity.Critical, true));

            schema.Add(ArrayStringField("frame.profile.selection.allowedProfiles", BaselineProfiles, BaselineProfiles, "FRM-002", "VAL-FRM-002-PROFILE", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("frame.profile.selection.dimensionTolerance", 0.2m, 0.0m, 0.2m, "FRM-002", "VAL-FRM-002-PROFILE", ValidationSeverity.Critical, true));
            schema.Add(EnumField("frame.naming.ruleSet", "AXF_STANDARD_V1", new[] { "AXF_STANDARD_V1" }, "FRM-003", "VAL-FRM-003-NAMING", ValidationSeverity.Warning, false));

            schema.Add(DecimalField("pivot.geometry.primary.axisLocationMin", 300.0m, 300.0m, 450.0m, "PVT-001", "VAL-PVT-001-GEOMETRY", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("pivot.geometry.primary.axisLocationMax", 450.0m, 300.0m, 450.0m, "PVT-001", "VAL-PVT-001-GEOMETRY", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("pivot.geometry.primary.alignmentTolerance", 0.25m, 0.0m, 0.25m, "PVT-001", "VAL-PVT-001-GEOMETRY", ValidationSeverity.Critical, true));

            schema.Add(DecimalField("pivot.hole.strategy.diameterMin", 10.5m, 10.5m, 11.0m, "PVT-002", "VAL-PVT-002-HOLES", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("pivot.hole.strategy.diameterMax", 11.0m, 10.5m, 11.0m, "PVT-002", "VAL-PVT-002-HOLES", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("pivot.hole.strategy.positionTolerance", 0.2m, 0.0m, 0.2m, "PVT-002", "VAL-PVT-002-HOLES", ValidationSeverity.Critical, true));
            schema.Add(EnumField("pivot.naming.mates", "AXF_STANDARD_V1", new[] { "AXF_STANDARD_V1" }, "PVT-003", "VAL-PVT-003-MATES", ValidationSeverity.Warning, false));

            schema.Add(ArrayDecimalField("height.supportedConfigurations.values", BaselineHeights, BaselineHeights, "HGT-001", "VAL-HGT-001-SUPPORTED-CONFIGS", ValidationSeverity.Critical, true));
            schema.Add(IntegerField("height.indexing.activation.requiredCount", 3, 3, 3, "HGT-002", "VAL-HGT-002-ACTIVATION", ValidationSeverity.Critical, true));
            schema.Add(BooleanField("height.indexing.activation.strictDeterminism", true, "HGT-002", "VAL-HGT-002-ACTIVATION", ValidationSeverity.Critical, true));
            schema.Add(ArrayDecimalField("height.validation.supportedSet", BaselineHeights, BaselineHeights, "HGT-003", "VAL-HGT-003-HEIGHT-VALIDITY", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("height.validation.dimensionTolerance", 1.0m, 0.0m, 1.0m, "HGT-003", "VAL-HGT-003-HEIGHT-VALIDITY", ValidationSeverity.Critical, true));

            schema.Add(DecimalField("plateBrace.dimensions.primary.thicknessMin", 5.0m, 5.0m, 8.0m, "PLT-001", "VAL-PLT-001-DIMENSIONS", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("plateBrace.dimensions.primary.thicknessMax", 8.0m, 5.0m, 8.0m, "PLT-001", "VAL-PLT-001-DIMENSIONS", ValidationSeverity.Critical, true));
            schema.Add(DecimalField("plateBrace.dimensions.primary.dimensionTolerance", 0.2m, 0.0m, 0.2m, "PLT-001", "VAL-PLT-001-DIMENSIONS", ValidationSeverity.Critical, true));
            schema.Add(BooleanField("plateBrace.export.dxfEligible", true, "PLT-002", "VAL-PLT-002-DXF-ELIGIBILITY", ValidationSeverity.Critical, true));
            schema.Add(EnumField("plateBrace.naming.ruleSet", "AXF_STANDARD_V1", new[] { "AXF_STANDARD_V1" }, "PLT-003", "VAL-PLT-003-NAMING", ValidationSeverity.Warning, false));

            schema.Add(BooleanField("exports.step.enabled", true, "CFG-EXP-STEP", "VAL-CFG-EXP-STEP", ValidationSeverity.Error, true));
            schema.Add(BooleanField("exports.dxf.enabled", true, "CFG-EXP-DXF", "VAL-CFG-EXP-DXF", ValidationSeverity.Error, true));
            schema.Add(BooleanField("exports.bom.enabled", true, "CFG-EXP-BOM", "VAL-CFG-EXP-BOM", ValidationSeverity.Error, true));
            schema.Add(BooleanField("exports.validationReport.enabled", true, "CFG-EXP-REPORT", "VAL-CFG-EXP-REPORT", ValidationSeverity.Error, true));

            schema.Add(EnumField("validation.mode", "StrictRelease", new[] { "BuildOnly", "FinalOutput", "StrictRelease" }, "CFG-VAL-MODE", "VAL-CFG-VAL-MODE", ValidationSeverity.Error, true));
            schema.Add(BooleanField("validation.stopOnCriticalFailure", true, "CFG-VAL-STOP", "VAL-CFG-VAL-STOP", ValidationSeverity.Error, true));
            schema.Add(BooleanField("run.packageOutputs", true, "CFG-RUN-PACKAGE", "VAL-CFG-RUN-PACKAGE", ValidationSeverity.Error, true));

            return schema;
        }

        private static SchemaFieldDefinition DecimalField(
            string key,
            decimal defaultValue,
            decimal minValue,
            decimal maxValue,
            string ruleId,
            string validatorId,
            ValidationSeverity severity,
            bool blocking)
        {
            SchemaFieldDefinition field = new SchemaFieldDefinition(key, ConfigValueType.Decimal, true, defaultValue, ruleId, validatorId, severity, blocking);
            field.MinDecimal = minValue;
            field.MaxDecimal = maxValue;
            return field;
        }

        private static SchemaFieldDefinition IntegerField(
            string key,
            int defaultValue,
            int minValue,
            int maxValue,
            string ruleId,
            string validatorId,
            ValidationSeverity severity,
            bool blocking)
        {
            SchemaFieldDefinition field = new SchemaFieldDefinition(key, ConfigValueType.Integer, true, defaultValue, ruleId, validatorId, severity, blocking);
            field.MinInteger = minValue;
            field.MaxInteger = maxValue;
            return field;
        }

        private static SchemaFieldDefinition BooleanField(
            string key,
            bool defaultValue,
            string ruleId,
            string validatorId,
            ValidationSeverity severity,
            bool blocking)
        {
            return new SchemaFieldDefinition(key, ConfigValueType.Boolean, true, defaultValue, ruleId, validatorId, severity, blocking);
        }

        private static SchemaFieldDefinition EnumField(
            string key,
            string defaultValue,
            string[] allowedValues,
            string ruleId,
            string validatorId,
            ValidationSeverity severity,
            bool blocking)
        {
            SchemaFieldDefinition field = new SchemaFieldDefinition(key, ConfigValueType.Enum, true, defaultValue, ruleId, validatorId, severity, blocking);
            for (int i = 0; i < allowedValues.Length; i++)
            {
                field.AllowedStringValues.Add(allowedValues[i]);
            }

            return field;
        }

        private static SchemaFieldDefinition ArrayStringField(
            string key,
            string[] defaultValue,
            string[] allowedValues,
            string ruleId,
            string validatorId,
            ValidationSeverity severity,
            bool blocking)
        {
            SchemaFieldDefinition field = new SchemaFieldDefinition(key, ConfigValueType.ArrayString, true, new List<string>(defaultValue), ruleId, validatorId, severity, blocking);
            for (int i = 0; i < allowedValues.Length; i++)
            {
                field.AllowedStringValues.Add(allowedValues[i]);
            }

            return field;
        }

        private static SchemaFieldDefinition ArrayDecimalField(
            string key,
            decimal[] defaultValue,
            decimal[] allowedValues,
            string ruleId,
            string validatorId,
            ValidationSeverity severity,
            bool blocking)
        {
            SchemaFieldDefinition field = new SchemaFieldDefinition(key, ConfigValueType.ArrayDecimal, true, new List<decimal>(defaultValue), ruleId, validatorId, severity, blocking);
            for (int i = 0; i < allowedValues.Length; i++)
            {
                field.AllowedDecimalValues.Add(allowedValues[i]);
            }

            return field;
        }

        private static Dictionary<string, object> LoadFlatConfiguration(string configPath, IList<ValidationMessage> messages)
        {
            Dictionary<string, object> flattened = new Dictionary<string, object>(StringComparer.Ordinal);
            if (string.IsNullOrWhiteSpace(configPath))
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.Schema,
                    ValidationSeverity.Error,
                    true,
                    "CFG-SRC-001",
                    "VAL-CFG-SRC-PATH",
                    "configuration.sourcePath",
                    string.Empty,
                    "Configuration path is missing.",
                    "A valid path to Config/GlobalParams.json.",
                    "<missing>",
                    "Pass an explicit configuration path to the loader."));
                return flattened;
            }

            if (!File.Exists(configPath))
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.Schema,
                    ValidationSeverity.Warning,
                    false,
                    "CFG-SRC-002",
                    "VAL-CFG-SRC-FILE",
                    "configuration.sourcePath",
                    string.Empty,
                    "Configuration file not found; documented defaults are applied.",
                    "Existing JSON file.",
                    configPath,
                    "Create the config file or keep relying on deterministic defaults."));
                return flattened;
            }

            string jsonContent = File.ReadAllText(configPath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.Schema,
                    ValidationSeverity.Warning,
                    false,
                    "CFG-SRC-003",
                    "VAL-CFG-SRC-EMPTY",
                    "configuration.sourcePath",
                    string.Empty,
                    "Configuration file is empty; documented defaults are applied.",
                    "Non-empty JSON object.",
                    "<empty>",
                    "Populate the file or keep relying on deterministic defaults."));
                return flattened;
            }

            object deserializedRoot;
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                deserializedRoot = serializer.DeserializeObject(jsonContent);
            }
            catch (Exception ex)
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.Schema,
                    ValidationSeverity.Critical,
                    true,
                    "CFG-SRC-004",
                    "VAL-CFG-SRC-PARSE",
                    "configuration.sourcePath",
                    string.Empty,
                    "Configuration JSON could not be parsed.",
                    "A valid JSON object.",
                    ex.Message,
                    "Fix JSON syntax errors in the configuration file."));
                return flattened;
            }

            IDictionary<string, object> rootDictionary = deserializedRoot as IDictionary<string, object>;
            if (rootDictionary == null)
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.Schema,
                    ValidationSeverity.Critical,
                    true,
                    "CFG-SRC-005",
                    "VAL-CFG-SRC-ROOT",
                    "configuration.sourcePath",
                    string.Empty,
                    "Configuration root must be a JSON object.",
                    "{ ... }",
                    deserializedRoot == null ? "<null>" : deserializedRoot.GetType().Name,
                    "Wrap top-level values under a JSON object."));
                return flattened;
            }

            FlattenDictionary(rootDictionary, string.Empty, flattened);
            return flattened;
        }

        private static void FlattenDictionary(
            IDictionary<string, object> source,
            string prefix,
            IDictionary<string, object> destination)
        {
            foreach (KeyValuePair<string, object> pair in source)
            {
                string keyPath = string.IsNullOrEmpty(prefix) ? pair.Key : prefix + "." + pair.Key;
                if (pair.Value is IDictionary<string, object>)
                {
                    FlattenDictionary((IDictionary<string, object>)pair.Value, keyPath, destination);
                    continue;
                }

                IDictionary nonGenericDictionary = pair.Value as IDictionary;
                if (nonGenericDictionary != null)
                {
                    Dictionary<string, object> converted = new Dictionary<string, object>(StringComparer.Ordinal);
                    foreach (DictionaryEntry entry in nonGenericDictionary)
                    {
                        converted[Convert.ToString(entry.Key, CultureInfo.InvariantCulture)] = entry.Value;
                    }

                    FlattenDictionary(converted, keyPath, destination);
                    continue;
                }

                destination[keyPath] = pair.Value;
            }
        }

        private static void ApplySchemaField(
            IDictionary<string, object> sourceValues,
            IDictionary<string, object> normalizedValues,
            SchemaFieldDefinition field,
            IList<ValidationMessage> messages)
        {
            object sourceValue;
            bool exists = sourceValues.TryGetValue(field.Key, out sourceValue);

            if (!exists || sourceValue == null)
            {
                normalizedValues[field.Key] = CloneDefaultValue(field.DefaultValue);
                messages.Add(CreateMessage(
                    ValidatorGroup.Schema,
                    field.Severity,
                    field.Blocking,
                    field.RuleId,
                    field.ValidatorId,
                    field.Key,
                    string.Empty,
                    "Required value is missing; default value was applied.",
                    FormatExpectedFromDefinition(field),
                    "<missing>",
                    "Provide a valid value in configuration or accept the default."));
                return;
            }

            object convertedValue;
            string conversionError;
            if (!TryConvertValue(sourceValue, field, out convertedValue, out conversionError))
            {
                normalizedValues[field.Key] = CloneDefaultValue(field.DefaultValue);
                messages.Add(CreateMessage(
                    ValidatorGroup.Schema,
                    field.Severity,
                    field.Blocking,
                    field.RuleId,
                    field.ValidatorId,
                    field.Key,
                    string.Empty,
                    "Invalid value type or format; default value was applied.",
                    FormatExpectedFromDefinition(field),
                    conversionError,
                    "Provide a value with the expected type and range."));
                return;
            }

            string validationError;
            if (!ValidateAllowedRanges(field, convertedValue, out validationError))
            {
                normalizedValues[field.Key] = CloneDefaultValue(field.DefaultValue);
                messages.Add(CreateMessage(
                    ValidatorGroup.Schema,
                    field.Severity,
                    field.Blocking,
                    field.RuleId,
                    field.ValidatorId,
                    field.Key,
                    string.Empty,
                    "Value failed schema constraints; default value was applied.",
                    FormatExpectedFromDefinition(field),
                    validationError,
                    "Adjust the value to the documented allowed range or set."));
                return;
            }

            normalizedValues[field.Key] = convertedValue;
        }

        private static object CloneDefaultValue(object defaultValue)
        {
            List<string> stringList = defaultValue as List<string>;
            if (stringList != null)
            {
                return new List<string>(stringList);
            }

            List<decimal> decimalList = defaultValue as List<decimal>;
            if (decimalList != null)
            {
                return new List<decimal>(decimalList);
            }

            return defaultValue;
        }

        private static bool TryConvertValue(
            object sourceValue,
            SchemaFieldDefinition field,
            out object convertedValue,
            out string conversionError)
        {
            convertedValue = null;
            conversionError = string.Empty;

            switch (field.ValueType)
            {
                case ConfigValueType.String:
                case ConfigValueType.Enum:
                    {
                        string stringValue = sourceValue as string;
                        if (stringValue == null)
                        {
                            conversionError = "Actual type: " + sourceValue.GetType().Name + ".";
                            return false;
                        }

                        convertedValue = stringValue;
                        return true;
                    }

                case ConfigValueType.Boolean:
                    {
                        bool booleanValue;
                        if (sourceValue is bool)
                        {
                            convertedValue = sourceValue;
                            return true;
                        }

                        string stringValue = sourceValue as string;
                        if (stringValue != null && bool.TryParse(stringValue, out booleanValue))
                        {
                            convertedValue = booleanValue;
                            return true;
                        }

                        conversionError = "Actual value: " + FormatValue(sourceValue) + ".";
                        return false;
                    }

                case ConfigValueType.Integer:
                    {
                        int integerValue;
                        if (sourceValue is int)
                        {
                            convertedValue = sourceValue;
                            return true;
                        }

                        if (sourceValue is long)
                        {
                            long longValue = (long)sourceValue;
                            if (longValue <= int.MaxValue && longValue >= int.MinValue)
                            {
                                convertedValue = (int)longValue;
                                return true;
                            }
                        }

                        if (sourceValue is decimal)
                        {
                            decimal decimalValue = (decimal)sourceValue;
                            if (decimal.Truncate(decimalValue) == decimalValue)
                            {
                                convertedValue = (int)decimalValue;
                                return true;
                            }
                        }

                        string stringValue = sourceValue as string;
                        if (stringValue != null && int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out integerValue))
                        {
                            convertedValue = integerValue;
                            return true;
                        }

                        conversionError = "Actual value: " + FormatValue(sourceValue) + ".";
                        return false;
                    }

                case ConfigValueType.Decimal:
                    {
                        decimal decimalValue;
                        if (sourceValue is decimal)
                        {
                            convertedValue = sourceValue;
                            return true;
                        }

                        if (sourceValue is double || sourceValue is float || sourceValue is int || sourceValue is long)
                        {
                            convertedValue = Convert.ToDecimal(sourceValue, CultureInfo.InvariantCulture);
                            return true;
                        }

                        string stringValue = sourceValue as string;
                        if (stringValue != null && decimal.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimalValue))
                        {
                            convertedValue = decimalValue;
                            return true;
                        }

                        conversionError = "Actual value: " + FormatValue(sourceValue) + ".";
                        return false;
                    }

                case ConfigValueType.ArrayString:
                    {
                        List<string> values = new List<string>();
                        IList items;
                        if (!TryExtractEnumerable(sourceValue, out items))
                        {
                            conversionError = "Actual value is not an array: " + FormatValue(sourceValue) + ".";
                            return false;
                        }

                        for (int i = 0; i < items.Count; i++)
                        {
                            string itemValue = items[i] as string;
                            if (itemValue == null)
                            {
                                conversionError = "Array item at index " + i.ToString(CultureInfo.InvariantCulture) + " is not a string.";
                                return false;
                            }

                            values.Add(itemValue);
                        }

                        convertedValue = values;
                        return true;
                    }

                case ConfigValueType.ArrayDecimal:
                    {
                        List<decimal> values = new List<decimal>();
                        IList items;
                        if (!TryExtractEnumerable(sourceValue, out items))
                        {
                            conversionError = "Actual value is not an array: " + FormatValue(sourceValue) + ".";
                            return false;
                        }

                        for (int i = 0; i < items.Count; i++)
                        {
                            object item = items[i];
                            decimal decimalItem;
                            if (item is decimal)
                            {
                                values.Add((decimal)item);
                                continue;
                            }

                            if (item is double || item is float || item is int || item is long)
                            {
                                values.Add(Convert.ToDecimal(item, CultureInfo.InvariantCulture));
                                continue;
                            }

                            string stringValue = item as string;
                            if (stringValue != null && decimal.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimalItem))
                            {
                                values.Add(decimalItem);
                                continue;
                            }

                            conversionError = "Array item at index " + i.ToString(CultureInfo.InvariantCulture) + " is not a decimal.";
                            return false;
                        }

                        convertedValue = values;
                        return true;
                    }

                case ConfigValueType.Object:
                    {
                        if (sourceValue is IDictionary<string, object> || sourceValue is IDictionary)
                        {
                            convertedValue = sourceValue;
                            return true;
                        }

                        conversionError = "Actual value is not an object: " + FormatValue(sourceValue) + ".";
                        return false;
                    }

                default:
                    conversionError = "Unsupported value type.";
                    return false;
            }
        }

        private static bool TryExtractEnumerable(object sourceValue, out IList items)
        {
            items = sourceValue as IList;
            if (items != null)
            {
                return true;
            }

            object[] array = sourceValue as object[];
            if (array != null)
            {
                items = array;
                return true;
            }

            return false;
        }

        private static bool ValidateAllowedRanges(SchemaFieldDefinition field, object value, out string validationError)
        {
            validationError = string.Empty;

            if (field.ValueType == ConfigValueType.Decimal)
            {
                decimal decimalValue = (decimal)value;
                if (field.MinDecimal.HasValue && decimalValue < field.MinDecimal.Value)
                {
                    validationError = "Actual " + decimalValue.ToString(CultureInfo.InvariantCulture) + " is below minimum " + field.MinDecimal.Value.ToString(CultureInfo.InvariantCulture) + ".";
                    return false;
                }

                if (field.MaxDecimal.HasValue && decimalValue > field.MaxDecimal.Value)
                {
                    validationError = "Actual " + decimalValue.ToString(CultureInfo.InvariantCulture) + " is above maximum " + field.MaxDecimal.Value.ToString(CultureInfo.InvariantCulture) + ".";
                    return false;
                }
            }

            if (field.ValueType == ConfigValueType.Integer)
            {
                int integerValue = (int)value;
                if (field.MinInteger.HasValue && integerValue < field.MinInteger.Value)
                {
                    validationError = "Actual " + integerValue.ToString(CultureInfo.InvariantCulture) + " is below minimum " + field.MinInteger.Value.ToString(CultureInfo.InvariantCulture) + ".";
                    return false;
                }

                if (field.MaxInteger.HasValue && integerValue > field.MaxInteger.Value)
                {
                    validationError = "Actual " + integerValue.ToString(CultureInfo.InvariantCulture) + " is above maximum " + field.MaxInteger.Value.ToString(CultureInfo.InvariantCulture) + ".";
                    return false;
                }
            }

            if (field.ValueType == ConfigValueType.Enum)
            {
                string stringValue = (string)value;
                if (!ContainsString(field.AllowedStringValues, stringValue))
                {
                    validationError = "Actual " + stringValue + " is not in allowed set.";
                    return false;
                }
            }

            if (field.ValueType == ConfigValueType.ArrayString && field.AllowedStringValues.Count > 0)
            {
                List<string> array = (List<string>)value;
                for (int i = 0; i < array.Count; i++)
                {
                    if (!ContainsString(field.AllowedStringValues, array[i]))
                    {
                        validationError = "Actual value " + array[i] + " is not in allowed set.";
                        return false;
                    }
                }
            }

            if (field.ValueType == ConfigValueType.ArrayDecimal && field.AllowedDecimalValues.Count > 0)
            {
                List<decimal> array = (List<decimal>)value;
                for (int i = 0; i < array.Count; i++)
                {
                    if (!ContainsDecimal(field.AllowedDecimalValues, array[i]))
                    {
                        validationError = "Actual value " + array[i].ToString(CultureInfo.InvariantCulture) + " is not in allowed set.";
                        return false;
                    }
                }
            }

            return true;
        }

        private static void ApplyCrossFieldRules(
            IDictionary<string, object> values,
            IList<ValidationMessage> messages)
        {
            ValidateMinLessOrEqual(values, messages, "frame.layout.primary.memberExtentMin", "frame.layout.primary.memberExtentMax", "FRM-001", "VAL-FRM-001-LAYOUT", "frame.layout.primary");
            ValidateMinLessOrEqual(values, messages, "pivot.geometry.primary.axisLocationMin", "pivot.geometry.primary.axisLocationMax", "PVT-001", "VAL-PVT-001-GEOMETRY", "pivot.geometry.primary");
            ValidateMinLessOrEqual(values, messages, "pivot.hole.strategy.diameterMin", "pivot.hole.strategy.diameterMax", "PVT-002", "VAL-PVT-002-HOLES", "pivot.hole.strategy");
            ValidateMinLessOrEqual(values, messages, "plateBrace.dimensions.primary.thicknessMin", "plateBrace.dimensions.primary.thicknessMax", "PLT-001", "VAL-PLT-001-DIMENSIONS", "plateBrace.dimensions.primary");

            List<decimal> supportedHeights = GetDecimalList(values, "height.supportedConfigurations.values");
            List<decimal> validationSet = GetDecimalList(values, "height.validation.supportedSet");
            if (supportedHeights != null && validationSet != null && !DecimalListsEqual(supportedHeights, validationSet))
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.CrossField,
                    ValidationSeverity.Critical,
                    true,
                    "HGT-003",
                    "VAL-HGT-003-HEIGHT-VALIDITY",
                    "height.validation.supportedSet",
                    "height.validation",
                    "Supported height list and validation set must match exactly.",
                    "Matching decimal arrays with identical order.",
                    FormatValue(validationSet),
                    "Set both height arrays to the same values and ordering."));
            }

            int? requiredCount = GetInteger(values, "height.indexing.activation.requiredCount");
            if (requiredCount.HasValue && supportedHeights != null && requiredCount.Value != supportedHeights.Count)
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.CrossField,
                    ValidationSeverity.Critical,
                    true,
                    "HGT-002",
                    "VAL-HGT-002-ACTIVATION",
                    "height.indexing.activation.requiredCount",
                    "height.indexing.activation",
                    "Required height configuration count must equal supported height entries.",
                    supportedHeights.Count.ToString(CultureInfo.InvariantCulture),
                    requiredCount.Value.ToString(CultureInfo.InvariantCulture),
                    "Update requiredCount or supported height array so both match."));
            }

            bool? dxfEnabled = GetBoolean(values, "exports.dxf.enabled");
            bool? dxfEligible = GetBoolean(values, "plateBrace.export.dxfEligible");
            if (dxfEnabled.HasValue && dxfEnabled.Value && dxfEligible.HasValue && !dxfEligible.Value)
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.CrossField,
                    ValidationSeverity.Critical,
                    true,
                    "PLT-002",
                    "VAL-PLT-002-DXF-ELIGIBILITY",
                    "plateBrace.export.dxfEligible",
                    "exports.dxf",
                    "DXF export cannot be enabled when no fabrication-relevant component is DXF-eligible.",
                    "plateBrace.export.dxfEligible = true when exports.dxf.enabled = true.",
                    "exports.dxf.enabled=true, plateBrace.export.dxfEligible=false",
                    "Enable DXF eligibility for at least one fabrication-relevant component."));
            }

            string validationMode = GetString(values, "validation.mode");
            bool? stopOnCriticalFailure = GetBoolean(values, "validation.stopOnCriticalFailure");
            if (string.Equals(validationMode, "StrictRelease", StringComparison.Ordinal) &&
                stopOnCriticalFailure.HasValue &&
                !stopOnCriticalFailure.Value)
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.CrossField,
                    ValidationSeverity.Critical,
                    true,
                    "CFG-REL-001",
                    "VAL-CFG-STRICT-RELEASE",
                    "validation.stopOnCriticalFailure",
                    "validation.mode",
                    "StrictRelease mode requires stopOnCriticalFailure to be true.",
                    "validation.stopOnCriticalFailure=true",
                    "validation.stopOnCriticalFailure=false",
                    "Set stopOnCriticalFailure to true for StrictRelease mode."));
            }
        }

        private static void ValidateMinLessOrEqual(
            IDictionary<string, object> values,
            IList<ValidationMessage> messages,
            string minKey,
            string maxKey,
            string ruleId,
            string validatorId,
            string scope)
        {
            decimal? minValue = GetDecimal(values, minKey);
            decimal? maxValue = GetDecimal(values, maxKey);
            if (!minValue.HasValue || !maxValue.HasValue)
            {
                return;
            }

            if (minValue.Value > maxValue.Value)
            {
                messages.Add(CreateMessage(
                    ValidatorGroup.CrossField,
                    ValidationSeverity.Critical,
                    true,
                    ruleId,
                    validatorId,
                    minKey + " / " + maxKey,
                    scope,
                    "Minimum value must be less than or equal to maximum value.",
                    minKey + " <= " + maxKey,
                    minValue.Value.ToString(CultureInfo.InvariantCulture) + " > " + maxValue.Value.ToString(CultureInfo.InvariantCulture),
                    "Swap or correct the configured min/max values."));
            }
        }

        private static decimal? GetDecimal(IDictionary<string, object> values, string key)
        {
            object value;
            if (values.TryGetValue(key, out value) && value is decimal)
            {
                return (decimal)value;
            }

            return null;
        }

        private static int? GetInteger(IDictionary<string, object> values, string key)
        {
            object value;
            if (values.TryGetValue(key, out value) && value is int)
            {
                return (int)value;
            }

            return null;
        }

        private static bool? GetBoolean(IDictionary<string, object> values, string key)
        {
            object value;
            if (values.TryGetValue(key, out value) && value is bool)
            {
                return (bool)value;
            }

            return null;
        }

        private static string GetString(IDictionary<string, object> values, string key)
        {
            object value;
            if (values.TryGetValue(key, out value) && value is string)
            {
                return (string)value;
            }

            return string.Empty;
        }

        private static List<decimal> GetDecimalList(IDictionary<string, object> values, string key)
        {
            object value;
            if (values.TryGetValue(key, out value))
            {
                List<decimal> list = value as List<decimal>;
                if (list != null)
                {
                    return list;
                }
            }

            return null;
        }

        private static bool DecimalListsEqual(IList<decimal> left, IList<decimal> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            for (int i = 0; i < left.Count; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ContainsString(IList<string> list, string value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i], value, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsDecimal(IList<decimal> list, decimal value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == value)
                {
                    return true;
                }
            }

            return false;
        }

        private static string FormatExpectedFromDefinition(SchemaFieldDefinition field)
        {
            if (field.ValueType == ConfigValueType.Decimal && field.MinDecimal.HasValue && field.MaxDecimal.HasValue)
            {
                return field.MinDecimal.Value.ToString(CultureInfo.InvariantCulture) + " to " + field.MaxDecimal.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (field.ValueType == ConfigValueType.Integer && field.MinInteger.HasValue && field.MaxInteger.HasValue)
            {
                return field.MinInteger.Value.ToString(CultureInfo.InvariantCulture) + " to " + field.MaxInteger.Value.ToString(CultureInfo.InvariantCulture);
            }

            if (field.ValueType == ConfigValueType.Enum || field.ValueType == ConfigValueType.ArrayString)
            {
                return FormatValue(field.AllowedStringValues);
            }

            if (field.ValueType == ConfigValueType.ArrayDecimal)
            {
                return FormatValue(field.AllowedDecimalValues);
            }

            return FormatValue(field.DefaultValue);
        }

        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return "<null>";
            }

            if (value is string)
            {
                return "\"" + (string)value + "\"";
            }

            if (value is bool)
            {
                return ((bool)value) ? "true" : "false";
            }

            if (value is decimal)
            {
                return ((decimal)value).ToString(CultureInfo.InvariantCulture);
            }

            if (value is int)
            {
                return ((int)value).ToString(CultureInfo.InvariantCulture);
            }

            IList<string> stringList = value as IList<string>;
            if (stringList != null)
            {
                return "[" + JoinStrings(stringList) + "]";
            }

            IList<decimal> decimalList = value as IList<decimal>;
            if (decimalList != null)
            {
                return "[" + JoinDecimals(decimalList) + "]";
            }

            IList genericList = value as IList;
            if (genericList != null)
            {
                List<string> parts = new List<string>();
                for (int i = 0; i < genericList.Count; i++)
                {
                    parts.Add(FormatValue(genericList[i]));
                }

                return "[" + string.Join(", ", parts.ToArray()) + "]";
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static string JoinStrings(IList<string> values)
        {
            string[] items = new string[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                items[i] = values[i];
            }

            return string.Join(", ", items);
        }

        private static string JoinDecimals(IList<decimal> values)
        {
            string[] items = new string[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                items[i] = values[i].ToString(CultureInfo.InvariantCulture);
            }

            return string.Join(", ", items);
        }

        private static ValidationMessage CreateMessage(
            ValidatorGroup group,
            ValidationSeverity severity,
            bool blocking,
            string ruleId,
            string validatorId,
            string configKey,
            string artifactScope,
            string message,
            string expected,
            string actual,
            string recommendedAction)
        {
            return new ValidationMessage(
                severity,
                ruleId,
                validatorId,
                configKey,
                artifactScope,
                message,
                expected,
                actual,
                recommendedAction,
                blocking,
                group);
        }

        private static void SortMessages(List<ValidationMessage> messages)
        {
            messages.Sort(CompareMessages);
        }

        private static int CompareMessages(ValidationMessage left, ValidationMessage right)
        {
            int groupCompare = left.Group.CompareTo(right.Group);
            if (groupCompare != 0)
            {
                return groupCompare;
            }

            int ruleCompare = string.Compare(left.RuleId, right.RuleId, StringComparison.Ordinal);
            if (ruleCompare != 0)
            {
                return ruleCompare;
            }

            int validatorCompare = string.Compare(left.ValidatorId, right.ValidatorId, StringComparison.Ordinal);
            if (validatorCompare != 0)
            {
                return validatorCompare;
            }

            int keyCompare = string.Compare(left.ConfigKey, right.ConfigKey, StringComparison.Ordinal);
            if (keyCompare != 0)
            {
                return keyCompare;
            }

            return string.Compare(left.Message, right.Message, StringComparison.Ordinal);
        }
    }
}
