using nHibernate4.Model.Base;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace nHibernate4.Mapping.Base
{
    public class MapBaseEnhSeqAudit<T> : ClassMapping<T> where T : ModelBase
    {
        public MapBaseEnhSeqAudit()
        {
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.EnhancedSequence, g => g.Params(new
                {
                    sequence_name = "enhanced_sequence",
                    optimizer = "pooled",
                    increment_size = 20
                }));
            });

            //Audit Properties
            Property(x => x.ChangedBy);
            Property(x => x.ChangedOn);
            Property(x => x.CreatedBy);
            Property(x => x.CreatedOn);

            Version(x => x.Version, m =>
            {
                m.Generated(VersionGeneration.Never);
                m.UnsavedValue(0);
            });
        }
    }
}