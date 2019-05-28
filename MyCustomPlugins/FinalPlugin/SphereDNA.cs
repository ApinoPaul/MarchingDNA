using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCustomPlugins.FinalPlugin {
    public class SphereDNA {

        public double CentreSphere { get; }
        public string Name { get; }

        public SphereDNA (string name, double centreSphere, double[,,] vertices) {
            Name = name;

            CentreSphere = centreSphere;
        }

        public void GetVertexValues() {

        }
    }
}
