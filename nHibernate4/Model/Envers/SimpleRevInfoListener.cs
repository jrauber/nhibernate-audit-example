using System.Security.Principal;
using NHibernate.Envers;

namespace nHibernate4.Model.Envers
{
    class SimpleRevInfoListener : IRevisionListener
    {
        public void NewRevision(object revisionEntity)
        {
            EnversRevInfo revInfo = revisionEntity as EnversRevInfo;

            if (revInfo != null)
            {
                revInfo.UserName = WindowsIdentity.GetCurrent().Name;
            }
        }
    }
}
