using nHibernate4.Mapping.Base;
using nHibernate4.Model.LifecycleExample;

namespace nHibernate4.Mapping.LifecycleExample
{
    public class AddressILifecycleMap : MapBaseEnhSeqAudit<AddressILifecycle>
    {
        public AddressILifecycleMap()
        {
            Property(x => x.Street);

            Property(x => x.HouseNumber);

            Property(x => x.ZipCode);

            Property(x => x.City);

            ManyToOne(x => x.Account);
        }
    }
}