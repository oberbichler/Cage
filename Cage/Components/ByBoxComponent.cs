using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using Cage.Parameters;

namespace Cage.Components
{
    public class ByBoxComponent : GH_Component
    {
        public ByBoxComponent() : base("cage Find by Box", "ByBox", "", "Cage", "RTree")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new RTreeParameter(), "RTree", "R", "", GH_ParamAccess.item);
            pManager.AddBoxParameter("Box", "B", "", GH_ParamAccess.item);
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
            var box = default(Box);

            if (!DA.GetData(0, ref rtree)) return;
            if (!DA.GetData(1, ref box)) return;

            // --- compute

            var bbox = box.BoundingBox;

            var indices = rtree.FindByBox(bbox.Min, bbox.Max);

            // --- output

            DA.SetDataList(0, indices.Select(o => rtree.Geometries[o]));
            DA.SetDataList(1, indices);
        }

        protected override Bitmap Icon => Properties.Resources.cage_bybox;

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public override Guid ComponentGuid => new Guid("{C5C23E9C-905B-4CA7-A915-87CA5B334512}");
    }
}
