using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace MyCustomPlugins
{
    class MySphere
    {
        private Point3d centrePoint { get; set; }
        private double radius { get; set; }
        private double growthRate { get; set; }
        private bool canGrow { get; set; }

        public MySphere(Point3d centrePoint, double radius, double growthRate)
        {
            this.centrePoint = centrePoint;
            this.radius = radius;
            this.growthRate = growthRate;

            canGrow = true;
        }

        public Point3d CentrePoint
        {
            get { return centrePoint; }
        }

        public double Radius
        {
            get { return radius; }
        }

        public bool CanGrow
        {
            get { return canGrow; }
        }

        public void Grow()
        {
            if(canGrow) radius = radius + growthRate;
        }

        public void OffSetGrow(double size)
        {
            radius = radius + size;
        }

        public void StopGrow()
        {
            canGrow = false;
        }

        public Brep RhinoSphere()
        {
            //return new Sphere(centrePoint, radius);
            return Brep.CreateFromSphere(new Sphere(centrePoint, radius));
        }
    }
}
