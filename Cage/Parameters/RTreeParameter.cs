using Cage.Types;
using Grasshopper.Kernel;
using System;

namespace Cage.Parameters
{
    class RTreeParameter : GH_Param<GH_RTree>
    {
        public RTreeParameter() : base(new GH_InstanceDescription("cage RTree", "RTree", "", "Cage", "RTree"))
        {
        }

        public override Guid ComponentGuid => new Guid("{E5852C6E-9A4C-4BA8-BCAF-DD66CFD4EBC7}");
    }
}