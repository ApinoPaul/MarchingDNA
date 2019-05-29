using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.Geometry;

namespace MyCustomPlugins.FinalPlugin {
    public class SphereDNA {

        public double CentreSphereDist { get; }
        public string Name { get; }

        private readonly MarchingArea marchingArea;
        private readonly string bitString;
        private readonly int hashDNA;
        private readonly int[] bitDNA;

        private double sphereRadius;
        private Point3d centrePoint;

        public SphereDNA(string name, double area, MarchingArea marchingArea) {
            Name = name;

            this.marchingArea = marchingArea;

            hashDNA = name.GetHashCode();

            bitString = Convert.ToString(hashDNA, 2).PadLeft(32, '0');
            bitDNA = bitString.Select(c => int.Parse(c.ToString())).ToArray();

            CentreSphereDist = area / 2;

            ExtractSphereVal();
        }

        public void GetVertexValues() {
            ExtractSphereMesh();
            ExtractBorderShape();
            ExtractFaceShape();
        }

        private void ExtractSphereVal() {
            int extractedInt = GetIntValueOf(0);

            sphereRadius = (int)Math.Round(12 + ((17 - 12) / 256.0 * extractedInt));
            centrePoint = new Point3d(CentreSphereDist, CentreSphereDist, CentreSphereDist);
        }

        private void ExtractSphereMesh() {
            for (int z = 0; z < marchingArea.VertSizeZ; z++) {
                for (int y = 0; y < marchingArea.VertSizeY; y++) {
                    for (int x = 0; x < marchingArea.VertSizeX; x++) {
                        Point3d currentVert = new Point3d(x * marchingArea.CellSize, y * marchingArea.CellSize, z * marchingArea.CellSize);
                        double dist = currentVert.DistanceTo(centrePoint);
                        marchingArea.Vertices[x, y, z] = Math.Round(sphereRadius - dist, 8);
                    }
                }
            }
        }

        public void ExtractBorderShape() {
            int extractedInt = GetIntValueOf(1);

            int shapeSides = extractedInt % 7 + 5;
            double shapeRadius = CentreSphereDist;
            Vector3d polygonTransform = new Vector3d(0, 0, shapeRadius);
            Vector3d rotationAxis = new Vector3d(0, 1, 0);

            Point3d[] polygon = new Point3d[shapeSides + 1];
            Point3d shapeCentre = Point3d.Add(centrePoint, new Point3d(0, -1, 0));

            for (int i = 0; i < polygon.Length; i++) {
                polygon[i] = Point3d.Add(shapeCentre, polygonTransform);
                polygonTransform.Rotate(2 * Math.PI / shapeSides, rotationAxis);
            }

            Curve shapeCurve = Curve.CreateControlPointCurve(polygon, 1);

            int vertRadius = (int)(shapeRadius / marchingArea.CellSize);
            PointInt minVert = new PointInt(
                0,
                marchingArea.VertSizeY / 2 - (int)(1 / marchingArea.CellSize),
                0
            );
            PointInt maxVert = new PointInt(
                marchingArea.VertSizeX,
                marchingArea.VertSizeY / 2 + (int)(1 / marchingArea.CellSize),
                marchingArea.VertSizeZ
            );

            Vector3d shapeTranslation = new Vector3d(0, marchingArea.CellSize, 0);

            GetShapeVertices(minVert, maxVert, shapeCurve, shapeCentre, shapeTranslation);
        }

        public void ExtractFaceShape() {
            int extractedInt = GetIntValueOf(2);
            int shapeSides = extractedInt % 3 + 3;
            double shapeRadius = sphereRadius - 4;
            Vector3d polygonTransform = new Vector3d(0, 0, shapeRadius);
            Vector3d rotationAxis = new Vector3d(0, 1, 0);

            Point3d[] polygon1 = new Point3d[shapeSides + 1];
            Point3d[] polygon2 = new Point3d[shapeSides + 1];
            Point3d shapeCentre1 = centrePoint;
            Point3d shapeCentre2 = Point3d.Add(centrePoint, new Point3d(0, -1, 0));

            for (int i = 0; i < polygon1.Length; i++) {
                polygon1[i] = Point3d.Add(shapeCentre1, polygonTransform);
                polygonTransform.Rotate(2 * Math.PI / shapeSides, rotationAxis);
            }
            for (int i = 0; i < polygon2.Length; i++) {
                polygon2[i] = Point3d.Add(shapeCentre2, polygonTransform);
                polygonTransform.Rotate(2 * Math.PI / shapeSides, rotationAxis);
            }

            Curve shapeCurve1 = Curve.CreateControlPointCurve(polygon1, 1);
            Curve shapeCurve2 = Curve.CreateControlPointCurve(polygon2, 1);

            Vector3d shape1Translate = new Vector3d(0, -8, 0);
            Vector3d shape2Translate = new Vector3d(0, 8, 0);

            shapeCurve1.Translate(shape1Translate);
            shapeCentre1 = Point3d.Add(shapeCentre1, shape1Translate);
            shapeCurve2.Translate(shape2Translate);
            shapeCentre2 = Point3d.Add(shapeCentre2, shape2Translate);

            Vector3d shape1VecTrans = new Vector3d(0, marchingArea.CellSize, 0);
            Vector3d shape2VecTrans = new Vector3d(0, marchingArea.CellSize, 0);

            int centreDist = marchingArea.VertSizeY / 2;
            int shapeDist = (int)(8 / marchingArea.CellSize);

            PointInt minVert = new PointInt(
                0,
                centreDist - 2 * shapeDist,
                0
            );
            PointInt maxVert = new PointInt(
                marchingArea.VertSizeX,
                centreDist - shapeDist,
                marchingArea.VertSizeZ
            );

            for (int y = minVert.Y; y < maxVert.Y + 1; y++) {
                for (int z = minVert.Z; z < maxVert.Z; z++) {
                    for (int x = minVert.X; x < maxVert.X; x++) {
                        Point3d currentVert = new Point3d(x * marchingArea.CellSize, y * marchingArea.CellSize, z * marchingArea.CellSize);
                        shapeCurve1.ClosestPoint(currentVert, out double curveParam);
                        Point3d closestPoint = shapeCurve1.PointAt(curveParam);
                        double distToCurrent = currentVert.DistanceTo(shapeCentre1);
                        double distToClosest = closestPoint.DistanceTo(shapeCentre1);
                        double dist = currentVert.DistanceTo(closestPoint);
                        marchingArea.Vertices[x, y, z] = dist * -1;
                    }
                }
                shapeCurve1.Translate(shape1VecTrans);
                shapeCentre1 = Point3d.Add(shapeCentre1, shape1VecTrans);
            }

            minVert = new PointInt(
                0,
                centreDist + shapeDist,
                0
            );
            maxVert = new PointInt(
                marchingArea.VertSizeX,
                centreDist + 2 * shapeDist,
                marchingArea.VertSizeZ
            );

            for (int y = minVert.Y - 1; y < maxVert.Y; y++) {
                for (int z = minVert.Z; z < maxVert.Z; z++) {
                    for (int x = minVert.X; x < maxVert.X; x++) {
                        Point3d currentVert = new Point3d(x * marchingArea.CellSize, y * marchingArea.CellSize, z * marchingArea.CellSize);
                        shapeCurve2.ClosestPoint(currentVert, out double curveParam);
                        Point3d closestPoint = shapeCurve2.PointAt(curveParam);
                        double distToCurrent = currentVert.DistanceTo(shapeCentre2);
                        double distToClosest = closestPoint.DistanceTo(shapeCentre2);
                        double dist = currentVert.DistanceTo(closestPoint);
                        marchingArea.Vertices[x, y, z] = dist * -1;
                    }
                }
                shapeCurve2.Translate(shape2VecTrans);
                shapeCentre2 = Point3d.Add(shapeCentre2, shape2VecTrans);
            }
        }

        private void GetShapeVertices(PointInt minVert, PointInt maxVert, Curve shapeCurve, Point3d shapeCentre, Vector3d shapeTranslation) {
            for (int y = minVert.Y; y < maxVert.Y + 1; y++) {
                for (int z = minVert.Z; z < maxVert.Z; z++) {
                    for (int x = minVert.X; x < maxVert.X; x++) {
                        Point3d currentVert = new Point3d(x * marchingArea.CellSize, y * marchingArea.CellSize, z * marchingArea.CellSize);
                        shapeCurve.ClosestPoint(currentVert, out double curveParam);
                        Point3d closestPoint = shapeCurve.PointAt(curveParam);
                        double distToCurrent = currentVert.DistanceTo(shapeCentre);
                        double distToClosest = closestPoint.DistanceTo(shapeCentre);
                        double dist = currentVert.DistanceTo(closestPoint);
                        if (distToClosest > distToCurrent) {
                            marchingArea.Vertices[x, y, z] = (marchingArea.Vertices[x, y, z] > 0) ? marchingArea.Vertices[x, y, z] : dist;
                        } else {
                            dist = -dist;
                            marchingArea.Vertices[x, y, z] = (marchingArea.Vertices[x, y, z] > 0) ? marchingArea.Vertices[x, y, z]
                                : (dist > marchingArea.Vertices[x, y, z]) ? dist : marchingArea.Vertices[x, y, z];
                        }
                    }
                }
                shapeCurve.Translate(shapeTranslation);
                shapeCentre = Point3d.Add(shapeCentre, shapeTranslation);
            }
        }

        private int GetIntValueOf(int bitIndex) {
            int extractedInt = 0;
            int indexRange = (bitIndex + 1) * 8 - 1;
            for (int i = indexRange, pow = 1; i > indexRange - 8; i--) {
                extractedInt += (bitDNA[i] > 0) ? pow : 0;
                pow += pow;
            }
            return extractedInt;
        }

        private struct PointInt {
            public int X { get; }
            public int Y { get; }
            public int Z { get; }

            public PointInt(int x, int y, int z) {
                X = x;
                Y = y;
                Z = z;
            }
        }
    }
}
