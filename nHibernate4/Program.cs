using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using nHibernate4.Model;
using nHibernate4.Model.Interceptor;
using nHibernate4.Model.LifecycleExample;
using nHibernate4.Model.Listener;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Event;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;

namespace nHibernate4
{
    internal class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            //ExampleInterceptor();

            //ExampleListener();

            ExampleEnvers();
        }

        private static void ExampleEnvers()
        {
            var cfg = new Configuration().Configure();

            var mapper = new ModelMapper();
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

            #region Insert-Test-Data

            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                for (var i = 0; i < 5; i++)
                {
                    var parent = new Parent
                    {
                        Name = "PARENT#" + i
                    };

                    var child1 = new Child
                    {
                        Parent = parent,
                        Name = "PARENT#" + i + "CHILD#1"
                    };

                    var child2 = new Child
                    {
                        Parent = parent,
                        Name = "PARENT#" + i + "CHILD#2"
                    };

                    parent.Children.Add(child1);
                    parent.Children.Add(child2);

                    session.SaveOrUpdate(parent);
                }

                tx.Commit();
            }

            #endregion

            #region Update-Test-Data

            // rename existing parent
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                parent.Name += "UPDATE";

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // insert new entry into existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                var child = new Child
                {
                    Parent = parent,
                    Name = parent.Name + "CHILD#3"
                };

                parent.Children.Add(child);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // rename entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                Child child = parent.Children.Single(c => c.Name.Contains("CHILD#3"));

                child.Name += "UPDATE";

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // delete entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                Child child = parent.Children.Single(c => c.Name.Contains("CHILD#3"));

                parent.Children.Remove(child);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // delete parent
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                session.Delete(parent);

                tx.Commit();
            }

            #endregion
        }

        private static void ExampleListener()
        {
            var cfg = new Configuration().Configure();

            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(mapping);

            cfg.SetListener(ListenerType.PreInsert, new SimpleListener());
            cfg.SetListener(ListenerType.PreUpdate, new SimpleListener());
            cfg.SetListener(ListenerType.PreDelete, new SimpleListener());
            cfg.SetListener(ListenerType.PreCollectionUpdate, new SimpleListener());
            cfg.SetListener(ListenerType.PostCollectionUpdate, new SimpleListener());
            cfg.SetListener(ListenerType.PreCollectionRemove, new SimpleListener());

            var sf = cfg.BuildSessionFactory();

            #region Insert-Test-Data

            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                for (var i = 0; i < 5; i++)
                {
                    var parent = new Parent
                    {
                        Name = "PARENT#" + i
                    };

                    var child1 = new Child
                    {
                        Parent = parent,
                        Name = "PARENT#" + i + "CHILD#1"
                    };

                    var child2 = new Child
                    {
                        Parent = parent,
                        Name = "PARENT#" + i + "CHILD#2"
                    };

                    parent.Children.Add(child1);
                    parent.Children.Add(child2);

                    session.SaveOrUpdate(parent);
                }

                tx.Commit();
            }

            #endregion

            #region Update-Test-Data

            // rename existing parent
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                parent.Name += "UPDATE";

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // insert new entry into existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                var child = new Child
                {
                    Parent = parent,
                    Name = parent.Name + "CHILD#3"
                };

                parent.Children.Add(child);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // rename entry in existing collection
            using (var session = sf.OpenSession(new SimpleInterceptor()))
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                Child child = parent.Children.Single(c => c.Name.Contains("CHILD#3"));

                child.Name += "UPDATE";

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // delete entry in existing collection
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                Child child = parent.Children.Single(c => c.Name.Contains("CHILD#3"));

                parent.Children.Remove(child);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // delete parent
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                session.Delete(parent);

                tx.Commit();
            }

            #endregion
        }

        private static void ExampleInterceptor()
        {
            var cfg = new Configuration().Configure();

            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(mapping);

            var sf = cfg.BuildSessionFactory();

            #region Insert-Test-Data

            using (var session = sf.OpenSession(new SimpleInterceptor()))
            using (var tx = session.BeginTransaction())
            {
                for (var i = 0; i < 5; i++)
                {
                    var parent = new Parent
                    {
                        Name = "PARENT#" + i
                    };

                    var child1 = new Child
                    {
                        Parent = parent,
                        Name = "PARENT#" + i + "CHILD#1"
                    };

                    var child2 = new Child
                    {
                        Parent = parent,
                        Name = "PARENT#" + i + "CHILD#2"
                    };

                    parent.Children.Add(child1);
                    parent.Children.Add(child2);

                    session.SaveOrUpdate(parent);
                }

                tx.Commit();
            }

            #endregion

            #region Update-Test-Data

            // rename existing parent
            using (var session = sf.OpenSession(new SimpleInterceptor()))
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                parent.Name += "UPDATE";

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // insert new entry into existing collection
            using (var session = sf.OpenSession(new SimpleInterceptor()))
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                 var child = new Child
                {
                    Parent = parent,
                    Name = parent.Name + "CHILD#3"
                };

                parent.Children.Add(child);

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            // rename entry in existing collection
            using (var session = sf.OpenSession(new SimpleInterceptor()))
            using (var tx = session.BeginTransaction())
            {
                Parent parent = session.Get<Parent>(3L);

                Child child = parent.Children.Single(c => c.Name.Contains("CHILD#3"));

                child.Name += "UPDATE";

                session.SaveOrUpdate(parent);

                tx.Commit();
            }

            #endregion
        }


        private static void ExampleILifecycle()
        {
            var cfg = new Configuration().Configure();

            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(mapping);

            var sf = cfg.BuildSessionFactory();

            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                for (var i = 0; i < 100; i++)
                {
                    var parent = new ParentILifecycle
                    {
                        Name = "PARENT#" + i
                    };

                    var child1 = new ChildILifecycle()
                    {
                        Parent = parent,
                        Name = "PARENT#" + i + "CHILD#1"
                    };

                    var child2 = new ChildILifecycle()
                    {
                        Parent = parent,
                        Name = "PARENT#" + i + "CHILD#2"
                    };

                    parent.Children.Add(child1);
                    parent.Children.Add(child2);

                    session.SaveOrUpdate(parent);
                }

                tx.Commit();
            }
        }
    }
}