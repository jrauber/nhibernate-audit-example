using nHibernate4.Model.Base;
using NHibernate.Envers.Configuration.Attributes;

namespace nHibernate4.Model
{
    [Audited]
    public class Child : ModelBaseAudit
    {
        public virtual string Name { get; set; }

        public virtual Parent Parent { get; set; }

        public override string ToString()
        {
            return Id + "#" + Name;
        }
    }
}