using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Cage.Components
{
    public class HilbertCurveComponent : GH_Component
    {
        public HilbertCurveComponent() : base("cage Hilbert Curve", "HilbertCrv", "", "Cage", "RTree")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("m", "m", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("i", "i", "", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("x", "x", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("y", "y", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("z", "z", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var m = default(int);
            var i = default(int);

            if (DA.GetData(0, ref m)) return;
            if (DA.GetData(1, ref i)) return;

            var hilbert = new HilbertCurve(2);

            var p = hilbert.PointAt((ulong)i);

            DA.SetData(0, p[0]);
            DA.SetData(1, p[1]);
            DA.SetData(2, p[2]);
        }

        protected override Bitmap Icon => null;

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public override Guid ComponentGuid => new Guid("{6C5AC666-FF6B-4EF6-BB0F-288F8D643C62}");
    }
}
