using System;
using System.Security.Principal;
using nHibernate4.Model.Base;
using NHibernate;
using NHibernate.Classic;

namespace nHibernate4.Model.LifecycleExample
{
    public class AddressILifecycle : ModelBaseAudit, ILifecycle
    {
        public virtual string Street { get; set; }

        public virtual string HouseNumber { get; set; }

        public virtual string ZipCode { get; set; }

        public virtual string City { get; set; }

        public virtual AccountILifecycle Account { get; set; }

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

        public virtual LifecycleVeto OnDelete(ISession s)
        {
            return LifecycleVeto.NoVeto;
        }

        public virtual void OnLoad(ISession s, object id)
        {
            
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