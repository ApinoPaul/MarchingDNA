using System;
using System.Text;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;

namespace MyCustomPlugins.FinalPlugin {
    public class IsoSurfaceGenerator : GH_Component {
        /// <summary>
        /// Initializes a new instance of the IsoSurfaceGenerator class.
        /// </summary>
        public IsoSurfaceGenerator()
          : base("IsoSurfaceGenerator", "IsoGen",
              "Creates IsoSurface from personal information",
              "FinalBuild", "FinalBuild") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            //pManager.AddNumberParameter("Resolution", "Resolution", "Quality of the IsoSurface", GH_ParamAccess.item);
            //pManager.AddNumberParameter("BoxArea", "Area", "Maximum size of the IsoSurface working sqaure area", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGeometryParameter("IsoSurface", "IsoSurf", "IsoSurface made from the input parameters", GH_ParamAccess.item);
            pManager.AddGeometryParameter("IsoSurface", "IsoSurf", "IsoSurface made from the input parameters", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            double resolutionTemp = 150;
            double area = 50.0;

            //DA.GetData(0, ref resolutionTemp);
            //DA.GetData(1, ref area);

            int resolution = (int)resolutionTemp;

            MarchingArea myArea = new MarchingArea(resolution, area);

            //myArea.Vertices[0, 0, 0] = -10;
            //myArea.Vertices[1, 0, 0] = -30;
            //myArea.Vertices[0, 1, 0] = 10;
            //myArea.Vertices[1, 1, 0] = -30;
            //myArea.Vertices[0, 0, 1] = -30;
            //myArea.Vertices[1, 0, 1] = -30;
            //myArea.Vertices[0, 1, 1] = -30;
            //myArea.Vertices[1, 1, 1] = -30;

            //Plane pl = new Plane(new Point3d(0, 0, -10), new Point3d(area, 0, -30), new Point3d(0, area, 60));
            //double cellSize = myArea.CellSize;

            //List<Point3d> points = new List<Point3d>();

            //StringBuilder sb = new StringBuilder();

            //for (int z = 0; z < resolution + 1; z++) {
            //    for (int y = 0; y < resolution + 1; y++) {
            //        for (int x = 0; x < resolution + 1; x++) {
            //            Point3d currentVert = new Point3d(x * cellSize, y * cellSize, z * cellSize);
            //            points.Add(currentVert);
            //            double dist = pl.DistanceTo(currentVert);
            //            myArea.Vertices[x, y, z] = Math.Round(dist, 8);
            //            sb.Append(myArea.Vertices[x, y, z] + ", ");
            //        }
            //    }
            //}
            //DA.SetDataList(1, points);

            //System.IO.File.WriteAllText(@"E:\UserFiles\Documents\University\4th_Year_Engineer\DigiFab\Array.txt", sb.ToString());

            double radius = 15;
            Point3d centre = new Point3d(25, 25, 25);

            double cellSize = myArea.CellSize;

            for (int z = 0; z < resolution + 1; z++) {
                for (int y = 0; y < resolution + 1; y++) {
                    for (int x = 0; x < resolution + 1; x++) {
                        Point3d currentVert = new Point3d(x * cellSize, y * cellSize, z * cellSize);
                        double dist = currentVert.DistanceTo(centre);
                        myArea.Vertices[x, y, z] = Math.Round(radius - dist, 8);
                    }
                }
            }

            //List<Point3d> stemPoints = new List<Point3d> {
            //    new Point3d(48, 48, 41),
            //    new Point3d(41, 41, 39),
            //    new Point3d(25, 25, 31),
            //    new Point3d(13, 13, 17),
            //    new Point3d(3, 3, 0)
            //};

            //Stem sampleStem = new Stem(stemPoints, 2.0, 0.3);

            //double cellSize = myArea.CellSize;

            //StringBuilder sb = new StringBuilder();

            //for (int z = 0; z < resolution + 1; z++) {
            //    for (int y = 0; y < resolution + 1; y++) {
            //        sb.Append("(");
            //        for (int x = 0; x < resolution + 1; x++) {
            //            Point3d currentPoint = new Point3d(x * cellSize, y * cellSize, z * cellSize);
            //            bool inside = sampleStem.InShape(currentPoint, out double closestDist);
            //            myArea.Vertices[x, y, z] = closestDist;
            //        }
            //        sb.Append(")\n");
            //    }
            //}

            Mesh generatedMesh = myArea.GetMesh();

            DA.SetData(0, generatedMesh);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon {
            get {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("9b8e651b-c12e-45df-8861-8df1f7146b7c"); }
        }
    }
}