using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace MyCustomPlugins.RhinoTesting {
    public class TestComponent : GH_Component {
        /// <summary>
        /// Initializes a new instance of the TestComponent class.
        /// </summary>
        public TestComponent()
          : base("TestComponent", "Nickname",
              "Description",
              "Category", "Subcategory") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddNumberParameter("Number", "Number", "asdasdasd", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGeometryParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            double zPoint = 0;

            DA.GetData(0, ref zPoint);

            List<Point3d> stemPoints = new List<Point3d> {
                new Point3d(48, 48, 41),
                new Point3d(41, 41, 39),
                new Point3d(32, 32, 32),
                new Point3d(25, 25, 31),
                new Point3d(18, 18, 20),
                new Point3d(13, 13, 17),
                new Point3d(3, 3, 0)
            };

            Point3d samplePoint = new Point3d(zPoint, zPoint, zPoint);

            FinalPlugin.Stem stem = new FinalPlugin.Stem(stemPoints, 3, 0.5);

            stem.StemBone.ClosestPoint(samplePoint, out double param);

            Point3d closestPoint = stem.StemBone.PointAt(param);

            RhinoApp.WriteLine(param + "");

            //List<Point3d> triPoints1 = new List<Point3d> {
            //    new Point3d(0, 0, 0),
            //    new Point3d(0, 1, 1),
            //    new Point3d(1, 1, 1),
            //    new Point3d(0, 0, 0)
            //};

            //List<Point3d> triPoints2 = new List<Point3d> {
            //    new Point3d(0, 0, 0),
            //    new Point3d(0, 1, 0),
            //    new Point3d(0, 1, 1),
            //    new Point3d(0, 0, 0)
            //};

            //List<Point3d> triPoints3 = new List<Point3d> {
            //    new Point3d(0, 1, 0),
            //    new Point3d(1, 1, 1),
            //    new Point3d(0, 1, 1),
            //    new Point3d(0, 1, 0)
            //};

            //List<Point3d> triPoints4 = new List<Point3d> {
            //    new Point3d(0, 0, 0),
            //    new Point3d(0, 1, 0),
            //    new Point3d(1, 1, 1),
            //    new Point3d(0, 0, 0)
            //};

            //List<Point3d> triPoints5 = new List<Point3d> {
            //    new Point3d(2, 2, 2),
            //    new Point3d(2, 3, 2),
            //    new Point3d(3, 3, 3),
            //    new Point3d(2, 2, 2)
            //};

            //List<Mesh> meshes = new List<Mesh> {
            //    Mesh.CreateFromClosedPolyline(new Polyline(triPoints1)),
            //    Mesh.CreateFromClosedPolyline(new Polyline(triPoints2)),
            //    Mesh.CreateFromClosedPolyline(new Polyline(triPoints3)),
            //    Mesh.CreateFromClosedPolyline(new Polyline(triPoints4)),
            //    Mesh.CreateFromClosedPolyline(new Polyline(triPoints5))
            //};

            //Mesh closedMaybe = new Mesh();
            //closedMaybe.Append(meshes[0]);
            //closedMaybe.Append(meshes[1]);
            //closedMaybe.Append(meshes[2]);
            //closedMaybe.Append(meshes[3]);
            //closedMaybe.Append(meshes[4]);

            //closedMaybe.FillHoles();

            DA.SetData(0, closestPoint);
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
            get { return new Guid("3ef1438e-98f7-4ab0-96ec-faad7796f255"); }
        }
    }
}