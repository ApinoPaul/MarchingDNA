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
            //ExtractSphereMesh();
            ExtractBorderShape();
        }

        private void ExtractSphereVal() {
            int extractedInt = getIntValueOf(0);

            sphereRadius = (int)Math.Round(12 + ((17 - 12) / 256.0 * extractedInt));
            centrePoint = new Point3d(CentreSphereDist, CentreSphereDist, CentreSphereDist);
        }

        private void ExtractSphereMesh() {
            int vertRadius = (int)(sphereRadius / marchingArea.CellSize) + 3;
            PointInt minVert = new PointInt(
                marchingArea.VertSizeX / 2 - vertRadius,
                marchingArea.VertSizeY / 2 - vertRadius,
                marchingArea.VertSizeZ / 2 - vertRadius
            );

            PointInt maxVert = new PointInt(
                (minVert.X + vertRadius * 2) > marchingArea.VertSizeX ? marchingArea.VertSizeX : minVert.X + vertRadius * 2,
                (minVert.Y + vertRadius * 2) > marchingArea.VertSizeY ? marchingArea.VertSizeY : minVert.Y + vertRadius * 2,
                (minVert.Z + vertRadius * 2) > marchingArea.VertSizeZ ? marchingArea.VertSizeZ : minVert.Z + vertRadius * 2
            );

            for (int z = minVert.Z; z < maxVert.Z; z++) {
                for (int y = minVert.Y; y < maxVert.Y; y++) {
                    for (int x = minVert.X; x < maxVert.X; x++) {
                        Point3d currentVert = new Point3d(x * marchingArea.CellSize, y * marchingArea.CellSize, z * marchingArea.CellSize);
                        double dist = currentVert.DistanceTo(centrePoint);
                        marchingArea.Vertices[x, y, z] = Math.Round(sphereRadius - dist, 8);
                    }
                }
            }
        }

        public void ExtractBorderShape() {
            int extractedInt = getIntValueOf(1);

            int shapeSides = extractedInt % 7 + 5;
            double shapeRadius = sphereRadius + sphereRadius / 3;
            Vector3d triangleTransform = new Vector3d(0, 0, shapeRadius);
            Vector3d rotationAxis = new Vector3d(0, 1, 0);
            Point3d shapeCentre = centrePoint;

            Point3d[] triangle = new Point3d[shapeSides + 1];

            for (int i = 0; i < triangle.Length; i++) {
                triangle[i] = Point3d.Add(shapeCentre, triangleTransform);
                triangleTransform.Rotate(2 * Math.PI / shapeSides, rotationAxis);
            }

            Curve triangleCurve = Curve.CreateControlPointCurve(triangle, 1);
            //Brep surf = Surface.CreateExtrusion(triangleCurve, new Vector3d(0, 2, 0)).ToBrep();
            //surf = surf.CapPlanarHoles(0.000000001);

            int vertRadius = (int)(shapeRadius / marchingArea.CellSize);
            PointInt minVert = new PointInt(
                marchingArea.VertSizeX / 2 - vertRadius,
                marchingArea.VertSizeY / 2 - (int)(2 / marchingArea.CellSize),
                marchingArea.VertSizeZ / 2 - vertRadius
            );
            PointInt maxVert = new PointInt(
                marchingArea.VertSizeX / 2 + vertRadius,
                marchingArea.VertSizeY / 2 + (int)(2 / marchingArea.CellSize),
                marchingArea.VertSizeZ / 2 + vertRadius
            );

            for (int z = minVert.Z; z < maxVert.Z; z++) {
                for (int y = minVert.Y; y < maxVert.Y; y++) {

                    for (int x = minVert.X; x < maxVert.X; x++) {
                        double curveParam = 0;
                        Point3d currentVert = new Point3d(x * marchingArea.CellSize, y * marchingArea.CellSize, z * marchingArea.CellSize);
                        triangleCurve.ClosestPoint(currentVert, out curveParam);
                        Point3d closestPoint = triangleCurve.PointAt(curveParam);
                        double distToCurrent = currentVert.DistanceTo(shapeCentre);
                        double distToClosest = closestPoint.DistanceTo(shapeCentre);
                        double dist = currentVert.DistanceTo(closestPoint);
                        if (distToClosest > distToCurrent) {
                            if (marchingArea.Vertices[x, y, z] > 0)
                                marchingArea.Vertices[x, y, z] = (dist < marchingArea.Vertices[x, y, z]) ? dist : marchingArea.Vertices[x, y, z];
                            else marchingArea.Vertices[x, y, z] = dist;
                        } else if (!(marchingArea.Vertices[x, y, z] > 0)) {
                            dist = dist * -1;
                            marchingArea.Vertices[x, y, z] = (dist > marchingArea.Vertices[x, y, z]) ? dist : marchingArea.Vertices[x, y, z];
                        }
                    }
                }
            }
        }

        private int getIntValueOf(int bitIndex) {
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
