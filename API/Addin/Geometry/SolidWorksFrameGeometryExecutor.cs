using SolidWorks.Interop.sldworks;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Reflection;

namespace AxionFrame
{
    public sealed class SolidWorksFrameGeometryExecutor : IFrameGeometryExecutor
    {
        private const double MillimetersToMeters = 0.001d;
        private const int ConnectedSegmentsSimpleCut = 1;
        private const int ConnectedSegmentsCopedCut = 2;

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
            List<string> traceEvents = new List<string>();
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                traceEvents.Add("generate.start");
                traceEvents.Add("request.profileCode=" + SafeText(request.SelectedProfileCode));
                traceEvents.Add("request.profileLibraryPath=" + SafeText(request.ProfileLibraryPath));
                traceEvents.Add("request.profileStandard=" + SafeText(request.SelectedProfileStandard));
                traceEvents.Add("request.profileType=" + SafeText(request.SelectedProfileType));
                traceEvents.Add("request.profileSize=" + SafeText(request.SelectedProfileSize));

                IModelDoc2 part = _swApp.ActiveDoc as IModelDoc2;
                if (part == null)
                {
                    throw new InvalidOperationException("No active SolidWorks document is available.");
                }

                traceEvents.Add("doc.active=" + part.GetTitle());
                traceEvents.Add("doc.type=" + ((object)part).GetType().FullName);
                if (!SelectRightPlane(part))
                {
                    throw new InvalidOperationException("Right Plane could not be selected.");
                }

                traceEvents.Add("plane.selected=Right Plane");
                part.SketchManager.InsertSketch(true);
                part.ClearSelection2(true);
                traceEvents.Add("sketch.opened=true");

                double tableWidthMillimeters = (double)request.TableWidth;
                double tableHeightMillimeters = (double)request.TableHeight;
                traceEvents.Add(
                    "request.dimensions.widthMm=" + tableWidthMillimeters.ToString("0.###", CultureInfo.InvariantCulture) +
                    ";heightMm=" + tableHeightMillimeters.ToString("0.###", CultureInfo.InvariantCulture));
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
                traceEvents.Add("segments.primary.created=true");
                traceEvents.Add("segment.top=" + DescribeSegment(topSegment));
                traceEvents.Add("segment.diagonal=" + DescribeSegment(diagonalSegment));
                traceEvents.Add("segment.bottom=" + DescribeSegment(bottomSegment));

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
                traceEvents.Add("segments.brace.created=true");
                traceEvents.Add("segment.braceTop=" + DescribeSegment(braceTop));
                traceEvents.Add("segment.braceBottom=" + DescribeSegment(braceBottom));

                part.ClearSelection2(true);
                part.SketchManager.InsertSketch(true);
                traceEvents.Add("sketch.closed=true");

                ApplyStructuralMemberProfile(part, request, topSegment, diagonalSegment, bottomSegment, braceTop, braceBottom, traceEvents);
                part.ForceRebuild3(false);
                traceEvents.Add("rebuild.completed=true");

                string note =
                    "Z-frame sketch generated on Right Plane: width=" + tableWidthMillimeters.ToString("0.###") + "mm, height=" + tableHeightMillimeters.ToString("0.###") + "mm, centered at origin; " +
                    "braces connect end points to diagonal one-third split points; profile context=" + request.SelectedProfileCode + "; trace=" + BuildTrace(traceEvents) + ".";
                return new FrameGeometryResult(true, part.GetTitle(), note);
            }
            catch (Exception ex)
            {
                traceEvents.Add("generate.exception.type=" + ex.GetType().FullName);
                traceEvents.Add("generate.exception.message=" + SafeText(ex.Message));
                throw new InvalidOperationException("Frame geometry execution failed; trace=" + BuildTrace(traceEvents), ex);
            }
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
            SketchSegment braceBottom,
            List<string> traceEvents)
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
            traceEvents.Add("profile.path=" + profilePath);

            if (!File.Exists(profilePath))
            {
                throw new InvalidOperationException("Selected weldment profile file was not found: " + profilePath + ".");
            }
            traceEvents.Add("profile.exists=true");

            IFeatureManager featureManager = part.FeatureManager;
            if (featureManager == null)
            {
                throw new InvalidOperationException("SolidWorks FeatureManager is not available.");
            }
            traceEvents.Add("featureManager.type=" + featureManager.GetType().FullName);

            Feature weldmentFeature = featureManager.InsertWeldmentFeature();
            traceEvents.Add("weldment.feature.inserted=true");
            traceEvents.Add("weldment.feature.null=" + (weldmentFeature == null ? "true" : "false"));
            traceEvents.Add("selection.beforeGroupBuild.count=" + GetSelectionCount(part).ToString(CultureInfo.InvariantCulture));

            StructuralMemberGroup groupOne = CreateStructuralMemberGroup(
                part,
                featureManager,
                "group1",
                traceEvents,
                topSegment,
                diagonalSegment,
                bottomSegment);
            StructuralMemberGroup groupTwo = CreateStructuralMemberGroup(
                part,
                featureManager,
                "group2",
                traceEvents,
                braceTop,
                braceBottom);

            traceEvents.Add("group1.created.null=" + (groupOne == null ? "true" : "false"));
            traceEvents.Add("group2.created.null=" + (groupTwo == null ? "true" : "false"));

            DispatchWrapper[] groupedMembers = new DispatchWrapper[]
            {
                new DispatchWrapper(groupOne),
                new DispatchWrapper(groupTwo)
            };

            Feature structuralMemberFeature = TryInsertStructuralWeldmentWithFallback(
                part,
                featureManager,
                profilePath,
                groupedMembers,
                traceEvents);
            if (structuralMemberFeature == null)
            {
                throw new InvalidOperationException("Structural member creation failed for profile path: " + profilePath + ".");
            }
            traceEvents.Add("weldment.feature.created=true");
            traceEvents.Add("weldment.feature.type=" + structuralMemberFeature.GetType().FullName);

            part.ClearSelection2(true);
            traceEvents.Add("selection.cleared.afterWeldment=true");
        }

        private static StructuralMemberGroup CreateStructuralMemberGroup(
            IModelDoc2 part,
            IFeatureManager featureManager,
            string groupKey,
            List<string> traceEvents,
            params SketchSegment[] segments)
        {
            if (segments == null || segments.Length == 0)
            {
                throw new InvalidOperationException("No segments were provided for structural member group generation.");
            }

            part.ClearSelection2(true);
            traceEvents.Add(groupKey + ".selection.cleared=true");
            int selectedCount = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == null)
                {
                    traceEvents.Add(groupKey + ".segment[" + i.ToString(CultureInfo.InvariantCulture) + "].null=true");
                    continue;
                }

                string selectionToken = TryGetSegmentSelectionToken(segments[i]);
                bool selected = false;
                if (!string.IsNullOrWhiteSpace(selectionToken))
                {
                    selected = part.Extension.SelectByID2(
                        selectionToken,
                        "EXTSKETCHSEGMENT",
                        0.0d,
                        0.0d,
                        0.0d,
                        selectedCount > 0,
                        0,
                        null,
                        0);

                    traceEvents.Add(
                        groupKey + ".segment[" + i.ToString(CultureInfo.InvariantCulture) + "].selectById=" + (selected ? "true" : "false") +
                        ";token=" + selectionToken);
                }

                if (!selected)
                {
                    selected = segments[i].Select4(selectedCount > 0, null);
                    traceEvents.Add(
                        groupKey + ".segment[" + i.ToString(CultureInfo.InvariantCulture) + "].select4Fallback=" + (selected ? "true" : "false"));
                }

                traceEvents.Add(
                    groupKey + ".segment[" + i.ToString(CultureInfo.InvariantCulture) + "].select=" + (selected ? "true" : "false") +
                    ";detail=" + DescribeSegment(segments[i]));
                if (!selected)
                {
                    throw new InvalidOperationException("Failed to select a sketch segment for structural member generation.");
                }

                selectedCount++;
            }
            traceEvents.Add(groupKey + ".selected.segmentCount=" + selectedCount.ToString(CultureInfo.InvariantCulture));
            traceEvents.Add(groupKey + ".selection.countAfterSelect=" + GetSelectionCount(part).ToString(CultureInfo.InvariantCulture));

            if (selectedCount == 0)
            {
                throw new InvalidOperationException("No valid sketch segments were selected for structural member generation.");
            }

            ISelectionMgr selectionManager = part.SelectionManager as ISelectionMgr;
            if (selectionManager == null)
            {
                throw new InvalidOperationException("SolidWorks SelectionManager is not available.");
            }

            SketchSegment[] selectedSegments = new SketchSegment[selectedCount];
            for (int selectionIndex = 0; selectionIndex < selectedCount; selectionIndex++)
            {
                object selectedObject = selectionManager.GetSelectedObject6(selectionIndex + 1, 0);
                if (selectedObject == null)
                {
                    throw new InvalidOperationException("Failed to retrieve selected sketch segment for structural member generation.");
                }

                SketchSegment selectedSegment = selectedObject as SketchSegment;
                if (selectedSegment == null)
                {
                    throw new InvalidOperationException("Selected object is not a sketch segment.");
                }

                selectedSegments[selectionIndex] = selectedSegment;
                traceEvents.Add(
                    groupKey + ".selection[" + selectionIndex.ToString(CultureInfo.InvariantCulture) + "]=" +
                    selectedObject.GetType().FullName +
                    ";describe=" + SafeDescribeObject(selectedObject));
            }

            StructuralMemberGroup group = featureManager.CreateStructuralMemberGroup();
            if (group == null)
            {
                throw new InvalidOperationException("Failed to create structural member group.");
            }
            traceEvents.Add(groupKey + ".group.type=" + group.GetType().FullName);

            group.Segments = selectedSegments;
            group.ApplyCornerTreatment = true;
            group.CornerTreatmentType = 1;
            group.GapWithinGroup = 0.0d;
            group.GapForOtherGroups = 0.0d;
            group.Angle = 0.0d;
            traceEvents.Add(groupKey + ".configured=true;cornerType=1;gapWithin=0;gapOther=0;angle=0");
            return group;
        }

        private static Feature TryInsertStructuralWeldmentWithFallback(
            IModelDoc2 part,
            IFeatureManager featureManager,
            string profilePath,
            DispatchWrapper[] groupedMembers,
            List<string> traceEvents)
        {
            if (groupedMembers == null || groupedMembers.Length == 0)
            {
                return null;
            }

            Feature feature = TryInsertStructuralWeldment(
                featureManager,
                profilePath,
                ConnectedSegmentsCopedCut,
                true,
                groupedMembers,
                traceEvents,
                "insert.bothGroups.coped.allow");
            if (feature != null)
            {
                return feature;
            }

            if (groupedMembers.Length >= 2)
            {
                Feature firstGroupFeature = TryInsertStructuralWeldment(
                    featureManager,
                    profilePath,
                    ConnectedSegmentsCopedCut,
                    true,
                    new DispatchWrapper[] { groupedMembers[0] },
                    traceEvents,
                    "insert.group1.coped.allow");
                if (firstGroupFeature != null)
                {
                    Feature secondGroupFeature = TryInsertStructuralWeldment(
                        featureManager,
                        profilePath,
                        ConnectedSegmentsCopedCut,
                        true,
                        new DispatchWrapper[] { groupedMembers[1] },
                        traceEvents,
                        "insert.group2.coped.allow");
                    if (secondGroupFeature != null)
                    {
                        return secondGroupFeature;
                    }
                }
            }

            part.ClearSelection2(true);
            return TryInsertStructuralWeldment(
                featureManager,
                profilePath,
                ConnectedSegmentsSimpleCut,
                false,
                groupedMembers,
                traceEvents,
                "insert.bothGroups.simple.noProtrusion");
        }

        private static Feature TryInsertStructuralWeldment(
            IFeatureManager featureManager,
            string profilePath,
            int connectedSegmentsOption,
            bool allowProtrusion,
            DispatchWrapper[] groups,
            List<string> traceEvents,
            string attemptLabel)
        {
            if (featureManager == null || groups == null || groups.Length == 0)
            {
                return null;
            }

            try
            {
                object featureObject = featureManager.GetType().InvokeMember(
                    "InsertStructuralWeldment5",
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    featureManager,
                    new object[] { profilePath, connectedSegmentsOption, allowProtrusion, groups, string.Empty });

                Feature insertedFeature = featureObject as Feature;
                traceEvents.Add(
                    attemptLabel + ".method=InsertStructuralWeldment5;connectedOption=" + connectedSegmentsOption.ToString(CultureInfo.InvariantCulture) +
                    ";allowProtrusion=" + (allowProtrusion ? "true" : "false") +
                    ";groupCount=" + groups.Length.ToString(CultureInfo.InvariantCulture) +
                    ";resultNull=" + (insertedFeature == null ? "true" : "false"));

                if (insertedFeature != null)
                {
                    return insertedFeature;
                }
            }
            catch (MissingMethodException ex)
            {
                traceEvents.Add(attemptLabel + ".method=InsertStructuralWeldment5;missing=true;error=" + SafeText(ex.Message));
            }
            catch (TargetInvocationException ex)
            {
                Exception inner = ex.InnerException ?? ex;
                traceEvents.Add(attemptLabel + ".method=InsertStructuralWeldment5;invokeError=" + SafeText(inner.Message));
            }
            catch (Exception ex)
            {
                traceEvents.Add(attemptLabel + ".method=InsertStructuralWeldment5;error=" + SafeText(ex.Message));
            }

            try
            {
                Feature insertedFeature = featureManager.InsertStructuralWeldment4(
                    profilePath,
                    connectedSegmentsOption,
                    allowProtrusion,
                    groups);

                traceEvents.Add(
                    attemptLabel + ".method=InsertStructuralWeldment4;connectedOption=" + connectedSegmentsOption.ToString(CultureInfo.InvariantCulture) +
                    ";allowProtrusion=" + (allowProtrusion ? "true" : "false") +
                    ";groupCount=" + groups.Length.ToString(CultureInfo.InvariantCulture) +
                    ";resultNull=" + (insertedFeature == null ? "true" : "false"));

                return insertedFeature;
            }
            catch (Exception ex)
            {
                traceEvents.Add(attemptLabel + ".method=InsertStructuralWeldment4;error=" + SafeText(ex.Message));
                return null;
            }
        }

        private static string BuildTrace(IList<string> traceEvents)
        {
            if (traceEvents == null || traceEvents.Count == 0)
            {
                return "none";
            }

            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < traceEvents.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(" > ");
                }

                builder.Append(traceEvents[index]);
            }

            return builder.ToString();
        }

        private static int GetSelectionCount(IModelDoc2 part)
        {
            if (part == null)
            {
                return -1;
            }

            try
            {
                ISelectionMgr selectionManager = part.SelectionManager as ISelectionMgr;
                if (selectionManager == null)
                {
                    return -1;
                }

                return selectionManager.GetSelectedObjectCount2(-1);
            }
            catch
            {
                return -1;
            }
        }

        private static string DescribeSegment(SketchSegment segment)
        {
            if (segment == null)
            {
                return "<null>";
            }

            try
            {
                return "segment=" + SafeDescribeObject(segment);
            }
            catch (Exception ex)
            {
                return "segmentDescribeError=" + SafeText(ex.Message);
            }
        }

        private static string SafeDescribeObject(object value)
        {
            if (value == null)
            {
                return "<null>";
            }

            try
            {
                Type valueType = value.GetType();
                PropertyInfo nameProperty = valueType.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                if (nameProperty != null && nameProperty.CanRead)
                {
                    object propertyValue = nameProperty.GetValue(value, null);
                    if (propertyValue != null)
                    {
                        return "name=" + SafeText(propertyValue.ToString()) + ";type=" + valueType.FullName;
                    }
                }

                return "value=" + SafeText(value.ToString()) + ";type=" + valueType.FullName;
            }
            catch (Exception ex)
            {
                return "objectDescribeError=" + SafeText(ex.Message);
            }
        }

        private static string TryGetSegmentSelectionToken(SketchSegment segment)
        {
            if (segment == null)
            {
                return string.Empty;
            }

            string segmentName = TryInvokeStringMethod(segment, "GetName");
            if (string.IsNullOrWhiteSpace(segmentName))
            {
                return string.Empty;
            }

            object sketchObject = TryInvokeObjectMethod(segment, "GetSketch");
            string sketchName = TryInvokeStringMethod(sketchObject, "GetName");
            if (string.IsNullOrWhiteSpace(sketchName))
            {
                sketchName = TryReadStringProperty(sketchObject, "Name");
            }

            if (string.IsNullOrWhiteSpace(sketchName))
            {
                return string.Empty;
            }

            return segmentName + "@" + sketchName;
        }

        private static string TryInvokeStringMethod(object target, string methodName)
        {
            if (target == null || string.IsNullOrWhiteSpace(methodName))
            {
                return string.Empty;
            }

            try
            {
                MethodInfo methodInfo = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if (methodInfo == null)
                {
                    return string.Empty;
                }

                object value = methodInfo.Invoke(target, null);
                return value == null ? string.Empty : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private static object TryInvokeObjectMethod(object target, string methodName)
        {
            if (target == null || string.IsNullOrWhiteSpace(methodName))
            {
                return null;
            }

            try
            {
                MethodInfo methodInfo = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if (methodInfo == null)
                {
                    return null;
                }

                return methodInfo.Invoke(target, null);
            }
            catch
            {
                return null;
            }
        }

        private static string TryReadStringProperty(object target, string propertyName)
        {
            if (target == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return string.Empty;
            }

            try
            {
                PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null || !propertyInfo.CanRead)
                {
                    return string.Empty;
                }

                object value = propertyInfo.GetValue(target, null);
                return value == null ? string.Empty : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string SafeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "<empty>";
            }

            return value.Replace("\r", " ").Replace("\n", " ").Trim();
        }
    }
}
