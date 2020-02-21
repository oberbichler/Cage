using Rhino.Geometry;

namespace Cage
{
    public interface ICheck
    {
        int Dimension { get; }

        bool Check(Point3d box_min, Point3d box_max);
    }
}
