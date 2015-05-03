using nHibernate4.Mapping.Base;
using nHibernate4.Model;
using NHibernate.Mapping.ByCode;

namespace nHibernate4.Mapping
{
    public class ParentMap : MapBaseEnhSeq<Parent>
    {
        public ParentMap()
        {
            Property(x => x.Name);

            Set(x => x.Children, m => { m.Cascade(Cascade.All); }, z => { z.OneToMany(o => { }); });
        }
    }
}