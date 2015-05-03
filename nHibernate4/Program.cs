using System.Reflection;
using log4net;
using log4net.Config;
using nHibernate4.Model;
using nHibernate4.Model.LifecycleExample;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace nHibernate4
{
    internal class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var cfg = new Configuration().Configure();

            cfg.Interceptor = new SimpleInterceptor();

            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            cfg.AddMapping(mapping);

            var sf = cfg.BuildSessionFactory();

            #region

            GetValue(sf);

            GetValue2(sf);

            using (var session = sf.OpenSession(new SimpleInterceptor()))
            using (var tx = session.BeginTransaction())
            {
                long id = 4;
                var test = session.Load<Parent>(id);

                test.Name = "TEST";

                session.SaveOrUpdate(test);

                tx.Commit();
            }



            #endregion
        }

        private static void GetValue(ISessionFactory sf)
        {
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                for (var i = 0; i < 100; i++)
                {
                    var parent = new Parent
                    {
                        Name = "PARENT#" + i
                    };

                    var child = new Child
                    {
                        Parent = parent,
                        Name = "CHILDFORPARENT#" + i
                    };

                    parent.Children.Add(child);

                    session.SaveOrUpdate(parent);

                    using (var session2 = sf.OpenSession())
                    using (var tx2 = session2.BeginTransaction())
                    {
                        var parent2 = new Parent
                        {
                            Name = "PARENT#" + i
                        };

                        session2.SaveOrUpdate(parent2);

                        tx2.Commit();
                    }
                }

                tx.Commit();
            }
        }


        private static void GetValue2(ISessionFactory sf)
        {
            using (var session = sf.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                for (var i = 0; i < 100; i++)
                {
                    var parent = new ParentILifecycle
                    {
                        Name = "PARENT#" + i
                    };

                    var child = new ChildILifecycle()
                    {
                        Parent = parent,
                        Name = "CHILDFORPARENT#" + i
                    };

                    parent.Children.Add(child);

                    session.SaveOrUpdate(parent);

                    using (var session2 = sf.OpenSession())
                    using (var tx2 = session2.BeginTransaction())
                    {
                        var parent2 = new Parent
                        {
                            Name = "PARENT#" + i
                        };

                        session2.SaveOrUpdate(parent2);

                        tx2.Commit();
                    }
                }

                tx.Commit();
            }
        }
    }
}