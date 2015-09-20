using System;
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
                revInfo.UserName = GetCurrentUserName();
            }
        }

        private string GetCurrentUserName()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();

            if (windowsIdentity != null)
            {
                return windowsIdentity.Name;
            }

            return String.Empty;
        }
    }
}
