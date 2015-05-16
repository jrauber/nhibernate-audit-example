using System;
using System.Collections.Generic;
using System.Security.Principal;
using nHibernate4.Model.Base;
using nHibernate4.Model.LifecycleExample;
using NHibernate;
using NHibernate.Classic;

namespace nHibernate4.Model
{
    public class ParentILifecycle : ModelBaseAudit, ILifecycle
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

        public virtual LifecycleVeto OnSave(ISession s)
        {
            IAuditable entity = this as IAuditable;

            if (entity != null)
            {
                entity.CreatedOn = DateTime.Now;
                entity.CreatedBy = GetCurrentUserName();
            }

            return LifecycleVeto.NoVeto;
        }

        public virtual LifecycleVeto OnUpdate(ISession s)
        {
            IAuditable entity = this as IAuditable;

            if (entity != null)
            {
                entity.ChangedOn = DateTime.Now;
                entity.ChangedBy = GetCurrentUserName();
            }

            return LifecycleVeto.NoVeto;
        }

        private string GetCurrentUserName()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();

            if (windowsIdentity != null)
            {
                return windowsIdentity.Name;
            }

            return String.Empty;
        }
    }
}