using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace MyCustomPlugins.FinalPlugin {
    public interface Shape {

        bool InShape(Point3d aPoint, out double nearestDist);
    }
}
