using System;
using NHibernate;
using NHibernate.Classic;

namespace nHibernate4.Model.Base
{
    public abstract class ModelBase
    {
        public virtual long Id { get; set; }

        public virtual int Version { get; set; }
    }
}