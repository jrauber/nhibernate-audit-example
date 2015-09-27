using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using nHibernate4.Model;
using nHibernate4.Model.Listener;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Envers;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Query;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;

namespace nHibernate4
{
    internal class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private const bool DROP_GENERATED_FK_AND_RI = false; // Drop all FK from DB if true

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            //ExampleILifecycle();

            //ExampleInterceptor();

            ExampleListener();

            //ExampleEnvers();
        }

        private static void ExampleEnvers()
        {
            var cfg = new Configuration().Configure();

            var mapper = new ConventionModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(mapping);
            
            // Default-Strategy does not output the end of a given REV
            //cfg.SetEnversProperty(ConfigurationKey.AuditStrategy, typeof(NHibernate.Envers.Strategy.DefaultAuditStrategy));

            // Validity-Strategy sets a revison end on changes
            cfg.SetEnversProperty(ConfigurationKey.AuditStrategy, typeof(NHibernate.Envers.Strategy.ValidityAuditStrategy));

            cfg.SetEnversProperty(ConfigurationKey.AuditStrategyValidityStoreRevendTimestamp, true); // Write timestap when revision ends
            cfg.SetEnversProperty(ConfigurationKey.TrackEntitiesChangedInRevision, true); // Write list of object-types in REV 
            cfg.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true); // Flag which values were changed 

            cfg.IntegrateWithEnvers(new AttributeConfiguration());

            var sf = cfg.BuildSessionFactory();

            //DropAllForeignKeysFromDatabase(cfg, sf, DROP_GENERATED_FK_AND_RI);

            #region Insert-Test-Data

            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var parent = new Account
                {
                    FirstName = "Alan",
                    LastName = "Turing"
                };

                var address1 = new Address
                {
                    Account = parent,
                    Street = "Adlington Road",
                    HouseNumber = "42",
                    ZipCode = "SK9",
                    City = "Wilmslow"
                };

                parent.Address.Add(address1);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            #endregion

            #region Update-Test-Data

            // rename existing Account
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                account.FirstName = "Alan Mathison";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // insert new entry into existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                var address1 = new Address
                {
                    Account = account,
                    Street = "Bletchley Park",
                    HouseNumber = "",
                    ZipCode = "MK3",
                    City = "Milton Keynes"
                };

                account.Address.Add(address1);

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // rename entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);
                Address address = account.Address.Single(c => c.Street.Contains("Bletchley"));

                address.HouseNumber = "HUT 8";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // rename entry in existing collection - again
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);
                Address address = account.Address.Single(c => c.Street.Contains("Bletchley"));

                address.HouseNumber = "HUT EIGHT";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // delete entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                Address address = account.Address.Single(c => c.Street.Contains("Adlington"));

                account.Address.Remove(address);

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // delete Account
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                session.Delete(account);

                tx.Commit();
            }

            #endregion

            #region query

            // Query object-state by revision number
            // -> All Accounts in (REVISION 5)
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                IAuditReader auditReader = session.Auditer();
                IEntityAuditQuery<Account> query = auditReader.CreateQuery().ForEntitiesAtRevision<Account>(5);
                IEnumerable<Account> entity = query.Results();

                tx.Commit();
            }

            // Query for entities related to a parent entity
            // -> All Addresses that existed in (REVISION 5) and that reference Account with ID 1
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                IAuditReader auditReader = session.Auditer();
                IEntityAuditQuery<Address> query = auditReader.CreateQuery().ForEntitiesAtRevision<Address>(5)
                                                                   .Add(AuditEntity.RelatedId("Account").Eq(1));

                IEnumerable<Address> entity = query.Results();

                tx.Commit();
            }

            // Querying for the minimal change revision (Rev-Number) after revision x for an entity
            // First Revision-Number that successes (REVISION 4) and contains changes of (Entity 22)
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                IAuditReader auditReader = session.Auditer();
                IAuditQuery query = auditReader.CreateQuery().ForRevisionsOfEntity(typeof(Address), false, true)
                                                                .AddProjection(AuditEntity.RevisionNumber().Min()) // minimal value projection
                                                                .Add(AuditEntity.Id().Eq(22)) // id of the searched entity
                                                                .Add(AuditEntity.RevisionNumber().Gt(4)); // lower boundary (excl.)

                IList entity = query.GetResultList();

                tx.Commit();
            }

            // querying for all addresses that do not have changes on a defined property compared to their
            // predecessor revision
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                IAuditReader auditReader = session.Auditer();
                
                // search in every revision
                var query1 = auditReader.CreateQuery().ForRevisionsOfEntity(typeof(Address), false, true)
	                                          .Add(AuditEntity.Property("ZipCode").HasNotChanged());

                IList entity1 = query1.GetResultList();

                // search in every revision below or equal to REV 6
                var query2 = auditReader.CreateQuery().ForEntitiesAtRevision(typeof(Address), 6)
                                              .Add(AuditEntity.Property("ZipCode").HasNotChanged());

                IList entity2 = query2.GetResultList();

                // search only in REV 6
                var query3 = auditReader.CreateQuery().ForEntitiesModifiedAtRevision(typeof(Address), 6)
                                              .Add(AuditEntity.Property("ZipCode").HasNotChanged());

                IList entity3 = query3.GetResultList();

                tx.Commit();
            }

            #endregion
        }

        private static void ExampleListener()
        {
            var cfg = new Configuration().Configure();

            var mapper = new ConventionModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(mapping);

            var advListener = new AdvancedListener();
            advListener.Register(cfg);

            // SimpleListener has flaws see omments inside Class
            //cfg.SetListener(ListenerType.PreInsert, new SimpleListener());
            //cfg.SetListener(ListenerType.PreUpdate, new SimpleListener());

            var sf = cfg.BuildSessionFactory();

            DropAllForeignKeysFromDatabase(cfg, sf, DROP_GENERATED_FK_AND_RI);

            #region Insert-Test-Data

            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var parent = new Account
                {
                    FirstName = "Alan",
                    LastName = "Turing"
                };

                var address1 = new Address
                {
                    Account = parent,
                    Street = "Adlington Road",
                    HouseNumber = "42",
                    ZipCode = "SK9",
                    City = "Wilmslow"
                };

                parent.Address.Add(address1);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            #endregion

            #region Update-Test-Data

            // rename existing Account
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                account.FirstName = "Alan Mathison";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // insert new entry into existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                var address1 = new Address
                {
                    Account = account,
                    Street = "Bletchley Park",
                    HouseNumber = "",
                    ZipCode = "MK3",
                    City = "Milton Keynes"
                };

                account.Address.Add(address1);

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // rename entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                Address address = account.Address.Single(c => c.Street.Contains("Bletchley"));

                address.HouseNumber = "HUT 8";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // delete entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                Address address = account.Address.Single(c => c.Street.Contains("Adlington"));

                account.Address.Remove(address);

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // delete Account
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(1L);

                session.Delete(account);

                tx.Commit();
            }

            #endregion
        }

        private static void ExampleInterceptor()
        {
            var cfg = new Configuration().Configure();

            var mapper = new ConventionModelMapper();

            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(mapping);

            var sf = cfg.BuildSessionFactory();

            DropAllForeignKeysFromDatabase(cfg, sf, DROP_GENERATED_FK_AND_RI);

            #region Insert-Test-Data

            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var parent = new Account
                {
                    FirstName = "Alan",
                    LastName = "Turing"
                };

                var address1 = new Address
                {
                    Account = parent,
                    Street = "Adlington Road",
                    HouseNumber = "42",
                    ZipCode = "SK9",
                    City = "Wilmslow"
                };

                parent.Address.Add(address1);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            #endregion

            #region Update-Test-Data

            // rename existing Account
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                account.FirstName = "Alan Mathison";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // insert new entry into existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                var address1 = new Address
                {
                    Account = account,
                    Street = "Bletchley Park",
                    HouseNumber = "",
                    ZipCode = "MK3",
                    City = "Milton Keynes"
                };

                account.Address.Add(address1);

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // rename entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                Address address = account.Address.Single(c => c.Street.Contains("Bletchley"));

                address.HouseNumber = "HUT 8";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // delete entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                Address address = account.Address.Single(c => c.Street.Contains("Adlington"));

                account.Address.Remove(address);

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // delete Account
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                session.Delete(account);

                tx.Commit();
            }

            #endregion
        }

        private static void ExampleILifecycle()
        {
            var cfg = new Configuration().Configure();

            var mapper = new ConventionModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(mapping);

            var sf = cfg.BuildSessionFactory();

            DropAllForeignKeysFromDatabase(cfg, sf, DROP_GENERATED_FK_AND_RI);

            #region Insert-Test-Data

            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var parent = new Account
                {
                    FirstName = "Alan",
                    LastName = "Turing"
                };

                var address1 = new Address
                {
                    Account = parent,
                    Street = "Adlington Road",
                    HouseNumber = "42",
                    ZipCode = "SK9",
                    City = "Wilmslow"
                };

                parent.Address.Add(address1);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            #endregion

            #region Update-Test-Data

            // rename existing Account
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                account.FirstName = "Alan Mathison";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // insert new entry into existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                var address1 = new Address
                {
                    Account = account,
                    Street = "Bletchley Park",
                    HouseNumber = "",
                    ZipCode = "MK3",
                    City = "Milton Keynes"
                };

                account.Address.Add(address1);

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // rename entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                Address address = account.Address.Single(c => c.Street.Contains("Bletchley"));

                address.HouseNumber = "HUT 8";

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // delete entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                Address address = account.Address.Single(c => c.Street.Contains("Adlington"));

                account.Address.Remove(address);

                session.SaveOrUpdate(account);

                tx.Commit();
            }

            // delete Account
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Account account = session.Get<Account>(3L);

                session.Delete(account);

                tx.Commit();
            }

            #endregion
        }

        // Source by Robin van der Knaap : http://stackoverflow.com/a/6536366/3722909
        private static void DropAllForeignKeysFromDatabase(Configuration cfg, ISessionFactory sf, bool dropFkAndRI)
        {
            if (dropFkAndRI)
            {
                var tableNamesFromMappings = cfg.ClassMappings.Select(x => x.Table.Name);

                var dropAllForeignKeysSql =
                    @"  DECLARE @cmd nvarchar(1000)
                      DECLARE @fk_table_name nvarchar(1000)
                      DECLARE @fk_name nvarchar(1000)

                      DECLARE cursor_fkeys CURSOR FOR
                      SELECT  OBJECT_NAME(fk.parent_object_id) AS fk_table_name,
                              fk.name as fk_name
                      FROM    sys.foreign_keys fk  JOIN
                              sys.tables tbl ON tbl.OBJECT_ID = fk.referenced_object_id
                      WHERE OBJECT_NAME(fk.parent_object_id) in ('" + String.Join("','", tableNamesFromMappings) + @"')

                      OPEN cursor_fkeys
                      FETCH NEXT FROM cursor_fkeys
                      INTO @fk_table_name, @fk_name

                      WHILE @@FETCH_STATUS=0
                      BEGIN
                        SET @cmd = 'ALTER TABLE [' + @fk_table_name + '] DROP CONSTRAINT [' + @fk_name + ']'
                        exec dbo.sp_executesql @cmd

                        FETCH NEXT FROM cursor_fkeys
                        INTO @fk_table_name, @fk_name
                      END
                      CLOSE cursor_fkeys
                      DEALLOCATE cursor_fkeys
                    ;";

                using (var connection = sf.OpenSession().Connection)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = dropAllForeignKeysSql;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}