using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AxionFrame
{
    public sealed class SettingsPage
    {
        private const string PageTitle = "AxionFrame Settings";
        private const int GeometryTabId = 11;
        private const int RuntimeTabId = 12;
        private const int FrameGroupId = 101;
        private const int PivotGroupId = 102;
        private const int HeightGroupId = 103;
        private const int PlateBraceGroupId = 104;
        private const int RuntimeGroupId = 105;

        private const string ConfigProfileLibraryPath = "frame.profile.library.path";
        private const string ConfigProfileSelectionStandard = "frame.profile.selection.standard";
        private const string ConfigProfileSelectionType = "frame.profile.selection.type";
        private const string ConfigProfileSelectionSize = "frame.profile.selection.size";
        private const string ConfigAllowedProfiles = "frame.profile.selection.allowedProfiles";

        private int _nextControlId = 1000;
        private readonly ISldWorks _swApp;
        private readonly SwAddin _userAddin;

        public IPropertyManagerPage2 swPropertyPage = null;
        private SettingsHandler _handler;
        private IPropertyManagerPageTab _geometryTab;
        private IPropertyManagerPageTab _runtimeTab;
        private IPropertyManagerPageGroup _frameGroup;
        private IPropertyManagerPageGroup _pivotGroup;
        private IPropertyManagerPageGroup _heightGroup;
        private IPropertyManagerPageGroup _plateBraceGroup;
        private IPropertyManagerPageGroup _runtimeGroup;
        private readonly Dictionary<int, string> _settingsKeyByControlId = new Dictionary<int, string>();
        private readonly Dictionary<string, IPropertyManagerPageTextbox> _textboxBySettingsKey = new Dictionary<string, IPropertyManagerPageTextbox>(StringComparer.Ordinal);
        private readonly Dictionary<int, string> _comboboxKeyByControlId = new Dictionary<int, string>();
        private readonly Dictionary<int, List<string>> _comboboxItemsByControlId = new Dictionary<int, List<string>>();
        private readonly Dictionary<string, IPropertyManagerPageCombobox> _comboboxBySettingsKey = new Dictionary<string, IPropertyManagerPageCombobox>(StringComparer.Ordinal);
        private readonly IDictionary<string, object> _initialSettings;
        private readonly Dictionary<string, string> _runtimeSettingsText = new Dictionary<string, string>(StringComparer.Ordinal);
        private bool _isInternalValueUpdate;

        public SettingsPage(SwAddin addin)
        {
            if (addin == null)
            {
                throw new ArgumentNullException(nameof(addin));
            }

            _userAddin = addin;
            _swApp = (ISldWorks)_userAddin.SwApp;
            if (_swApp == null)
            {
                throw new InvalidOperationException("SolidWorks application reference is not available.");
            }

            _initialSettings = LoadInitialSettings();
            CreatePropertyManagerPage();
        }

        private void CreatePropertyManagerPage()
        {
            int errors = -1;
            int options = (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton |
                          (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton;

            _handler = new SettingsHandler(_userAddin, this);
            swPropertyPage = (IPropertyManagerPage2)_swApp.CreatePropertyManagerPage(PageTitle, options, _handler, ref errors);
            if (swPropertyPage != null && errors == (int)swPropertyManagerPageStatus_e.swPropertyManagerPage_Okay)
            {
                AddControls();
                return;
            }

            _swApp.SendMsgToUser2(
                "PropertyManagerPage creation failed. Status code: " + errors.ToString(CultureInfo.InvariantCulture),
                (int)swMessageBoxIcon_e.swMbStop,
                (int)swMessageBoxBtn_e.swMbOk);
        }

        private void AddControls()
        {
            swPropertyPage.SetMessage3(
                "Configuration source priority: Config/GlobalParams.json first, validated defaults second.",
                (int)swPropertyManagerPageMessageVisibility.swImportantMessageBox,
                (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                "Source Priority");

            _geometryTab = swPropertyPage.AddTab(GeometryTabId, "Geometry", string.Empty, 0);
            _runtimeTab = swPropertyPage.AddTab(RuntimeTabId, "Runtime", string.Empty, 0);

            int groupOptions = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                               (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;

            _frameGroup = (IPropertyManagerPageGroup)_geometryTab.AddGroupBox(FrameGroupId, "Frame Settings", groupOptions);
            _pivotGroup = (IPropertyManagerPageGroup)_geometryTab.AddGroupBox(PivotGroupId, "Pivot Settings", groupOptions);
            _heightGroup = (IPropertyManagerPageGroup)_geometryTab.AddGroupBox(HeightGroupId, "Height Settings", groupOptions);
            _plateBraceGroup = (IPropertyManagerPageGroup)_geometryTab.AddGroupBox(PlateBraceGroupId, "Plate/Brace Settings", groupOptions);
            _runtimeGroup = (IPropertyManagerPageGroup)_runtimeTab.AddGroupBox(RuntimeGroupId, "Export/Validation/Run Settings", groupOptions);

            AddFrameSettings();
            AddPivotSettings();
            AddHeightSettings();
            AddPlateBraceSettings();
            AddRuntimeSettings();
            RefreshProfileSelectionFromLibrary(true);
        }

        private void AddFrameSettings()
        {
            AddSettingTextbox(_frameGroup, "frame.layout.primary.memberExtentMin", "Frame Minimum Member Extent (mm)", "Minimum frame member extent.");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.memberExtentMax", "Frame Maximum Member Extent (mm)", "Maximum frame member extent.");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.placementTolerance", "Frame Placement Tolerance (mm)", "Placement tolerance.");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.tableWidth", "Table Width (mm)", "Overall table width.");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.tableHeight", "Table Height (mm)", "Overall table height.");

            AddSettingTextbox(_frameGroup, ConfigProfileLibraryPath, "Profile Library Address", "SolidWorks weldment profile library folder path.");
            AddSettingCombobox(_frameGroup, ConfigProfileSelectionStandard, "Profile Standard", "Select profile standard from library.");
            AddSettingCombobox(_frameGroup, ConfigProfileSelectionType, "Profile Type", "Select profile type from library.");
            AddSettingCombobox(_frameGroup, ConfigProfileSelectionSize, "Profile Size", "Select profile size from library.");
            AddSettingTextbox(_frameGroup, ConfigAllowedProfiles, "Selected Profile Code (CSV)", "Resolved profile code list used at build time.", false);
            AddSettingTextbox(_frameGroup, "frame.profile.selection.dimensionTolerance", "Profile Dimension Tolerance (mm)", "Allowed profile dimension tolerance.");
            AddSettingTextbox(_frameGroup, "frame.naming.ruleSet", "Frame Naming Rule Set", "Deterministic naming baseline.");
        }

        private void AddPivotSettings()
        {
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.axisLocationMin", "Pivot Axis Location Min (mm)", "Minimum axis location.");
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.axisLocationMax", "Pivot Axis Location Max (mm)", "Maximum axis location.");
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.alignmentTolerance", "Pivot Alignment Tolerance (mm)", "Axis alignment tolerance.");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.diameterMin", "Pivot Hole Diameter Min (mm)", "Minimum hole diameter.");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.diameterMax", "Pivot Hole Diameter Max (mm)", "Maximum hole diameter.");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.positionTolerance", "Pivot Hole Position Tolerance (mm)", "Hole position tolerance.");
            AddSettingTextbox(_pivotGroup, "pivot.naming.mates", "Pivot Naming Rule Set", "Deterministic naming baseline.");
        }

        private void AddHeightSettings()
        {
            AddSettingTextbox(_heightGroup, "height.supportedConfigurations.values", "Supported Heights (CSV mm)", "Supported heights in millimeters.");
            AddSettingTextbox(_heightGroup, "height.indexing.activation.requiredCount", "Height Required Count", "Expected number of heights.");
            AddSettingTextbox(_heightGroup, "height.indexing.activation.strictDeterminism", "Strict Determinism (true/false)", "Enforce deterministic activation.");
            AddSettingTextbox(_heightGroup, "height.validation.supportedSet", "Height Validation Set (CSV mm)", "Validation set for heights.");
            AddSettingTextbox(_heightGroup, "height.validation.dimensionTolerance", "Height Dimension Tolerance (mm)", "Height validation tolerance.");
        }

        private void AddPlateBraceSettings()
        {
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.thicknessMin", "Plate/Brace Thickness Min (mm)", "Minimum plate/brace thickness.");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.thicknessMax", "Plate/Brace Thickness Max (mm)", "Maximum plate/brace thickness.");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.dimensionTolerance", "Plate/Brace Dimension Tolerance (mm)", "Dimension tolerance.");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.export.dxfEligible", "DXF Eligible (true/false)", "Whether DXF export is eligible.");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.naming.ruleSet", "Plate/Brace Naming Rule Set", "Deterministic naming baseline.");
        }

        private void AddRuntimeSettings()
        {
            AddSettingTextbox(_runtimeGroup, "exports.step.enabled", "Export STEP Enabled (true/false)", "Enable STEP output.");
            AddSettingTextbox(_runtimeGroup, "exports.dxf.enabled", "Export DXF Enabled (true/false)", "Enable DXF output.");
            AddSettingTextbox(_runtimeGroup, "exports.bom.enabled", "Export BOM Enabled (true/false)", "Enable BOM output.");
            AddSettingTextbox(_runtimeGroup, "exports.validationReport.enabled", "Validation Report Enabled (true/false)", "Enable validation report output.");
            AddSettingTextbox(_runtimeGroup, "validation.mode", "Validation Mode", "BuildOnly, FinalOutput, StrictRelease.");
            AddSettingTextbox(_runtimeGroup, "validation.stopOnCriticalFailure", "Stop On Critical Failure (true/false)", "Stop build on critical validation failure.");
            AddSettingTextbox(_runtimeGroup, "run.packageOutputs", "Package Outputs (true/false)", "Package output artifacts.");
        }

        private IPropertyManagerPageTextbox AddSettingTextbox(
            IPropertyManagerPageGroup group,
            string key,
            string label,
            string tooltip)
        {
            return AddSettingTextbox(group, key, label, tooltip, true);
        }

        private IPropertyManagerPageTextbox AddSettingTextbox(
            IPropertyManagerPageGroup group,
            string key,
            string label,
            string tooltip,
            bool isEditable)
        {
            short controlType = (short)swPropertyManagerPageControlType_e.swControlType_Textbox;
            short align = (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            int options = (int)swAddControlOptions_e.swControlOptions_Visible;
            if (isEditable)
            {
                options |= (int)swAddControlOptions_e.swControlOptions_Enabled;
            }

            int controlId = _nextControlId++;
            AddLabelControl(group, label, key);
            IPropertyManagerPageTextbox textbox = (IPropertyManagerPageTextbox)group.AddControl(
                controlId,
                controlType,
                string.Empty,
                align,
                options,
                tooltip + " | config key: " + key);

            if (textbox != null)
            {
                string value = ResolveDisplayValue(key);
                textbox.Text = value;
                _settingsKeyByControlId[controlId] = key;
                _textboxBySettingsKey[key] = textbox;
                _runtimeSettingsText[key] = value;
            }

            return textbox;
        }

        private IPropertyManagerPageCombobox AddSettingCombobox(
            IPropertyManagerPageGroup group,
            string key,
            string label,
            string tooltip)
        {
            short controlType = (short)swPropertyManagerPageControlType_e.swControlType_Combobox;
            short align = (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            int options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;

            int controlId = _nextControlId++;
            AddLabelControl(group, label, key);
            IPropertyManagerPageCombobox combobox = (IPropertyManagerPageCombobox)group.AddControl(
                controlId,
                controlType,
                string.Empty,
                align,
                options,
                tooltip + " | config key: " + key);

            if (combobox != null)
            {
                _comboboxKeyByControlId[controlId] = key;
                _comboboxBySettingsKey[key] = combobox;
                _comboboxItemsByControlId[controlId] = new List<string>();
                string value = ResolveDisplayValue(key);
                _runtimeSettingsText[key] = value;
            }

            return combobox;
        }

        private void AddLabelControl(IPropertyManagerPageGroup group, string label, string key)
        {
            short controlType = (short)swPropertyManagerPageControlType_e.swControlType_Label;
            short align = (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            int options = (int)swAddControlOptions_e.swControlOptions_Visible;

            int labelId = _nextControlId++;
            group.AddControl(
                labelId,
                controlType,
                label,
                align,
                options,
                "Setting label | config key: " + key);
        }

        public void Show()
        {
            ApplyRuntimeStateToControls();
            RefreshProfileSelectionFromLibrary(true);
            if (swPropertyPage != null)
            {
                swPropertyPage.Show();
            }
        }

        public IDictionary<string, string> CaptureRuntimeOverrides()
        {
            RefreshRuntimeStateFromControls();
            Dictionary<string, string> overrides = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, string> entry in _runtimeSettingsText)
            {
                overrides[entry.Key] = entry.Value == null ? string.Empty : entry.Value.Trim();
            }

            return overrides;
        }

        public bool TryResolveSettingsKey(int controlId, out string key)
        {
            return _settingsKeyByControlId.TryGetValue(controlId, out key);
        }

        public void UpdateRuntimeValueByControlId(int controlId, string value)
        {
            string key;
            if (!_settingsKeyByControlId.TryGetValue(controlId, out key))
            {
                return;
            }

            if (_isInternalValueUpdate)
            {
                return;
            }

            string normalizedValue = value == null ? string.Empty : value.Trim();
            _runtimeSettingsText[key] = normalizedValue;

            IPropertyManagerPageTextbox textbox;
            if (_textboxBySettingsKey.TryGetValue(key, out textbox) && textbox != null && !string.Equals(textbox.Text, normalizedValue, StringComparison.Ordinal))
            {
                textbox.Text = normalizedValue;
            }

            if (string.Equals(key, ConfigProfileLibraryPath, StringComparison.Ordinal) ||
                string.Equals(key, ConfigProfileSelectionStandard, StringComparison.Ordinal) ||
                string.Equals(key, ConfigProfileSelectionType, StringComparison.Ordinal) ||
                string.Equals(key, ConfigProfileSelectionSize, StringComparison.Ordinal))
            {
                RefreshProfileSelectionFromLibrary(false);
            }
        }

        public void UpdateRuntimeValueFromComboboxByControlId(int controlId, int selectedIndex)
        {
            if (_isInternalValueUpdate)
            {
                return;
            }

            string key;
            if (!_comboboxKeyByControlId.TryGetValue(controlId, out key))
            {
                return;
            }

            List<string> items;
            if (!_comboboxItemsByControlId.TryGetValue(controlId, out items) || items == null || selectedIndex < 0 || selectedIndex >= items.Count)
            {
                return;
            }

            _runtimeSettingsText[key] = items[selectedIndex];

            if (string.Equals(key, ConfigProfileSelectionStandard, StringComparison.Ordinal) ||
                string.Equals(key, ConfigProfileSelectionType, StringComparison.Ordinal) ||
                string.Equals(key, ConfigProfileSelectionSize, StringComparison.Ordinal))
            {
                RefreshProfileSelectionFromLibrary(false);
            }
        }

        private IDictionary<string, object> LoadInitialSettings()
        {
            try
            {
                string configPath = BuildWorkflowEngine.ResolveConfigurationPath(null);
                FeatureManager featureManager = new FeatureManager();
                ConfigurationProcessingResult result = featureManager.LoadAndValidate(configPath);
                if (result != null && result.NormalizedConfig != null)
                {
                    return result.NormalizedConfig;
                }
            }
            catch
            {
            }

            return new Dictionary<string, object>(StringComparer.Ordinal);
        }

        private string ResolveDisplayValue(string key)
        {
            string runtimeValue;
            if (_runtimeSettingsText.TryGetValue(key, out runtimeValue))
            {
                return runtimeValue;
            }

            object value;
            if (_initialSettings.TryGetValue(key, out value) && value != null)
            {
                List<string> stringValues = value as List<string>;
                if (stringValues != null)
                {
                    return JoinStringList(stringValues);
                }

                List<decimal> decimalValues = value as List<decimal>;
                if (decimalValues != null)
                {
                    return JoinDecimalList(decimalValues);
                }

                decimal decimalValue;
                if (value is decimal)
                {
                    decimalValue = (decimal)value;
                    return decimalValue.ToString(CultureInfo.InvariantCulture);
                }

                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }

            if (string.Equals(key, ConfigProfileLibraryPath, StringComparison.Ordinal))
            {
                return ResolveDefaultProfileLibraryPath();
            }

            return string.Empty;
        }

        private void ApplyRuntimeStateToControls()
        {
            foreach (KeyValuePair<string, IPropertyManagerPageTextbox> entry in _textboxBySettingsKey)
            {
                if (entry.Value == null)
                {
                    continue;
                }

                string value;
                if (_runtimeSettingsText.TryGetValue(entry.Key, out value))
                {
                    entry.Value.Text = value;
                }
            }

            foreach (KeyValuePair<string, IPropertyManagerPageCombobox> entry in _comboboxBySettingsKey)
            {
                SetComboboxSelectionByValue(entry.Key, GetRuntimeValue(entry.Key));
            }
        }

        private void RefreshRuntimeStateFromControls()
        {
            foreach (KeyValuePair<string, IPropertyManagerPageTextbox> entry in _textboxBySettingsKey)
            {
                if (entry.Value == null)
                {
                    continue;
                }

                _runtimeSettingsText[entry.Key] = entry.Value.Text == null ? string.Empty : entry.Value.Text.Trim();
            }

            foreach (KeyValuePair<int, string> entry in _comboboxKeyByControlId)
            {
                List<string> items;
                if (!_comboboxItemsByControlId.TryGetValue(entry.Key, out items) || items == null || items.Count == 0)
                {
                    continue;
                }

                int selectedIndex = GetComboboxSelectedIndex(entry.Key);
                if (selectedIndex >= 0 && selectedIndex < items.Count)
                {
                    _runtimeSettingsText[entry.Value] = items[selectedIndex];
                }
            }
        }

        private void RefreshProfileSelectionFromLibrary(bool initializeFromResolvedProfileCode)
        {
            string profileLibraryPath = GetRuntimeValue(ConfigProfileLibraryPath);
            if (string.IsNullOrWhiteSpace(profileLibraryPath))
            {
                profileLibraryPath = ResolveDefaultProfileLibraryPath();
            }

            if (!string.IsNullOrWhiteSpace(profileLibraryPath))
            {
                SetRuntimeValue(ConfigProfileLibraryPath, profileLibraryPath);
            }

            List<ProfileLibraryEntry> profileEntries = LoadProfilesFromLibrary(profileLibraryPath);
            if (profileEntries.Count == 0)
            {
                return;
            }

            string selectedStandard = GetRuntimeValue(ConfigProfileSelectionStandard);
            string selectedType = GetRuntimeValue(ConfigProfileSelectionType);
            string selectedSize = GetRuntimeValue(ConfigProfileSelectionSize);

            if (initializeFromResolvedProfileCode)
            {
                TryResolveSelectionFromProfileCode(profileEntries, GetFirstAllowedProfileCode(), out selectedStandard, out selectedType, out selectedSize);
            }

            if (!ContainsStandard(profileEntries, selectedStandard))
            {
                selectedStandard = profileEntries[0].Standard;
            }

            List<ProfileLibraryEntry> typeCandidates = FilterByStandard(profileEntries, selectedStandard);
            if (!ContainsType(typeCandidates, selectedType))
            {
                selectedType = typeCandidates[0].Type;
            }

            List<ProfileLibraryEntry> sizeCandidates = FilterByType(typeCandidates, selectedType);
            if (!ContainsSize(sizeCandidates, selectedSize))
            {
                selectedSize = sizeCandidates[0].Size;
            }

            string profileCode = selectedSize + "_" + selectedType;

            BindProfileComboboxValues(profileEntries, selectedStandard, selectedType, selectedSize);
            SetRuntimeValue(ConfigProfileSelectionStandard, selectedStandard);
            SetRuntimeValue(ConfigProfileSelectionType, selectedType);
            SetRuntimeValue(ConfigProfileSelectionSize, selectedSize);
            SetRuntimeValue(ConfigAllowedProfiles, profileCode);
        }

        private void BindProfileComboboxValues(
            IList<ProfileLibraryEntry> profileEntries,
            string selectedStandard,
            string selectedType,
            string selectedSize)
        {
            List<string> standards = new List<string>();
            for (int i = 0; i < profileEntries.Count; i++)
            {
                if (!ContainsValue(standards, profileEntries[i].Standard))
                {
                    standards.Add(profileEntries[i].Standard);
                }
            }

            List<ProfileLibraryEntry> typeCandidates = FilterByStandard(profileEntries, selectedStandard);
            List<string> types = new List<string>();
            for (int i = 0; i < typeCandidates.Count; i++)
            {
                if (!ContainsValue(types, typeCandidates[i].Type))
                {
                    types.Add(typeCandidates[i].Type);
                }
            }

            List<ProfileLibraryEntry> sizeCandidates = FilterByType(typeCandidates, selectedType);
            List<string> sizes = new List<string>();
            for (int i = 0; i < sizeCandidates.Count; i++)
            {
                if (!ContainsValue(sizes, sizeCandidates[i].Size))
                {
                    sizes.Add(sizeCandidates[i].Size);
                }
            }

            SetComboboxItemsAndSelection(ConfigProfileSelectionStandard, standards, selectedStandard);
            SetComboboxItemsAndSelection(ConfigProfileSelectionType, types, selectedType);
            SetComboboxItemsAndSelection(ConfigProfileSelectionSize, sizes, selectedSize);
        }

        private static List<ProfileLibraryEntry> LoadProfilesFromLibrary(string libraryPath)
        {
            List<ProfileLibraryEntry> entries = new List<ProfileLibraryEntry>();
            if (string.IsNullOrWhiteSpace(libraryPath) || !Directory.Exists(libraryPath))
            {
                return entries;
            }

            string[] standardDirectories = Directory.GetDirectories(libraryPath);
            for (int standardIndex = 0; standardIndex < standardDirectories.Length; standardIndex++)
            {
                string standardDirectoryPath = standardDirectories[standardIndex];
                string standardName = Path.GetFileName(standardDirectoryPath);
                if (string.IsNullOrWhiteSpace(standardName))
                {
                    continue;
                }

                string[] typeDirectories = Directory.GetDirectories(standardDirectoryPath);
                for (int typeIndex = 0; typeIndex < typeDirectories.Length; typeIndex++)
                {
                    string typeDirectoryPath = typeDirectories[typeIndex];
                    string typeName = Path.GetFileName(typeDirectoryPath);
                    if (string.IsNullOrWhiteSpace(typeName))
                    {
                        continue;
                    }

                    string[] profileFiles = Directory.GetFiles(typeDirectoryPath, "*.sldlfp");
                    for (int profileIndex = 0; profileIndex < profileFiles.Length; profileIndex++)
                    {
                        string profileFilePath = profileFiles[profileIndex];
                        string sizeName = Path.GetFileNameWithoutExtension(profileFilePath);
                        if (string.IsNullOrWhiteSpace(sizeName))
                        {
                            continue;
                        }

                        entries.Add(new ProfileLibraryEntry(standardName, typeName, sizeName));
                    }
                }
            }

            entries.Sort(ProfileLibraryEntry.Compare);
            return entries;
        }

        private void TryResolveSelectionFromProfileCode(
            IList<ProfileLibraryEntry> entries,
            string profileCode,
            out string standard,
            out string type,
            out string size)
        {
            standard = string.Empty;
            type = string.Empty;
            size = string.Empty;

            if (entries == null || entries.Count == 0 || string.IsNullOrWhiteSpace(profileCode))
            {
                return;
            }

            int splitIndex = profileCode.LastIndexOf('_');
            if (splitIndex <= 0 || splitIndex >= profileCode.Length - 1)
            {
                return;
            }

            string profileSize = profileCode.Substring(0, splitIndex);
            string profileType = profileCode.Substring(splitIndex + 1);

            for (int i = 0; i < entries.Count; i++)
            {
                ProfileLibraryEntry entry = entries[i];
                if (string.Equals(entry.Type, profileType, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(entry.Size, profileSize, StringComparison.OrdinalIgnoreCase))
                {
                    standard = entry.Standard;
                    type = entry.Type;
                    size = entry.Size;
                    return;
                }
            }
        }

        private string GetFirstAllowedProfileCode()
        {
            string csv = GetRuntimeValue(ConfigAllowedProfiles);
            if (string.IsNullOrWhiteSpace(csv))
            {
                return string.Empty;
            }

            string[] tokens = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                return string.Empty;
            }

            return tokens[0].Trim();
        }

        private string ResolveDefaultProfileLibraryPath()
        {
            try
            {
                string commonData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);
                string[] candidates =
                {
                    Path.Combine(commonData, "SOLIDWORKS", "SOLIDWORKS 2026", "weldment profiles"),
                    Path.Combine(commonData, "SOLIDWORKS", "SOLIDWORKS 2025", "weldment profiles"),
                    Path.Combine(commonData, "SOLIDWORKS", "SOLIDWORKS 2024", "weldment profiles")
                };

                for (int i = 0; i < candidates.Length; i++)
                {
                    if (Directory.Exists(candidates[i]))
                    {
                        return candidates[i];
                    }
                }

                return candidates[0];
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetRuntimeValue(string key)
        {
            string value;
            return _runtimeSettingsText.TryGetValue(key, out value) ? value : string.Empty;
        }

        private void SetRuntimeValue(string key, string value)
        {
            string normalized = value == null ? string.Empty : value.Trim();
            _runtimeSettingsText[key] = normalized;

            IPropertyManagerPageTextbox textbox;
            if (!_textboxBySettingsKey.TryGetValue(key, out textbox) || textbox == null)
            {
                return;
            }

            if (string.Equals(textbox.Text, normalized, StringComparison.Ordinal))
            {
                return;
            }

            _isInternalValueUpdate = true;
            try
            {
                textbox.Text = normalized;
            }
            finally
            {
                _isInternalValueUpdate = false;
            }
        }

        private void SetComboboxItemsAndSelection(string key, IList<string> values, string selectedValue)
        {
            int controlId = GetComboboxControlIdByKey(key);
            if (controlId < 0)
            {
                return;
            }

            IPropertyManagerPageCombobox combo;
            if (!_comboboxBySettingsKey.TryGetValue(key, out combo) || combo == null)
            {
                return;
            }

            List<string> items = new List<string>();
            if (values != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    string value = values[i] == null ? string.Empty : values[i].Trim();
                    if (!string.IsNullOrWhiteSpace(value) && !ContainsValue(items, value))
                    {
                        items.Add(value);
                    }
                }
            }

            _comboboxItemsByControlId[controlId] = items;
            InvokeComboboxClear(combo);
            InvokeComboboxAddItems(combo, items);
            SetComboboxSelectionByValue(key, selectedValue);
        }

        private void SetComboboxSelectionByValue(string key, string selectedValue)
        {
            int controlId = GetComboboxControlIdByKey(key);
            if (controlId < 0)
            {
                return;
            }

            List<string> items;
            if (!_comboboxItemsByControlId.TryGetValue(controlId, out items) || items == null || items.Count == 0)
            {
                return;
            }

            int index = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (string.Equals(items[i], selectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }

            IPropertyManagerPageCombobox combo;
            if (_comboboxBySettingsKey.TryGetValue(key, out combo) && combo != null)
            {
                _isInternalValueUpdate = true;
                try
                {
                    SetComboboxSelectedIndex(combo, index);
                }
                finally
                {
                    _isInternalValueUpdate = false;
                }
            }
        }

        private int GetComboboxControlIdByKey(string key)
        {
            foreach (KeyValuePair<int, string> entry in _comboboxKeyByControlId)
            {
                if (string.Equals(entry.Value, key, StringComparison.Ordinal))
                {
                    return entry.Key;
                }
            }

            return -1;
        }

        private int GetComboboxSelectedIndex(int controlId)
        {
            string key;
            if (!_comboboxKeyByControlId.TryGetValue(controlId, out key))
            {
                return -1;
            }

            IPropertyManagerPageCombobox combo;
            if (!_comboboxBySettingsKey.TryGetValue(key, out combo) || combo == null)
            {
                return -1;
            }
            return combo.CurrentSelection;
        }

        private static void SetComboboxSelectedIndex(IPropertyManagerPageCombobox combo, int index)
        {
            combo.CurrentSelection = (short)index;
        }

        private static void InvokeComboboxClear(IPropertyManagerPageCombobox combo)
        {
            combo.Clear();
        }

        private static void InvokeComboboxAddItems(IPropertyManagerPageCombobox combo, IList<string> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            combo.AddItems(string.Join("|", items));
        }

        private static string JoinStringList(IList<string> values)
        {
            if (values == null || values.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(",", values);
        }

        private static string JoinDecimalList(IList<decimal> values)
        {
            if (values == null || values.Count == 0)
            {
                return string.Empty;
            }

            string[] tokens = new string[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                tokens[i] = values[i].ToString(CultureInfo.InvariantCulture);
            }

            return string.Join(",", tokens);
        }

        private static bool ContainsStandard(IList<ProfileLibraryEntry> entries, string standard)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (string.Equals(entries[i].Standard, standard, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsType(IList<ProfileLibraryEntry> entries, string type)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (string.Equals(entries[i].Type, type, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsSize(IList<ProfileLibraryEntry> entries, string size)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (string.Equals(entries[i].Size, size, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsValue(IList<string> values, string candidate)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (string.Equals(values[i], candidate, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static List<ProfileLibraryEntry> FilterByStandard(IList<ProfileLibraryEntry> entries, string standard)
        {
            List<ProfileLibraryEntry> result = new List<ProfileLibraryEntry>();
            for (int i = 0; i < entries.Count; i++)
            {
                if (string.Equals(entries[i].Standard, standard, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(entries[i]);
                }
            }

            return result;
        }

        private static List<ProfileLibraryEntry> FilterByType(IList<ProfileLibraryEntry> entries, string type)
        {
            List<ProfileLibraryEntry> result = new List<ProfileLibraryEntry>();
            for (int i = 0; i < entries.Count; i++)
            {
                if (string.Equals(entries[i].Type, type, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(entries[i]);
                }
            }

            return result;
        }

        private sealed class ProfileLibraryEntry
        {
            public ProfileLibraryEntry(string standard, string type, string size)
            {
                Standard = standard ?? string.Empty;
                Type = type ?? string.Empty;
                Size = size ?? string.Empty;
            }

            public string Standard { get; private set; }
            public string Type { get; private set; }
            public string Size { get; private set; }

            public static int Compare(ProfileLibraryEntry left, ProfileLibraryEntry right)
            {
                if (left == null && right == null)
                {
                    return 0;
                }

                if (left == null)
                {
                    return -1;
                }

                if (right == null)
                {
                    return 1;
                }

                int standard = string.Compare(left.Standard, right.Standard, StringComparison.OrdinalIgnoreCase);
                if (standard != 0)
                {
                    return standard;
                }

                int type = string.Compare(left.Type, right.Type, StringComparison.OrdinalIgnoreCase);
                if (type != 0)
                {
                    return type;
                }

                return string.Compare(left.Size, right.Size, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
