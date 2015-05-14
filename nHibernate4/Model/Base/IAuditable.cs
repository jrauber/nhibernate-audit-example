using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nHibernate4.Model.Base
{
    interface IAuditable
    {
        DateTime? CreatedOn { get; set; }
        string CreatedBy { get; set; }

        DateTime? ChangedOn { get; set; }
        string ChangedBy { get; set; }
    }
}
