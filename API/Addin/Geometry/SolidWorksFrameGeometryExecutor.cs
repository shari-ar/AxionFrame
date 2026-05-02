using SolidWorks.Interop.sldworks;
using System;

namespace AxionFrame
{
    public sealed class SolidWorksFrameGeometryExecutor : IFrameGeometryExecutor
    {
        private const double MillimetersToMeters = 0.001d;
        private const double ZWidthMillimeters = 700.0d;
        private const double ZHeightMillimeters = 1000.0d;

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

            double halfWidth = ToMeters(ZWidthMillimeters / 2.0d);
            double halfHeight = ToMeters(ZHeightMillimeters / 2.0d);

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
            part.ForceRebuild3(false);

            string note =
                "Z-frame sketch generated on Right Plane: width=700mm, height=1000mm, centered at origin; " +
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
    }
}
