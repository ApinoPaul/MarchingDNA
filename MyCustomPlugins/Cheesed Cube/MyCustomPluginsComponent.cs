using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace MyCustomPlugins {
    public class MyCustomPluginsComponent : GH_Component {
        private List<MySphere> mySpheres;
        private Random rand;
        private double minRadius, maxRadius;
        private double minGrowth, maxGrowth;
        private double area;
        private double maxOverLap;
        private int MaxSpawnFailures;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MyCustomPluginsComponent()
          : base("Growing Spheres", "GSpheres",
              "Creates spheres randomly and grow them",
              "My Components", "Blocks") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            //pManager.AddNumberParameter("Sphere Count", "Count", "Amount of Spheres", GH_ParamAccess.item);
            pManager.AddNumberParameter("Working Length", "Area", "Spawning area of spheres", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Sphere Radius", "MaxRad", "Maximum radius of spheres", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Sphere Radius", "MinRad", "Minimum radius of spheres", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Sphere Growth", "MaxGrowth", "Maximum growth rate of spheres", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Sphere Growth", "MinGrowth", "Minimum growth rate of spheres", GH_ParamAccess.item);
            pManager.AddNumberParameter("Sphere Overlap", "Overlap", "Maximum overlap distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("Sphere Spawn Aggresiveness", "SpawnStregnth", "Aggresiveness in terms of number failures to spawn spheres", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGeometryParameter("Shape", "Shape", "Result shape", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            // Defaults
            mySpheres = new List<MySphere>();
            rand = new Random();

            area = 50;
            maxRadius = 10;
            minRadius = 5;
            maxGrowth = 0.3;
            minGrowth = 0.05;
            maxOverLap = -0.5;
            MaxSpawnFailures = 100;

            // Loop Stuff
            bool isFinished = false;
            int spawnFailCount = 0;

            // Plugin Arguments
            DA.GetData(0, ref area);
            DA.GetData(1, ref maxRadius);
            DA.GetData(2, ref minRadius);
            DA.GetData(3, ref maxGrowth);
            DA.GetData(4, ref minGrowth);
            DA.GetData(5, ref maxOverLap);
            DA.GetData(6, ref MaxSpawnFailures);

            while (!isFinished) {
                if (SpawnSphere()) spawnFailCount = 0;
                else spawnFailCount++;

                // Controls the growth of the spheres, stops the sphere it overlaps with another
                for (int i = 0; i < mySpheres.Count; i++) {
                    for (int j = i + 1; j < mySpheres.Count; j++) {
                        if ((mySpheres[i].CentrePoint.DistanceTo(mySpheres[j].CentrePoint)) < (mySpheres[i].Radius + mySpheres[j].Radius - maxOverLap)) {
                            mySpheres[i].StopGrow();
                            mySpheres[j].StopGrow();
                        }
                    }
                    if (mySpheres[i].CanGrow & mySpheres[i].Radius >= maxRadius) mySpheres[i].StopGrow();
                }

                bool finishGrowing = true;

                foreach (MySphere aSphere in mySpheres) {
                    aSphere.Grow();
                    if (aSphere.CanGrow) finishGrowing = false;
                }

                if (finishGrowing && spawnFailCount > MaxSpawnFailures) isFinished = true;
            }

            List<Brep> spheres = new List<Brep>();
            List<Brep> aCube = new List<Brep>();

            aCube.Add(Brep.CreateFromBox(new BoundingBox(new Point3d(minRadius, minRadius, minRadius), new Point3d(area + (2 * minRadius), area + (2 * minRadius), area + (2 * minRadius)))));

            for (int i = 0; i < mySpheres.Count; i++) {
                spheres.Add(mySpheres[i].RhinoSphere());
            }

            Brep[] cheesed = Brep.CreateBooleanDifference(aCube, spheres, 0.001);

            Rhino.RhinoApp.WriteLine(mySpheres.Count.ToString());

            DA.SetDataList(0, cheesed);
        }

        // Spawns a sphere of a random size randomly inside the specified box
        private bool SpawnSphere() {
            double spawnArea = area + minRadius + minRadius;
            Point3d newPoint = new Point3d(rand.NextDouble() * spawnArea, rand.NextDouble() * spawnArea, rand.NextDouble() * spawnArea);
            double newRadius = minRadius;
            double newGrowthRate = (rand.NextDouble() * (maxGrowth - minGrowth)) + minGrowth;
            bool canSpawn = true;

            // Looks if the new sphere overlaps with other spheres
            for (int i = 0; i < mySpheres.Count; i++) {
                if (mySpheres[i].CentrePoint.DistanceTo(newPoint) < mySpheres[i].Radius + newRadius - maxOverLap) {
                    canSpawn = false;
                    break;
                }
            }

            if (canSpawn) {
                mySpheres.Add(new MySphere(newPoint, newRadius, newGrowthRate));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon {
            get {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("6db2a33f-40db-44e1-b4d0-1a4ce611427c"); }
        }
    }
}
