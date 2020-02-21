namespace Cage
{
    class HilbertCurve
    {
        public int Dimension { get; private set; }

        public HilbertCurve(int dimension)
        {
            Dimension = dimension;
        }

        ulong Log2(ulong value)
        {
            ulong result = 0;

            while ((value >>= 1) != 0) {
                result += 1;
            }

            return result;
        }

        ulong Rol(ulong x, int d)
        {
            int c = d % Dimension >= 0 ? d % Dimension : d % Dimension + Dimension;
            return (x << c | x >> (Dimension - c)) & (((ulong)1 << Dimension) - 1);
        }

        ulong Ror(ulong x, int d)
        {
            int c = d % Dimension >= 0 ? d % Dimension : d % Dimension + Dimension;
            return (x >> c | x << (Dimension - c)) & (((ulong)1 << Dimension) - 1);
        }

        ulong Bit(ulong x, int i)
        {
            return x >> i & 1;
        }

        ulong Gc(ulong i)
        {
            return i ^ (i >> 1);
        }

        ulong E(ulong i)
        {
            return i == 0 ? 0 : Gc(((i - 1) >> 1) << 1);
        }

        ulong InverseGc(ulong gc)
        {
            ulong i = gc;
            int j = 1;

            while (j < Dimension) {
                i ^= gc >> j;
                j += 1;
            }

            return i;
        }

        ulong G(ulong i)
        {
            return Log2(Gc(i) ^ Gc(i + 1));
        }

        int D(ulong i)
        {
            if (i == 0) {
                return 0;
            } else if (i % 2 == 0) {
                return (int)(G(i - 1) % (ulong)Dimension);
            } else {
                return (int)(G(i) % (ulong)Dimension);
            }
        }

        ulong T(ulong e, int d, ulong b)
        {
            return Ror(b ^ e, d + 1);
        }

        ulong InverseT(ulong e, int d, ulong b)
        {
            return T(Ror(e, d + 1), Dimension - d - 2, b);
        }

        public ulong IndexAt(int m, ulong[] p)
        {
            // FIXME: check m

            ulong h = 0;
            ulong ve = 0;
            int vd = 0;

            for (int i = m - 1; i > -1; i--) {
                ulong s = 0;

                for (int j = 0; j<Dimension; j++) {
                    s += Bit(p[j], i) << j;
                }

                ulong l = T(ve, vd, s);
                ulong w = InverseGc(l);

                ve ^= (Rol(E(w), vd + 1));
                vd = (vd + D(w) + 1) % Dimension;
                h = (h << Dimension) | w;
            }

            return h;
        }

        public ulong IndexAt(ulong[] p)
        {
            int m = MaxM();
            return IndexAt(m, p);
        }

        public ulong MaxAxisSize()
        {
            return (ulong)1 << MaxM();
        }

        public int MaxM()
        {
            return 8 * sizeof(ulong) / Dimension;
        }

        public ulong[] PointAt(int m, ulong h)
        {
            // FIXME: check m

            ulong ve = 0;
            int vd = 0;

            var p = new ulong[3];

            for (int i = m - 1; i > -1; i--) {
                ulong w = 0;

                for (int j = 0; j<Dimension; j++) {
                    w += Bit(h, i* Dimension + j) << j;
                }

                ulong l = InverseT(ve, vd, Gc(w));

                for (int j = 0; j<Dimension; j++) {
                    p[j] += Bit(l, j) << i;
                }

                ve ^= Rol(E(w), vd + 1);
                vd = (vd + D(w) + 1) % Dimension;
            }

            return p;
        }

        public ulong[] PointAt(ulong h)
        {
            int m = MaxM();
            return PointAt(m, h);
        }
    }
}
