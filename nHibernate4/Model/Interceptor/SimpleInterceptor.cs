using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using nHibernate4.Model;
using NHibernate;
using NHibernate.Hql.Util;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace nHibernate4
{
    public class SimpleInterceptor : EmptyInterceptor
    {
        private ISession localSession;

        public SimpleInterceptor()
        {
          
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return base.OnSave(entity, id, state, propertyNames, types);
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
            localSession = session;
        }

        public override void PreFlush(ICollection entitites)
        {
            //var parent = new Parent() { Name = "TTTTTTTT" };

            //localSession.Save(parent);

            base.PreFlush(entitites);
        }

        public override void AfterTransactionBegin(ITransaction tx)
        {
            base.AfterTransactionBegin(tx);
        }

        public override void BeforeTransactionCompletion(ITransaction tx)
        {
            base.BeforeTransactionCompletion(tx);
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
            IType[] types)
        {
            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        //public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        //{
            //var time = DateTime.Now;
            //var userName = // Find user name here 

            //var typedEntity = (BaseEntity)entity;
            //typedEntity.Created = time;
            //typedEntity.CreatedBy = userName;
            //typedEntity.Modified = time;
            //typedEntity.ModifiedBy = userName;

            //var indexOfCreated = GetIndex(propertyNames, "Created");
            //var indexOfCreatedBy = GetIndex(propertyNames, "CreatedBy");
            //var indexOfModified = GetIndex(propertyNames, "Modified");
            //var indexOfModifiedBy = GetIndex(propertyNames, "ModifiedBy");

            //state[indexOfCreated] = time;
            //state[indexOfCreatedBy] = userName;
            //state[indexOfModified] = time;
            //state[indexOfModifiedBy] = userName;

            //return base.OnSave(entity, id, state, propertyNames, types);
        //}

        private int GetIndex(object[] array, string property)
        {
            for (var i = 0; i < array.Length; i++)
                if (array[i].ToString() == property)
                    return i;

            return -1;
        }
    }
}
