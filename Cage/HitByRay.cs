using Rhino.Geometry;
using System;

namespace Cage
{
    public class HitByRay : ICheck
    {
        public HitByRay(int dimension, Point3d origin, Vector3d direction)
        {
            Dimension = dimension;
            Origin = origin;
            Direction = direction;
        }

        public int Dimension { get; }

        public Point3d Origin { get; }

        public Vector3d Direction { get; }

        public bool Check(Point3d box_min, Point3d box_max)
        {
            // based on Fast Ray-Box Intersection
            // by Andrew Woo
            // from "Graphics Gems", Academic Press, 1990

            var inside = true;
            var quadrant = new int[Dimension];
            var maxT = Vector3d.Zero;
            var candidatePlane = Vector3d.Zero;
            var coordinate = Vector3d.Zero;

            for (int i = 0; i < Dimension; i++)
            {
                if (Origin[i] < box_min[i])
                {
                    quadrant[i] = -1;
                    candidatePlane[i] = box_min[i];
                    inside = false;
                }
                else if (Origin[i] > box_max[i])
                {
                    quadrant[i] = 1;
                    candidatePlane[i] = box_max[i];
                    inside = false;
                }
                else
                    quadrant[i] = 0;
            }

            if (inside)
            {
                // coordinate = m_origin;
                return true;
            }

            for (int i = 0; i < Dimension; i++)
            {
                if (quadrant[i] != 0 && Direction[i] != 0)
                    maxT[i] = (candidatePlane[i] - Origin[i]) / Direction[i];
                else
                    maxT[i] = -1;
            }

            var which_plane = 0;

            for (int i = 1; i < Dimension; i++)
            {
                if (maxT[which_plane] < maxT[i])
                    which_plane = i;
            }

            if (maxT[which_plane] < 0)
                return false;

            for (int i = 0; i < Dimension; i++)
            {
                if (which_plane != i)
                {
                    coordinate[i] = Origin[i] + maxT[which_plane] * Direction[i];
                    if (coordinate[i] < box_min[i] || coordinate[i] > box_max[i])
                        return false;
                }
                else
                    coordinate[i] = candidatePlane[i];
            }

            return true;
        }
    };
}
