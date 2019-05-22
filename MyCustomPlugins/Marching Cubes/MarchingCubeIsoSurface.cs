using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyCustomPlugins.Marching_Cubes {
    public class MarchingCubeIsoSurface : GH_Component {

        MarchingVertices marchingVertices;
        List<Brep> surfsToExtract;

        /// <summary>
        /// Initializes a new instance of the MarchingCubeIsoSurface class.
        /// </summary>
        public MarchingCubeIsoSurface()
          : base("Marching Cube IsoSurface", "MC IsoSurface",
              "Iso Surface made from the marching cube algorithm",
              "My Components", "Mesh") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddGenericParameter("Marching Points", "MC Points", "Points used for mesh generation", GH_ParamAccess.item);
            pManager.AddBrepParameter("Surface Inside Area", "Surf", "Surface to extract mesh from", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGeometryParameter("Marching Cube Mesh", "Mesh", "Mesh generated from marching cube algorithm", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {

            surfsToExtract = new List<Brep>();

            DA.GetData(0, ref marchingVertices);
            DA.GetDataList(1, surfsToExtract);

            Brep[] booleanSurfs = Brep.CreateBooleanUnion(surfsToExtract, 0.01);

            marchingVertices.ResetPoints();
            marchingVertices.ExtractSurfs(booleanSurfs);

            DA.SetDataList(0, marchingVertices.GeoGetSurfPoints());
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
            get { return new Guid("3c60a201-71b4-4625-8a95-f47d54103f64"); }
        }
    }
}