using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using log4net;
using nHibernate4.Model.Base;
using nHibernate4.Model.Interceptor;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Type;

namespace nHibernate4.Model.Listener
{
    public class SimpleListener : IPreInsertEventListener, IPreUpdateEventListener, IPreDeleteEventListener, 
                                  IPreCollectionUpdateEventListener, IPostCollectionUpdateEventListener, IPreCollectionRemoveEventListener
    {
        private const string CHANGED_ON = "ChangedOn";
        private const string CHANGED_BY = "ChangedBy";

        private const string CREATED_ON = "CreatedOn";
        private const string CREATED_BY = "CreatedBy";

        private static readonly ILog Log = LogManager.GetLogger(typeof(SimpleListener));

        public bool OnPreInsert(PreInsertEvent @event)
        {
            Log.Debug("OnPreInsert : " + @event.Entity);

            IAuditable entity = @event.Entity as IAuditable;

            if (entity != null)
            {
                IAuditable auditEntity = entity as IAuditable;
                DateTime now = DateTime.Now;
                string user = GetCurrentUserName();

                int idxChangedOn = GetIndex(@event.Persister.PropertyNames, CREATED_ON);
                int idxChangedBy = GetIndex(@event.Persister.PropertyNames, CREATED_BY);

                auditEntity.CreatedBy = user;
                auditEntity.CreatedOn = now;

                @event.State[idxChangedOn] = now;
                @event.State[idxChangedBy] = user;
            }

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            Log.Debug("OnPreUpdate : " + @event.Entity);

            IAuditable entity = @event.Entity as IAuditable;

            if (entity != null)
            {
                IAuditable auditEntity = entity as IAuditable;
                DateTime now = DateTime.Now;
                string user = GetCurrentUserName();

                int idxChangedOn = GetIndex(@event.Persister.PropertyNames, CHANGED_ON);
                int idxChangedBy = GetIndex(@event.Persister.PropertyNames, CHANGED_BY);

                auditEntity.ChangedBy = user;
                auditEntity.ChangedOn = now;

                @event.State[idxChangedOn] = now;
                @event.State[idxChangedBy] = user;
            }

            return false;
        }

        private int GetIndex(string[] propertyNames, string property)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i].Equals(property))
                {
                    return i;
                }
            }
            return -1;
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

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            Log.Debug("OnPreDelete : " + @event.Entity);

            return false;
        }

        public void OnPreUpdateCollection(PreCollectionUpdateEvent @event)
        {
            
        }

        public void OnPostUpdateCollection(PostCollectionUpdateEvent @event)
        {
           
        }

        public void OnPreRemoveCollection(PreCollectionRemoveEvent @event)
        {
           
        }
    }
}
