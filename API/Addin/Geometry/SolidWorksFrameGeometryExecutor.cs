using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;

namespace AxionFrame
{
    public sealed class SolidWorksFrameGeometryExecutor : IFrameGeometryExecutor
    {
        private const double MillimetersToMeters = 0.001d;
        private const decimal MinimumFeatureDepthMillimeters = 0.1m;

        private static readonly string[] SketchPlaneCandidates =
        {
            "Front Plane",
            "Top Plane",
            "Right Plane"
        };

        private readonly ISldWorks _swApp;

        public SolidWorksFrameGeometryExecutor(ISldWorks swApp)
        {
            if (swApp == null)
            {
                throw new ArgumentNullException(nameof(swApp));
            }

            _swApp = swApp;
        }

        public FrameGeometryResult Generate(FrameGeometryRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string partTemplatePath = _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
            if (string.IsNullOrWhiteSpace(partTemplatePath))
            {
                throw new InvalidOperationException("Default SolidWorks part template is not configured.");
            }

            IModelDoc2 model = (IModelDoc2)_swApp.NewDocument(partTemplatePath, (int)swDwgPaperSizes_e.swDwgPaperA2size, 0.0, 0.0);
            if (model == null)
            {
                throw new InvalidOperationException("SolidWorks failed to create a new part document for frame geometry.");
            }

            CreateExtrudedRectangleFeature(
                model,
                request.LayoutFeatureName,
                request.MemberExtentMax,
                request.MemberExtentMin,
                Max(request.PlacementTolerance, MinimumFeatureDepthMillimeters));

            CreateExtrudedRectangleFeature(
                model,
                request.ProfileFeatureName,
                request.SelectedProfile.WidthMillimeters,
                request.SelectedProfile.HeightMillimeters,
                request.MemberExtentMax);

            model.ClearSelection2(true);
            model.ForceRebuild3(false);

            string note = "Frame layout/profile geometry generated using profile " + request.SelectedProfileCode + ".";
            return new FrameGeometryResult(true, model.GetTitle(), note);
        }

        private static void CreateExtrudedRectangleFeature(IModelDoc2 model, string featureName, decimal widthMillimeters, decimal heightMillimeters, decimal depthMillimeters)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(featureName))
            {
                throw new InvalidOperationException("Target feature name cannot be null or whitespace.");
            }

            if (widthMillimeters <= 0m || heightMillimeters <= 0m || depthMillimeters <= 0m)
            {
                throw new InvalidOperationException("Frame geometry dimensions must be greater than zero.");
            }

            model.ClearSelection2(true);
            if (!TrySelectSketchPlane(model))
            {
                throw new InvalidOperationException("Unable to select a sketch plane for frame feature generation.");
            }

            model.InsertSketch2(true);

            double halfWidth = ToMeters(widthMillimeters) / 2.0d;
            double halfHeight = ToMeters(heightMillimeters) / 2.0d;
            model.SketchRectangle(-halfWidth, -halfHeight, 0.0d, halfWidth, halfHeight, 0.0d, false);

            model.InsertSketch2(true);

            IFeatureManager featureManager = model.FeatureManager;
            object featureObject = featureManager.FeatureExtrusion(
                true,
                false,
                false,
                (int)swEndConditions_e.swEndCondBlind,
                (int)swEndConditions_e.swEndCondBlind,
                ToMeters(depthMillimeters),
                0.0d,
                false,
                false,
                false,
                false,
                0.0d,
                0.0d,
                false,
                false,
                false,
                false,
                true,
                false,
                false);
            IFeature feature = featureObject as IFeature;

            if (feature == null)
            {
                throw new InvalidOperationException("SolidWorks failed to create feature '" + featureName + "'.");
            }

            feature.Name = featureName;
        }

        private static bool TrySelectSketchPlane(IModelDoc2 model)
        {
            IModelDocExtension extension = model.Extension;
            for (int i = 0; i < SketchPlaneCandidates.Length; i++)
            {
                bool selected = extension.SelectByID2(
                    SketchPlaneCandidates[i],
                    "PLANE",
                    0.0d,
                    0.0d,
                    0.0d,
                    false,
                    0,
                    null,
                    0);

                if (selected)
                {
                    return true;
                }
            }

            return false;
        }

        private static double ToMeters(decimal valueMillimeters)
        {
            return (double)valueMillimeters * MillimetersToMeters;
        }

        private static decimal Max(decimal left, decimal right)
        {
            if (left >= right)
            {
                return left;
            }

            return right;
        }
    }
}
