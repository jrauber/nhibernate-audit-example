using System.Collections.Generic;
using nHibernate4.Model.Base;
using NHibernate.Envers.Configuration.Attributes;

namespace nHibernate4.Model
{
    [Audited]
    public class Account : ModelBaseAudit
    {
        public Account()
        {
            Address = new HashSet<Address>();
        }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual ISet<Address> Address { get; set; }

        public override string ToString()
        {
            return string.Format("#{0}#{1}#", FirstName, LastName);
        }
    }
}