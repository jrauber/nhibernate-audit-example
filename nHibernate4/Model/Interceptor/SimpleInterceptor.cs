using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;
using log4net;
using nHibernate4.Model.Base;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace nHibernate4.Model.Interceptor
{
    public class SimpleInterceptor : EmptyInterceptor
    {
        private const string CHANGED_ON = "ChangedOn";
        private const string CHANGED_BY = "ChangedBy";

        private const string CREATED_ON = "CreatedOn";
        private const string CREATED_BY = "CreatedBy";

        private ISession _session;

        private static readonly ILog Log = LogManager.GetLogger(typeof(SimpleInterceptor));

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            Log.Debug(string.Format("Insert {0} , {1}", entity.GetType(), entity.ToString()));

            if (entity is IAuditable)
            {
                IAuditable auditEntity = entity as IAuditable;
                DateTime now = DateTime.Now;
                string user = WindowsIdentity.GetCurrent().Name;

                if (string.IsNullOrWhiteSpace(auditEntity.CreatedBy))
                {
                    int idxCreatedOn = GetIndex(propertyNames, CREATED_ON);
                    int idxCreatedBy = GetIndex(propertyNames, CREATED_BY);

                    auditEntity.CreatedBy = user;
                    auditEntity.CreatedOn = now;

                    state[idxCreatedOn] = now;
                    state[idxCreatedBy] = user;
                }
            }

            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
            IType[] types)
        {
            Log.Debug(string.Format("Update {0} , {1}", entity.GetType(), entity.ToString()));

            if (entity is IAuditable)
            {
                IAuditable auditEntity = entity as IAuditable;
                DateTime now = DateTime.Now;
                string user = WindowsIdentity.GetCurrent().Name;

                int idxChangedOn = GetIndex(propertyNames, CHANGED_ON);
                int idxChangedBy = GetIndex(propertyNames, CHANGED_BY);

                auditEntity.ChangedBy = user;
                auditEntity.ChangedOn = now;

                currentState[idxChangedOn] = now;
                currentState[idxChangedBy] = user;
            }

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        private int GetIndex(object[] propertyNames, string property)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i].ToString().Equals(property))
                {
                    return i;
                }
            }
            return -1;
        }

        public override SqlString OnPrepareStatement(SqlString sql)
        {
            return base.OnPrepareStatement(sql);
        }

        public override void PostFlush(ICollection entities)
        {
            base.PostFlush(entities);
        }

        public override void SetSession(ISession session)
        {
            _session = session;
        }

        public override void PreFlush(ICollection entitites)
        {
            base.PreFlush(entitites);
        }

        public override void OnCollectionRecreate(object collection, object key)
        {
            base.OnCollectionRecreate(collection, key);
        }

        public override void OnCollectionRemove(object collection, object key)
        {
            base.OnCollectionRemove(collection, key);
        }

        public override void AfterTransactionBegin(ITransaction tx)
        {
            base.AfterTransactionBegin(tx);
        }

        public override void BeforeTransactionCompletion(ITransaction tx)
        {
            base.BeforeTransactionCompletion(tx);
        }
    }
}
