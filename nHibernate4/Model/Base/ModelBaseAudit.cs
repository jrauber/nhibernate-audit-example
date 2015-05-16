using System;
using System.IO;
using NHibernate;
using NHibernate.Classic;
using NHibernate.Envers.Configuration.Attributes;

namespace nHibernate4.Model.Base
{
    public abstract class ModelBaseAudit : ModelBase, IAuditable
    {
        public virtual DateTime? CreatedOn { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTime? ChangedOn { get; set; }

        public virtual string ChangedBy { get; set; }
    }
}