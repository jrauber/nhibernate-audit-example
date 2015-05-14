using nHibernate4.Mapping.Base;
using nHibernate4.Model;

namespace nHibernate4.Mapping
{
    public class ChildMap : MapBaseEnhSeqAudit<Child>
    {
        public ChildMap()
        {
            Property(x => x.Name);

            ManyToOne(x => x.Parent);
        }
    }
}