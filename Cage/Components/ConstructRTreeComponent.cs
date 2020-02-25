using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Cage.Parameters;

namespace Cage.Components
{
    public class ConstructRTreeComponent : GH_Component
    {
        public ConstructRTreeComponent() : base("cage Construct RTree", "RTree", "", "Cage", "RTree")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometries", "G", "", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new RTreeParameter(), "RTree", "R", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // --- input

            var geometries = new List<GeometryBase>();

            if (!DA.GetDataList(0, geometries)) return;

            // --- execute

            var rtree = new RTree(3, geometries.Count);

            foreach (var geometry in geometries)
            {
                var bbox = geometry.GetBoundingBox(true);
                rtree.Add(bbox.Min, bbox.Max);
            }

            rtree.Finish();
            rtree.Geometries = geometries; // FIXME: Cleaner solution

            // --- output

            DA.SetData(0, rtree);
        }

        protected override Bitmap Icon => Properties.Resources.cage_cage;

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public override Guid ComponentGuid => new Guid("{6718EB0A-5DB5-4369-AF3A-57332807029D}");
    }
}
