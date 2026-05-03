using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using System;

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
                "These defaults are loaded from the approved technical documentation baseline.",
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
            AddSettingTextbox(_frameGroup, "frame.layout.primary.memberExtentMin", "620.0", "mm");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.memberExtentMax", "980.0", "mm");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.placementTolerance", "0.5", "mm");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.tableWidth", "700.0", "mm");
            AddSettingTextbox(_frameGroup, "frame.layout.primary.tableHeight", "1000.0", "mm");
            AddSettingTextbox(_frameGroup, "frame.profile.selection.allowedProfiles", "40x40x2.0_SHS,60x30x2.0_RHS", "approved baseline profile set");
            AddSettingTextbox(_frameGroup, "frame.profile.selection.dimensionTolerance", "0.2", "mm");
            AddSettingTextbox(_frameGroup, "frame.naming.ruleSet", "AXF_STANDARD_V1", "deterministic naming baseline");
        }

        private void AddPivotDefaults()
        {
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.axisLocationMin", "300.0", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.axisLocationMax", "450.0", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.geometry.primary.alignmentTolerance", "0.25", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.diameterMin", "10.5", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.diameterMax", "11.0", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.hole.strategy.positionTolerance", "0.2", "mm");
            AddSettingTextbox(_pivotGroup, "pivot.naming.mates", "AXF_STANDARD_V1", "deterministic naming baseline");
        }

        private void AddHeightDefaults()
        {
            AddSettingTextbox(_heightGroup, "height.supportedConfigurations.values", "680.0,730.0,780.0", "mm");
            AddSettingTextbox(_heightGroup, "height.indexing.activation.requiredCount", "3", "count");
            AddSettingTextbox(_heightGroup, "height.indexing.activation.strictDeterminism", "true", "boolean");
            AddSettingTextbox(_heightGroup, "height.validation.supportedSet", "680.0,730.0,780.0", "mm");
            AddSettingTextbox(_heightGroup, "height.validation.dimensionTolerance", "1.0", "mm");
        }

        private void AddPlateBraceDefaults()
        {
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.thicknessMin", "5.0", "mm");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.thicknessMax", "8.0", "mm");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.dimensions.primary.dimensionTolerance", "0.2", "mm");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.export.dxfEligible", "true", "boolean");
            AddSettingTextbox(_plateBraceGroup, "plateBrace.naming.ruleSet", "AXF_STANDARD_V1", "deterministic naming baseline");
        }

        private void AddRuntimeDefaults()
        {
            AddSettingTextbox(_runtimeGroup, "exports.step.enabled", "true", "boolean");
            AddSettingTextbox(_runtimeGroup, "exports.dxf.enabled", "true", "boolean");
            AddSettingTextbox(_runtimeGroup, "exports.bom.enabled", "true", "boolean");
            AddSettingTextbox(_runtimeGroup, "exports.validationReport.enabled", "true", "boolean");
            AddSettingTextbox(_runtimeGroup, "validation.mode", "StrictRelease", "BuildOnly, FinalOutput, StrictRelease");
            AddSettingTextbox(_runtimeGroup, "validation.stopOnCriticalFailure", "true", "boolean");
            AddSettingTextbox(_runtimeGroup, "run.packageOutputs", "true", "boolean");
        }

        private IPropertyManagerPageTextbox AddSettingTextbox(
            IPropertyManagerPageGroup group,
            string key,
            string defaultValue,
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
                textbox.Text = defaultValue;
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
    }
}
