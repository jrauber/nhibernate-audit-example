using System;
using System.Collections.Generic;
using nHibernate4.Model.Base;
using NHibernate.Envers;
using NHibernate.Envers.Configuration.Attributes;

namespace nHibernate4.Model.Envers
{
    [RevisionEntity(typeof(SimpleRevInfoListener))]
    public class EnversRevInfo 
    {
        [RevisionNumber]
        public virtual long Id { get; set; }

        [RevisionTimestamp]
        public virtual long RevisionTimestamp
        {
            get; set; 
        }

        public virtual string UserName { get; set; }

        [ModifiedEntityNames]
        public virtual ISet<string> ChangedObjects { get; set; }
    }
}
