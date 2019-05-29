using System;
using System.Text;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyCustomPlugins.FinalPlugin {
    public class SimpleDNA : GH_Component {
        /// <summary>
        /// Initializes a new instance of the SimpleDNA class.
        /// </summary>
        public SimpleDNA()
          : base("SimpleDNA", "SimpleDNA",
              "Takes in information and creates Physical representation",
              "FinalBuild", "DNA") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddTextParameter("First Name", "FirstName", "First name", GH_ParamAccess.item);
            pManager.AddTextParameter("Last Name", "LastName", "Last name", GH_ParamAccess.item);
            pManager.AddTextParameter("Date of Birth Day", "DoB Day", "Date of Birth Day", GH_ParamAccess.item);
            pManager.AddTextParameter("Date of Birth Month", "DoB Month", "Date of Birth Month", GH_ParamAccess.item);
            pManager.AddTextParameter("Date of Birth Year", "DoB Year", "Date of Birth Year", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGeometryParameter("IsoSurface", "IsoSurf", "IsoSurface made from the input parameters", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            int resolution = 150;
            double area = 50;

            string nameFirst = "";
            string nameLast = "";
            string day = "";
            string month = "";
            string year = "";

            DA.GetData(0, ref nameFirst);
            DA.GetData(1, ref nameLast);
            DA.GetData(2, ref day);
            DA.GetData(3, ref month);
            DA.GetData(4, ref year);

            StringBuilder sb = new StringBuilder();

            sb.Append(nameFirst.ToLower());
            sb.Append(" " + nameLast.ToLower());
            sb.Append(" (" + day);
            sb.Append("/" + month);
            sb.Append("/" + year + ")");

            MarchingArea myArea = new MarchingArea(resolution, area, area, area);
            SphereDNA myDNA = new SphereDNA(sb.ToString(), area, myArea);

            myDNA.GetVertexValues();

            Mesh genMesh = myArea.GetMesh();

            DA.SetData(0, genMesh);
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
            get { return new Guid("8659bfa4-5760-4b40-a7a6-12cfaa26f65f"); }
        }
    }
}