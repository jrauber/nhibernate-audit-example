using System.Collections.Generic;
using nHibernate4.Model.Base;
using NHibernate.Classic;
using NHibernate.Envers.Configuration.Attributes;

namespace nHibernate4.Model
{
    [Audited]
    public class Parent : ModelBaseAudit
    {
        public Parent()
        {
            Children = new HashSet<Child>();
        }

        public virtual string Name { get; set; }

        public virtual ISet<Child> Children { get; set; }

        public override string ToString()
        {
            return Id + "#" + Name;
        }
    }
}