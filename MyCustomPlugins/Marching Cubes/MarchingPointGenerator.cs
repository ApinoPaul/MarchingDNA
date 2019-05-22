using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyCustomPlugins.Marching_Cubes {

    public class MarchingPointGenerator : GH_Component {

        private double area;
        private double resolution;
        private bool show;

        /// <summary>
        /// Initializes a new instance of the MarchingCubes class.
        /// </summary>
        public MarchingPointGenerator()
          : base("Marching Point Generator", "MP Gen",
              "Generates points in the given area with given resolution",
              "My Components", "Mesh") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddNumberParameter("Area", "Area", "Area", GH_ParamAccess.item);
            pManager.AddNumberParameter("Resolution", "Resolution", "Resolution", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Show Grid", "ShowGrid", "Shows grid of points", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("Marching Points", "MarchPoints", "Marching points used in generation", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Geometry Points", "GeoPionts", "Shows the points in the space", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {

            area = 75;
            resolution = 10;
            show = false;

            DA.GetData(0, ref area);
            DA.GetData(1, ref resolution);
            DA.GetData(2, ref show);

            MarchingVertices marchingVertices = new MarchingVertices(area, (int)resolution);

            if (show) {
                DA.SetDataList(1, marchingVertices.GeoGetAllPoint());
            }

            DA.SetData(0, marchingVertices);
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
            get { return new Guid("48344fc6-c221-4219-9f7d-210f2b12e1ba"); }
        }
    }
}