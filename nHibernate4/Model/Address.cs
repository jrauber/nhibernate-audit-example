using nHibernate4.Model.Base;
using NHibernate.Envers.Configuration.Attributes;

namespace nHibernate4.Model
{
    [Audited]
    public class Address : ModelBaseAudit
    {
        public virtual string Street { get; set; }

        public virtual string HouseNumber { get; set; }

        public virtual string ZipCode { get; set; }

        public virtual string City { get; set; }

        public virtual Account Account { get; set; }

        public override string ToString()
        {
            return string.Format("#{0}#{1}#{2}#{3}#", Street, HouseNumber, ZipCode, City);
        }
    }
}