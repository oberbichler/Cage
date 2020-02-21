using Rhino.Geometry;

namespace Cage
{
    public class WithinBox : ICheck
    {
        public WithinBox(int dimension, Point3d box_a, Point3d box_b)
        {
            Dimension = dimension;

            var boxMin = default(Point3d);
            var boxMax = default(Point3d);

            for (int i = 0; i < Dimension; i++)
            {
                if (box_a[i] < box_b[i])
                {
                    boxMin[i] = box_a[i];
                    boxMax[i] = box_b[i];
                }
                else
                {
                    boxMin[i] = box_b[i];
                    boxMax[i] = box_a[i];
                }
            }

            BoxMin = boxMin;
            BoxMax = boxMax;
        }

        public int Dimension { get; }

        public Point3d BoxMin { get; }

        public Point3d BoxMax { get; }

        public bool Check(Point3d box_min, Point3d box_max)
        {
            for (int i = 0; i < Dimension; i++)
            {
                if (BoxMax[i] < box_min[i])
                    return false;

                if (BoxMin[i] > box_max[i])
                    return false;
            }

            return true;
        }
    }
}
