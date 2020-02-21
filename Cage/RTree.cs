using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Cage
{
    class RTree
    {
        int m_nb_items;
        int m_node_size;
        List<int> m_level_bounds = new List<int>();
        Point3d m_min;
        Point3d m_max;
        int m_position;
        public int[] m_indices;
        Point3d[] m_boxes_min;
        Point3d[] m_boxes_max;
        HilbertCurve m_hilbert;

        public List<GeometryBase> Geometries { get; set; } // FIXME: Cleaner solution

        public int Dimension { get; private set; }

        public RTree(int dimension, int nb_items, int node_size = 16)
        {
            if (dimension < 1)
                throw new ArgumentOutOfRangeException(nameof(dimension));
            if (nb_items < 0)
                throw new ArgumentOutOfRangeException(nameof(nb_items));
            if (node_size < 2)
                throw new ArgumentOutOfRangeException(nameof(node_size));

            Dimension = dimension;

            m_hilbert = new HilbertCurve(Dimension);

            m_nb_items = nb_items;
            m_node_size = Math.Min(Math.Max(node_size, 2), int.MaxValue);

            int n = nb_items;
            int nb_nodes = n;

            m_level_bounds.Add(n);

            do
            {
                n = (int)Math.Ceiling((double)n / m_node_size);
                nb_nodes += n;
                m_level_bounds.Add(nb_nodes);
            } while (n > 1);

            for (int i = 0; i < Dimension; i++)
            {
                m_min[i] = double.PositiveInfinity;
                m_max[i] = double.NegativeInfinity;
            }

            m_position = 0;
            m_indices = new int[nb_nodes];
            m_boxes_min = new Point3d[nb_nodes];
            m_boxes_max = new Point3d[nb_nodes];
        }

        void Sort(ulong[] values, int left, int right)
        {
            if (left >= right)
            {
                return;
            }

            ulong pivot = values[(left + right) >> 1];

            int i = left - 1;
            int j = right + 1;

            while (true)
            {
                do
                {
                    i += 1;
                } while (values[i] < pivot);

                do
                {
                    j -= 1;
                } while (values[j] > pivot);

                if (i >= j)
                {
                    break;
                }

                Swap(values, i, j);
            }

            Sort(values, left, j);
            Sort(values, j + 1, right);
        }

        private void Swap<T>(T[] a, T[] b, int i, int j)
        {
            var tmp = a[i];
            a[i] = b[j];
            b[j] = tmp;
        }

        private void Swap(ulong[] values, int i, int j)
        {
            Swap(values, values, i, j);
            Swap(m_indices, m_indices, i, j);
            Swap(m_boxes_min, m_boxes_min, i, j);
            Swap(m_boxes_max, m_boxes_max, i, j);
        }

        public void Add(Point3d box_a, Point3d box_b)
        {
            if (m_position >= m_nb_items)
                throw new Exception("Invalid number of items");

            int index = m_position++;

            m_indices[index] = index;

            var box_min = Point3d.Origin;
            var box_max = Point3d.Origin;

            for (int i = 0; i < Dimension; i++)
            {
                if (box_a[i] < box_b[i])
                {
                    box_min[i] = box_a[i];
                    box_max[i] = box_b[i];
                }
                else
                {
                    box_min[i] = box_b[i];
                    box_max[i] = box_a[i];
                }
            }

            m_boxes_min[index] = box_min;
            m_boxes_max[index] = box_max;

            for (int i = 0; i < Dimension; i++)
            {
                if (box_min[i] < m_min[i])
                    m_min[i] = box_min[i];
                if (box_max[i] > m_max[i])
                    m_max[i] = box_max[i];
            }
        }

        public void Finish()
        {
            if (m_position != m_nb_items)
                throw new Exception("Invalid number of items");

            Vector3d size = m_max - m_min;

            ulong[] hilbert_values = new ulong[m_nb_items];

            for (int i = 0; i < m_nb_items; i++)
            {
                Point3d box_min = m_boxes_min[i];
                Point3d box_max = m_boxes_max[i];

                ulong[] center = new ulong[3];

                for (int j = 0; j < Dimension; j++)
                {
                    center[j] = (ulong)(((box_min[j] + box_max[j]) / 2 - m_min[j]) / size[j] * m_hilbert.MaxAxisSize());
                }

                hilbert_values[i] = m_hilbert.IndexAt(center);
            }

            Sort(hilbert_values, 0, m_nb_items - 1);

            int pos = 0;

            for (int i = 0; i < m_level_bounds.Count - 1; i++)
            {
                int end = m_level_bounds[i];

                while (pos < end)
                {
                    Vector3d node_min = new Vector3d();
                    Vector3d node_max = new Vector3d();

                    for (int j = 0; j < Dimension; j++)
                    {
                        node_min[j] = double.PositiveInfinity;
                        node_max[j] = double.NegativeInfinity;
                    }

                    int node_index = pos;

                    for (int j = 0; j < m_node_size && pos < end; j++)
                    {
                        Point3d box_min = m_boxes_min[pos];
                        Point3d box_max = m_boxes_max[pos];

                        pos += 1;

                        for (int k = 0; k < Dimension; k++)
                        {
                            if (box_min[k] < node_min[k])
                            {
                                node_min[k] = box_min[k];
                            }
                            if (box_max[k] > node_max[k])
                            {
                                node_max[k] = box_max[k];
                            }
                        }
                    }

                    m_indices[m_position] = node_index;
                    m_boxes_min[m_position] = (Point3d)node_min;
                    m_boxes_max[m_position] = (Point3d)node_max;

                    m_position += 1;
                }
            }
        }

        public bool Check(Point3d m_box_min, Point3d m_box_max, Point3d box_min, Point3d box_max)
        {
            for (int i = 0; i < Dimension; i++)
            {
                if (m_box_max[i] < box_min[i])
                    return false;
                if (m_box_min[i] > box_max[i])
                    return false;
            }
            return true;
        }

        public List<int> Search(ICheck check, Func<int, bool> callback)
        {
            if (m_position != m_indices.Length)
                throw new Exception("Data not yet indexed - call RTree::finish().");

            var node_index = m_indices.Length - 1;
            var level = m_level_bounds.Count - 1;
            var queue = new Queue<int>();
            var results = new List<int>();

            while (node_index > -1)
            {
                var end = Math.Min(node_index + m_node_size, m_level_bounds[level]);

                for (var pos = node_index; pos < end; pos++)
                {
                    int index = m_indices[pos];

                    var node_min = m_boxes_min[pos];
                    var node_max = m_boxes_max[pos];

                    if (!check.Check(node_min, node_max))
                    {
                        continue;
                    }

                    if (node_index < m_nb_items)
                    {
                        if (callback == null || callback(index))
                        {
                            results.Add(index);
                        }
                    }
                    else
                    {
                        queue.Enqueue(index);
                        queue.Enqueue(level - 1);
                    }
                }

                if (queue.Count == 0)
                {
                    node_index = -1;
                    level = -1;
                }
                else
                {
                    level = queue.Dequeue();
                    node_index = queue.Dequeue();
                }
            }

            return results;
        }

        public List<int> WithinBox(Point3d box_a, Point3d box_b, Func<int, bool> callback = null)
        {
            var check = new WithinBox(Dimension, box_a, box_b);
            return Search(check, callback);
        }

        public List<int> HitByRay(Point3d origin, Vector3d direction, Func<int, bool> callback = null)
        {
            var check = new HitByRay(Dimension, origin, direction);
            return Search(check, callback);
        }
    }
}
