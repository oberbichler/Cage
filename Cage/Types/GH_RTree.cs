using Grasshopper.Kernel.Types;

namespace Cage.Types
{
    class GH_RTree : GH_Goo<RTree>
    {
        public GH_RTree() : base()
        {
        }

        public GH_RTree(RTree value) : base(value)
        {
        }

        public override bool IsValid => Value != null;

        public override string TypeName => "cage RTree";

        public override string TypeDescription => "";

        public override bool CastFrom(object source)
        {
            switch (source)
            {
                case RTree value:
                    Value = value;
                    return true;
                case GH_RTree ghValue:
                    Value = ghValue.Value;
                    return true;
                default:
                    return base.CastFrom(source);
            }
        }

        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q).IsAssignableFrom(m_value.GetType()))
            {
                var obj = (object)m_value;
                target = (Q)obj;
                return true;
            }

            return false;
        }

        public override IGH_Goo Duplicate()
        {
            return new GH_RTree(Value);
        }

        public override string ToString() => Value.ToString();
    }
}