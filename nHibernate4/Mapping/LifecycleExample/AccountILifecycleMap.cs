using nHibernate4.Mapping.Base;
using nHibernate4.Model;
using NHibernate.Mapping.ByCode;

namespace nHibernate4.Mapping.LifecycleExample
{
    public class AccountILifecycleMap : MapBaseEnhSeqAudit<AccountILifecycle>
    {
        public AccountILifecycleMap()
        {
            Property(x => x.FirstName);

            Property(x => x.LastName);

            Set(x => x.Address,
                m =>
                {
                    m.Cascade(Cascade.All | Cascade.DeleteOrphans); 
                    m.Inverse(true);
                },
                z =>
                {
                    z.OneToMany(o => { });
                });
        }
    }
}