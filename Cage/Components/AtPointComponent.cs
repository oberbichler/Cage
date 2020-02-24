using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using Cage.Parameters;

namespace Cage.Components
{
    public class AtPointComponent : GH_Component
    {
        public AtPointComponent() : base("cage At Point", "AtPoint", "", "Cage", "RTree")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RTreeParameter(), "RTree", "R", "", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "P", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance", "T", "", GH_ParamAccess.item, 1e-4);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometries", "G", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Indices", "i", "", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- input

            var rtree = default(RTree);
            var point = default(Point3d);
            var tolerance = default(double);

            if (!DA.GetData(0, ref rtree)) return;
            if (!DA.GetData(1, ref point)) return;
            if (!DA.GetData(2, ref tolerance)) return;

            // --- compute

            var delta = new Vector3d(tolerance, tolerance, tolerance);

            var bbox = new BoundingBox(point - delta, point + delta);

            var indices = rtree.WithinBox(bbox.Min, bbox.Max);

            // --- output

            DA.SetDataList(0, indices.Select(o => rtree.Geometries[o]));
            DA.SetDataList(1, indices);
        }

        protected override Bitmap Icon => null;

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public override Guid ComponentGuid => new Guid("{8A809D9C-AFA5-437A-B4DD-5A7917B3F3FD}");
    }
}
