using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        private readonly IDictionary<string, object> _initialSettings;

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
                "PropertyManagerPage creation failed. Status code: " + errors.ToString(),
                (int)swMessageBoxIcon_e.swMbStop,
                (int)swMessageBoxBtn_e.swMbOk);
        }

        private void AddControls()
        {
            swPropertyPage.SetMessage3(
                "Values are loaded from Config/GlobalParams.json when available; otherwise validated code defaults are used.",
                (int)swPropertyManagerPageMessageVisibility.swImportantMessageBox,
                (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                "Configuration Baseline");

            _geometryTab = swPropertyPage.AddTab(GeometryTabId, "Geometry", string.Empty, 0);
            _runtimeTab = swPropertyPage.AddTab(RuntimeTabId, "Runtime", string.Empty, 0);

            int groupOptions = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                               (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;

            _frameGroup = (IPropertyManagerPageGroup)_geometryTab.AddGroupBox(FrameGroupId, "Frame Defaults", groupOptions);
            _pivotGroup = (IPropertyManagerPageGroup)_geometryTab.AddGroupBox(PivotGroupId, "Pivot Defaults", groupOptions);
            _heightGroup = (IPropertyManagerPageGroup)_geometryTab.AddGroupBox(HeightGroupId, "Height Defaults", groupOptions);
            _plateBraceGroup = (IPropertyManagerPageGroup)_geometryTab.AddGroupBox(PlateBraceGroupId, "Plate/Brace Defaults", groupOptions);
            _runtimeGroup = (IPropertyManagerPageGroup)_runtimeTab.AddGroupBox(RuntimeGroupId, "Exports/Validation/Run Defaults", groupOptions);

            AddFrameDefaults();
            AddPivotDefaults();
            AddHeightDefaults();
            AddPlateBraceDefaults();
            AddRuntimeDefaults();
        }

        private void AddFrameDefaults()
        {
            AddSettingTextbox(_frameGroup, "frame.layout.primary.memberExtentMin", "mm");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.memberExtentMax", "mm");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.placementTolerance", "mm");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.tableWidth", "mm");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.tableHeight", "mm");
            AddSettingTextbox(_frameGroup, "frame.profile.selection.allowedProfiles", "approved baseline profile set");
            AddSettingTextbox(_frameGroup, "frame.profile.selection.dimensionTolerance", "mm");
            AddSettingTextbox(_frameGroup, "frame.naming.ruleSet", "deterministic naming baseline");
        }

        private void AddPivotDefaults()
        {
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.axisLocationMin", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.axisLocationMax", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.alignmentTolerance", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.diameterMin", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.diameterMax", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.positionTolerance", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.naming.mates", "deterministic naming baseline");
        }

        private void AddHeightDefaults()
        {
            AddSettingTextbox(_heightGroup, "height.supportedConfigurations.values", "mm");
            AddSettingTextbox(_heightGroup, "height.indexing.activation.requiredCount", "count");
            AddSettingTextbox(_heightGroup, "height.indexing.activation.strictDeterminism", "boolean");
            AddSettingTextbox(_heightGroup, "height.validation.supportedSet", "mm");
            AddSettingTextbox(_heightGroup, "height.validation.dimensionTolerance", "mm");
        }

        private void AddPlateBraceDefaults()
        {
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.thicknessMin", "mm");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.thicknessMax", "mm");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.dimensionTolerance", "mm");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.export.dxfEligible", "boolean");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.naming.ruleSet", "deterministic naming baseline");
        }

        private void AddRuntimeDefaults()
        {
            AddSettingTextbox(_runtimeGroup, "exports.step.enabled", "boolean");
            AddSettingTextbox(_runtimeGroup, "exports.dxf.enabled", "boolean");
            AddSettingTextbox(_runtimeGroup, "exports.bom.enabled", "boolean");
            AddSettingTextbox(_runtimeGroup, "exports.validationReport.enabled", "boolean");
            AddSettingTextbox(_runtimeGroup, "validation.mode", "BuildOnly, FinalOutput, StrictRelease");
            AddSettingTextbox(_runtimeGroup, "validation.stopOnCriticalFailure", "boolean");
            AddSettingTextbox(_runtimeGroup, "run.packageOutputs", "boolean");
        }

        private IPropertyManagerPageTextbox AddSettingTextbox(
            IPropertyManagerPageGroup group,
            string key,
            string tooltip)
        {
            short controlType = (short)swPropertyManagerPageControlType_e.swControlType_Textbox;
            short align = (short)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            int options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                          (int)swAddControlOptions_e.swControlOptions_Visible;

            int controlId = _nextControlId++;
            IPropertyManagerPageTextbox textbox = (IPropertyManagerPageTextbox)group.AddControl(
                controlId,
                controlType,
                key,
                align,
                options,
                tooltip);

            if (textbox != null)
            {
                textbox.Text = ResolveDisplayValue(key);
                _settingsKeyByControlId[controlId] = key;
                _textboxBySettingsKey[key] = textbox;
            }

            return textbox;
        }

        public void Show()
        {
            if (swPropertyPage != null)
            {
                swPropertyPage.Show();
            }
        }

        public IDictionary<string, string> CaptureRuntimeOverrides()
        {
            Dictionary<string, string> overrides = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, IPropertyManagerPageTextbox> entry in _textboxBySettingsKey)
            {
                overrides[entry.Key] = entry.Value == null ? string.Empty : (entry.Value.Text ?? string.Empty).Trim();
            }

            return overrides;
        }

        public bool TryResolveSettingsKey(int controlId, out string key)
        {
            return _settingsKeyByControlId.TryGetValue(controlId, out key);
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
            object value;
            if (!_initialSettings.TryGetValue(key, out value) || value == null)
            {
                return string.Empty;
            }

            List<string> stringValues = value as List<string>;
            if (stringValues != null)
            {
                return string.Join(",", stringValues.ToArray());
            }

            List<decimal> decimalValues = value as List<decimal>;
            if (decimalValues != null)
            {
                return string.Join(",", decimalValues.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray());
            }

            decimal decimalValue;
            if (value is decimal)
            {
                decimalValue = (decimal)value;
                return decimalValue.ToString(CultureInfo.InvariantCulture);
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}
