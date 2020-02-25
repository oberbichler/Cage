using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Cage
{
    public class Info : GH_AssemblyInfo
    {
        public override string Name => "Cage";

        public override Bitmap Icon => Properties.Resources.cage_cage;

        public override string Description => "A spatial static Hilbert R-Tree for Grasshopper";

        public override Guid Id => new Guid("c8e379c8-53bc-421d-aee5-4abd53cc177e");

        public override string AuthorName => "Thomas Oberbichler";

        public override string AuthorContact => "thomas.oberbichler@gmail.com";
    }
}
