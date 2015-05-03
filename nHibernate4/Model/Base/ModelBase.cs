using System;
using NHibernate;
using NHibernate.Classic;

namespace nHibernate4.Model.Base
{
    public abstract class ModelBase : IAuditable
    {
        public virtual long Id { get; set; }

        public virtual int Version { get; set; }

        public virtual DateTime CreatedOn { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTime ChangedOn { get; set; }

        public virtual string ChangedBy { get; set; }
    }
}