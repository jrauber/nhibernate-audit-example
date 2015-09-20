using nHibernate4.Mapping.Base;
using nHibernate4.Model;

namespace nHibernate4.Mapping
{
    public class AddressMap : MapBaseEnhSeqAudit<Address>
    {
        public AddressMap()
        {
            Property(x => x.Street);

            Property(x => x.HouseNumber);

            Property(x => x.ZipCode);

            Property(x => x.City);

            ManyToOne(x => x.Account, x => x.ForeignKey("FK_Address_Account"));
        }
    }
}