using System;
using System.Security.Principal;
using log4net;
using nHibernate4.Model.Base;
using NHibernate.Event;

namespace nHibernate4.Model.Listener
{
    public class SimpleListener : IPreInsertEventListener, IPreUpdateEventListener, IPreDeleteEventListener, 
                                  IPreCollectionUpdateEventListener, IPostCollectionUpdateEventListener, IPreCollectionRemoveEventListener
    {
        #region const

        private const string CHANGED_ON = "ChangedOn";
        private const string CHANGED_BY = "ChangedBy";

        private const string CREATED_ON = "CreatedOn";
        private const string CREATED_BY = "CreatedBy";

        private static readonly ILog Log = LogManager.GetLogger(typeof(SimpleListener));
        
        #endregion

        public bool OnPreInsert(PreInsertEvent args)
        {
            Log.Debug("OnPreInsert : " + args.Entity);

            IAuditable entity = args.Entity as IAuditable;

            if (entity != null)
            {
                IAuditable auditEntity = entity as IAuditable;
                DateTime now = DateTime.Now;
                string user = GetCurrentUserName();

                int idxChangedOn = GetIndex(args.Persister.PropertyNames, CREATED_ON);
                int idxChangedBy = GetIndex(args.Persister.PropertyNames, CREATED_BY);

                auditEntity.CreatedBy = user;
                auditEntity.CreatedOn = now;

                args.State[idxChangedOn] = now;
                args.State[idxChangedBy] = user;
            }

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent args)
        {
            Log.Debug("OnPreUpdate : " + args.Entity);

            IAuditable entity = args.Entity as IAuditable;

            if (entity != null)
            {

                var dirtyField = args.Persister.FindDirty(args.State, args.OldState, args.Entity, args.Session);

                IAuditable auditEntity = entity as IAuditable;
                DateTime now = DateTime.Now;
                string user = GetCurrentUserName();

                int idxChangedOn = GetIndex(args.Persister.PropertyNames, CHANGED_ON);
                int idxChangedBy = GetIndex(args.Persister.PropertyNames, CHANGED_BY);

                auditEntity.ChangedBy = user;
                auditEntity.ChangedOn = now;

                args.State[idxChangedOn] = now;
                args.State[idxChangedBy] = user;
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
