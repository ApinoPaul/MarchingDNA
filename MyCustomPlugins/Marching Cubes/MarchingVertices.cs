using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace MyCustomPlugins.Marching_Cubes {
    class MarchingVertices {

        public MarchingPoint[,,] MarchingPoints { get; }
        public double CubeSize { get; }
        public int Resolution { get; }

        public MarchingVertices(double area, int resolution) {

            CubeSize = area / (resolution - 1);
            Resolution = resolution;

            MarchingPoints = new MarchingPoint[Resolution, Resolution, Resolution];

            for (int z = 0; z < Resolution; z++) {
                for (int y = 0; y < Resolution; y++) {
                    for (int x = 0; x < Resolution; x++) {
                        MarchingPoints[x, y, z] =
                            new MarchingPoint(x * CubeSize, y * CubeSize, z * CubeSize);
                    }
                }
            }
        }

        public List<Point3d> GeoGetAllPoint() {
            List<Point3d> points = new List<Point3d>();

            for (int z = 0; z < Resolution; z++) {
                for (int y = 0; y < Resolution; y++) {
                    for (int x = 0; x < Resolution; x++) {
                        MarchingPoint currentPoint = MarchingPoints[x, y, z];
                        points.Add(MarchingPoints[x, y, z].point);
                    }
                }
            }
            return points;
        }

        public List<Point3d> GeoGetSurfPoints() {
            List<Point3d> points = new List<Point3d>();

            for (int z = 0; z < Resolution; z++) {
                for (int y = 0; y < Resolution; y++) {
                    for (int x = 0; x < Resolution; x++) {
                        MarchingPoint currentPoint = MarchingPoints[x, y, z];
                        if (currentPoint.IsOn) {
                            points.Add(MarchingPoints[x, y, z].point);
                        }
                    }
                }
            }
            return points;
        }

        public List<Brep> GeoGetTriangles() {
            List<Brep> triangles = new List<Brep>();

            return triangles;
        }

        public void ResetPoints() {
            for (int z = 0; z < Resolution; z++) {
                for (int y = 0; y < Resolution; y++) {
                    for (int x = 0; x < Resolution; x++) {
                        MarchingPoints[x, y, z].IsOn = false;
                    }
                }
            }
        }

        public void ExtractSurfs(Brep[] surfs) {
            for (int z = 0; z < Resolution; z++) {
                for (int y = 0; y < Resolution; y++) {
                    for (int x = 0; x < Resolution; x++) {
                        for (int i = 0; i < surfs.Length; i++) {
                            if (surfs[i].IsPointInside(MarchingPoints[x, y, z].point, 0.000001, false)) {
                                MarchingPoints[x, y, z].IsOn = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public struct MarchingPoint {

        public bool IsOn { get; set; }
        public Point3d point;

        public MarchingPoint(double x, double y, double z) {
            IsOn = false;
            point = new Point3d(x, y, z);
        }
    }
}
