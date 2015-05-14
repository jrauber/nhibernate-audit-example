using nHibernate4.Model.Base;

namespace nHibernate4.Model
{
    public class Child : ModelBase
    {
        public virtual string Name { get; set; }

        public virtual Parent Parent { get; set; }

        public override string ToString()
        {
            return Id + "#" + Name;
        }
    }
}