using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Geometry;

namespace MyCustomPlugins.FinalPlugin {
    public class Stem : Shape {

        public readonly Curve StemBone;
        private readonly double MaxRadius;
        private readonly double MinRadius;
        private readonly double StartToEnd;
        private readonly double MaxCurveParam;

        public Stem (List<Point3d> controlPoints, double maxRadius, double minRadius) {
            StemBone = Curve.CreateControlPointCurve(controlPoints, 3);
            MaxRadius = maxRadius;
            MinRadius = minRadius;
            StartToEnd = StemBone.PointAtStart.DistanceTo(StemBone.PointAtEnd);
            MaxCurveParam = controlPoints.Count - 1;
        }

        public bool InShape(Point3d aPoint, out double nearestDist) {
            StemBone.ClosestPoint(aPoint, out double curveParam);
            Point3d pointOnStem = StemBone.PointAt(curveParam);

            double distClosest = aPoint.DistanceTo(pointOnStem);

            double pointRadius = MinRadius + ((MaxRadius - MinRadius) / MaxCurveParam * curveParam);

            if (distClosest < pointRadius) {
                nearestDist = 0;
                return true;
            } else {
                nearestDist = distClosest - pointRadius;
                return false;
            }
        }
    }
}
