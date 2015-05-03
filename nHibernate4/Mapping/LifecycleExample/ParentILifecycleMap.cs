using nHibernate4.Mapping.Base;
using nHibernate4.Model;
using NHibernate.Mapping.ByCode;

namespace nHibernate4.Mapping.LifecycleExample
{
    public class ParentILifecycleMap : MapBaseEnhSeq<ParentILifecycle>
    {
        public ParentILifecycleMap()
        {
            Property(x => x.Name);

            Set(x => x.Children, m => { m.Cascade(Cascade.All); }, z => { z.OneToMany(o => { }); });
        }
    }
}