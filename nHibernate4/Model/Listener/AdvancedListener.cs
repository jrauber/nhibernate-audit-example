using System;
using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Intercept;
using NHibernate.Persister.Entity;
using nHibernate4.Model.Base;
using System.Security.Principal;

namespace nHibernate4.Model.Listener
{
    /// <summary>
    /// Improves the SimpleListener which is flawed
    /// see: http://nhibernate.info/doc/howto/various/changing-values-in-nhibernate-events.html
    /// 
    /// First implementation see: http://fabiomaulo.blogspot.de/2011/05/nhibernate-bizarre-audit.html
    /// Second implementation see: https://github.com/Buthrakaur/NHListenerTests/blob/master/NHListenerTest/SetModificationTimeFlushEntityEventListener.cs
    /// </summary>
    public class AdvancedListener :  IFlushEntityEventListener,
                                     ISaveOrUpdateEventListener, 
                                     IMergeEventListener
    {
        public AdvancedListener()
        {
            CurrentDateTimeProvider = () => DateTime.Now;
        }

        public Func<DateTime> CurrentDateTimeProvider { get; set; }

        private string GetCurrentUserName()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();

            if (windowsIdentity != null)
            {
                return windowsIdentity.Name;
            }

            return String.Empty;
        }

        public void OnFlushEntity(FlushEntityEvent @event)
        {
            var entity = @event.Entity;
            var entityEntry = @event.EntityEntry;

            if (entityEntry.Status == Status.Deleted)
            {
                return;
            }
            var trackable = entity as IAuditable;
            if (trackable == null)
            {
                return;
            }
            if (HasDirtyProperties(@event))
            {
                trackable.ChangedOn = CurrentDateTimeProvider();
                trackable.ChangedBy = GetCurrentUserName();
            }
        }

        private bool HasDirtyProperties(FlushEntityEvent @event)
        {
            ISessionImplementor session = @event.Session;
            EntityEntry entry = @event.EntityEntry;
            var entity = @event.Entity;
            if (!entry.RequiresDirtyCheck(entity) || !entry.ExistsInDatabase || entry.LoadedState == null)
            {
                return false;
            }
            IEntityPersister persister = entry.Persister;

            object[] currentState = persister.GetPropertyValues(entity, session.EntityMode);
            object[] loadedState = entry.LoadedState;

            return persister.EntityMetamodel.Properties
                .Where((property, i) => !LazyPropertyInitializer.UnfetchedProperty.Equals(currentState[i]) && property.Type.IsDirty(loadedState[i], currentState[i], session))
                .Any();
        }

        public void OnSaveOrUpdate(SaveOrUpdateEvent @event)
        {
            ExplicitUpdateCall(@event.Entity as IAuditable);
        }

        public void OnMerge(MergeEvent @event)
        {
            ExplicitUpdateCall(@event.Entity as IAuditable);
        }

        public void OnMerge(MergeEvent @event, IDictionary copiedAlready)
        {
            ExplicitUpdateCall(@event.Entity as IAuditable);
        }

        private void ExplicitUpdateCall(IAuditable trackable)
        {
            if (trackable == null)
            {
                return;
            }
            trackable.ChangedOn = CurrentDateTimeProvider();
            trackable.ChangedBy = GetCurrentUserName();
        }

        public void Register(Configuration cfg)
        {
            var listeners = cfg.EventListeners;
            listeners.FlushEntityEventListeners = new[] { this }
                .Concat(listeners.FlushEntityEventListeners)
                .ToArray();
            listeners.SaveEventListeners = new[] { this }
                .Concat(listeners.SaveEventListeners)
                .ToArray();
            listeners.SaveOrUpdateEventListeners = new[] { this }
                .Concat(listeners.SaveOrUpdateEventListeners)
                .ToArray();
            listeners.UpdateEventListeners = new[] { this }
                .Concat(listeners.UpdateEventListeners)
                .ToArray();
            listeners.MergeEventListeners = new[] { this }
                .Concat(listeners.MergeEventListeners)
                .ToArray();
        }
    }
}