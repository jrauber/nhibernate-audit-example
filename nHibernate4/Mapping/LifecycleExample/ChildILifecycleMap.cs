using nHibernate4.Mapping.Base;
using nHibernate4.Model.LifecycleExample;

namespace nHibernate4.Mapping.LifecycleExample
{
    public class ChildILifecycleMap : MapBaseEnhSeq<ChildILifecycle>
    {
        public ChildILifecycleMap()
        {
            Property(x => x.Name);

            ManyToOne(x => x.Parent);
        }
    }
}