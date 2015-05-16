using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using nHibernate4.Model.Interceptor;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Type;

namespace nHibernate4.Model.Listener
{
    public class SimpleListener : IPreInsertEventListener, IPreUpdateEventListener, IPreDeleteEventListener, 
                                  IPreCollectionUpdateEventListener, IPostCollectionUpdateEventListener, IPreCollectionRemoveEventListener
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SimpleListener));

        public bool OnPreInsert(PreInsertEvent @event)
        {
            Log.Debug("OnPreInsert : " + @event.Entity);

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            Log.Debug("OnPreUpdate : " + @event.Entity);

            return false;
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
