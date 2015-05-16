using nHibernate4.Model.Base;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace nHibernate4.Mapping.Base
{
    public class MapBaseEnhSeqAudit<T> : MapBaseEnhSeq<T> where T : ModelBaseAudit
    {
        public MapBaseEnhSeqAudit()
        {
            //Audit Properties
            Property(x => x.ChangedBy);
            Property(x => x.ChangedOn);
            Property(x => x.CreatedBy);
            Property(x => x.CreatedOn);
        }
    }
}