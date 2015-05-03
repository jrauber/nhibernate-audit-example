using System.Collections.Generic;
using nHibernate4.Model.Base;
using nHibernate4.Model.LifecycleExample;
using NHibernate.Classic;

namespace nHibernate4.Model
{
    public class ParentILifecycle : ModelBase, ILifecycle
    {
        public ParentILifecycle()
        {
            Children = new HashSet<ChildILifecycle>();
        }

        public virtual string Name { get; set; }

        public virtual ISet<ChildILifecycle> Children { get; set; }

        public virtual LifecycleVeto OnDelete(NHibernate.ISession s)
        {
            return LifecycleVeto.NoVeto;
        }

        public virtual void OnLoad(NHibernate.ISession s, object id)
        {
           
        }

        public virtual LifecycleVeto OnSave(NHibernate.ISession s)
        {
            return LifecycleVeto.NoVeto;
        }

        public virtual LifecycleVeto OnUpdate(NHibernate.ISession s)
        {
            return LifecycleVeto.NoVeto;
        }
    }
}