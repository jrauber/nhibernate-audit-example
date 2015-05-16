using nHibernate4.Model.Base;
using NHibernate;
using NHibernate.Classic;

namespace nHibernate4.Model.LifecycleExample
{
    public class ChildILifecycle : ModelBaseAudit, ILifecycle
    {
        public virtual string Name { get; set; }

        public virtual ParentILifecycle Parent { get; set; }

        public virtual LifecycleVeto OnSave(ISession s)
        {
            return LifecycleVeto.NoVeto;
        }

        public virtual LifecycleVeto OnUpdate(ISession s)
        {
            return LifecycleVeto.NoVeto;
        }

        public virtual LifecycleVeto OnDelete(ISession s)
        {
            return LifecycleVeto.NoVeto;
        }

        public virtual void OnLoad(ISession s, object id)
        {
            
        }
    }
}