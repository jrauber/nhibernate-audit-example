using nHibernate4.Model.Base;
using NHibernate;
using NHibernate.Classic;

namespace nHibernate4.Model.LifecycleExample
{
    public class ChildILifecycle : ModelBase, ILifecycle
    {
        public virtual string Name { get; set; }

        public virtual ParentILifecycle Parent { get; set; }

        public virtual LifecycleVeto OnSave(ISession s)
        {
            throw new System.NotImplementedException();
        }

        public virtual LifecycleVeto OnUpdate(ISession s)
        {
            throw new System.NotImplementedException();
        }

        public virtual LifecycleVeto OnDelete(ISession s)
        {
            throw new System.NotImplementedException();
        }

        public virtual void OnLoad(ISession s, object id)
        {
            throw new System.NotImplementedException();
        }
    }
}