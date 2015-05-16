using nHibernate4.Model.Envers;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace nHibernate4.Mapping.Envers
{
    public class MapEnversRevInfo : ClassMapping<EnversRevInfo>
    {
        public MapEnversRevInfo()
        {
            Table("REVINFO");

            Id(x => x.Id, m =>
            {
                m.Generator(Generators.EnhancedSequence, g => g.Params(new
                {
                    sequence_name = "enhanced_sequence_envers",
                    optimizer = "pooled",
                    increment_size = 20
                }));
            });

            Property(x => x.RevisionTimestamp);
            Property(x => x.UserName);

            Set(x => x.ChangedObjects,
                m =>
                {
                    m.Table("REVINFO_DETAIL");
                    m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                },
                rel =>
                {   
                    rel.Element();
                });
        }
    }
}
