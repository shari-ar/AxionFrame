using SolidWorks.Interop.sldworks;
using System.IO;
using System.Runtime.InteropServices;
using System;

namespace AxionFrame
{
    public sealed class SolidWorksFrameGeometryExecutor : IFrameGeometryExecutor
    {
        private const double MillimetersToMeters = 0.001d;

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

            IModelDoc2 part = _swApp.ActiveDoc as IModelDoc2;
            if (part == null)
            {
                throw new InvalidOperationException("No active SolidWorks document is available.");
            }

            if (!SelectRightPlane(part))
            {
                throw new InvalidOperationException("Right Plane could not be selected.");
            }

            part.SketchManager.InsertSketch(true);
            part.ClearSelection2(true);

            double tableWidthMillimeters = (double)request.TableWidth;
            double tableHeightMillimeters = (double)request.TableHeight;
            if (tableWidthMillimeters <= 0.0d || tableHeightMillimeters <= 0.0d)
            {
                throw new InvalidOperationException("Table width and height must be greater than zero.");
            }

            double halfWidth = ToMeters(tableWidthMillimeters / 2.0d);
            double halfHeight = ToMeters(tableHeightMillimeters / 2.0d);

            double leftX = -halfWidth;
            double rightX = halfWidth;
            double topY = halfHeight;
            double bottomY = -halfHeight;
            SketchSegment topSegment = part.SketchManager.CreateLine(leftX, topY, 0.0d, rightX, topY, 0.0d) as SketchSegment;
            SketchSegment diagonalSegment = part.SketchManager.CreateLine(rightX, topY, 0.0d, leftX, bottomY, 0.0d) as SketchSegment;
            SketchSegment bottomSegment = part.SketchManager.CreateLine(leftX, bottomY, 0.0d, rightX, bottomY, 0.0d) as SketchSegment;

            if (topSegment == null || diagonalSegment == null || bottomSegment == null)
            {
                throw new InvalidOperationException("Failed to create primary Z-frame sketch segments.");
            }

            double yAtOneThird = topY + (bottomY - topY) / 3.0d;
            double yAtTwoThird = topY + 2.0d * (bottomY - topY) / 3.0d;
            double diagonalDx = leftX - rightX;
            double diagonalDy = bottomY - topY;

            double firstSplitX = rightX + (diagonalDx / diagonalDy) * (yAtOneThird - topY);
            double secondSplitX = rightX + (diagonalDx / diagonalDy) * (yAtTwoThird - topY);

            SketchSegment braceTop = part.SketchManager.CreateLine(leftX, topY, 0.0d, firstSplitX, yAtOneThird, 0.0d) as SketchSegment;
            SketchSegment braceBottom = part.SketchManager.CreateLine(rightX, bottomY, 0.0d, secondSplitX, yAtTwoThird, 0.0d) as SketchSegment;

            if (braceTop == null || braceBottom == null)
            {
                throw new InvalidOperationException("Failed to create brace segments to split the Z center line.");
            }

            part.ClearSelection2(true);
            part.SketchManager.InsertSketch(true);
            ApplyStructuralMemberProfile(part, request, topSegment, diagonalSegment, bottomSegment, braceTop, braceBottom);
            part.ForceRebuild3(false);

            string note =
                "Z-frame sketch generated on Right Plane: width=" + tableWidthMillimeters.ToString("0.###") + "mm, height=" + tableHeightMillimeters.ToString("0.###") + "mm, centered at origin; " +
                "braces connect end points to diagonal one-third split points; profile context=" + request.SelectedProfileCode + ".";
            return new FrameGeometryResult(true, part.GetTitle(), note);
        }

        private static bool SelectRightPlane(IModelDoc2 part)
        {
            part.ClearSelection2(true);
            return part.Extension.SelectByID2("Right Plane", "PLANE", 0.0d, 0.0d, 0.0d, false, 0, null, 0);
        }

        private static double ToMeters(double millimeters)
        {
            return millimeters * MillimetersToMeters;
        }

        private static void ApplyStructuralMemberProfile(
            IModelDoc2 part,
            FrameGeometryRequest request,
            SketchSegment topSegment,
            SketchSegment diagonalSegment,
            SketchSegment bottomSegment,
            SketchSegment braceTop,
            SketchSegment braceBottom)
        {
            if (part == null)
            {
                throw new ArgumentNullException(nameof(part));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.ProfileLibraryPath) ||
                string.IsNullOrWhiteSpace(request.SelectedProfileStandard) ||
                string.IsNullOrWhiteSpace(request.SelectedProfileType) ||
                string.IsNullOrWhiteSpace(request.SelectedProfileSize))
            {
                throw new InvalidOperationException("Profile settings are incomplete. Library path, standard, type, and size are required.");
            }

            string profilePath = Path.Combine(
                request.ProfileLibraryPath,
                request.SelectedProfileStandard,
                request.SelectedProfileType,
                request.SelectedProfileSize + ".sldlfp");

            if (!File.Exists(profilePath))
            {
                throw new InvalidOperationException("Selected weldment profile file was not found: " + profilePath + ".");
            }

            IFeatureManager featureManager = part.FeatureManager;
            if (featureManager == null)
            {
                throw new InvalidOperationException("SolidWorks FeatureManager is not available.");
            }

            featureManager.InsertWeldmentFeature();

            StructuralMemberGroup groupOne = CreateStructuralMemberGroup(
                part,
                featureManager,
                topSegment,
                diagonalSegment,
                bottomSegment);
            StructuralMemberGroup groupTwo = CreateStructuralMemberGroup(
                part,
                featureManager,
                braceTop,
                braceBottom);

            DispatchWrapper[] groupedMembers = new DispatchWrapper[]
            {
                new DispatchWrapper(groupOne),
                new DispatchWrapper(groupTwo)
            };

            Feature structuralMemberFeature = featureManager.InsertStructuralWeldment4(
                profilePath,
                1,
                true,
                groupedMembers);

            if (structuralMemberFeature == null)
            {
                throw new InvalidOperationException("Structural member creation failed for profile path: " + profilePath + ".");
            }

            part.ClearSelection2(true);
        }

        private static StructuralMemberGroup CreateStructuralMemberGroup(
            IModelDoc2 part,
            IFeatureManager featureManager,
            params SketchSegment[] segments)
        {
            if (segments == null || segments.Length == 0)
            {
                throw new InvalidOperationException("No segments were provided for structural member group generation.");
            }

            part.ClearSelection2(true);
            int selectedCount = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == null)
                {
                    continue;
                }

                bool selected = segments[i].Select4(true, null);
                if (!selected)
                {
                    throw new InvalidOperationException("Failed to select a sketch segment for structural member generation.");
                }

                selectedCount++;
            }

            if (selectedCount == 0)
            {
                throw new InvalidOperationException("No valid sketch segments were selected for structural member generation.");
            }

            ISelectionMgr selectionManager = part.SelectionManager as ISelectionMgr;
            if (selectionManager == null)
            {
                throw new InvalidOperationException("SolidWorks SelectionManager is not available.");
            }

            object[] selectedSegments = new object[selectedCount];
            for (int selectionIndex = 0; selectionIndex < selectedCount; selectionIndex++)
            {
                object selectedObject = selectionManager.GetSelectedObject6(selectionIndex + 1, 0);
                if (selectedObject == null)
                {
                    throw new InvalidOperationException("Failed to retrieve selected sketch segment for structural member generation.");
                }

                selectedSegments[selectionIndex] = selectedObject;
            }

            StructuralMemberGroup group = featureManager.CreateStructuralMemberGroup();
            if (group == null)
            {
                throw new InvalidOperationException("Failed to create structural member group.");
            }

            group.Segments = selectedSegments;
            group.ApplyCornerTreatment = true;
            group.CornerTreatmentType = 1;
            group.GapWithinGroup = 0.0d;
            group.GapForOtherGroups = 0.0d;
            group.Angle = 0.0d;
            return group;
        }
    }
}
