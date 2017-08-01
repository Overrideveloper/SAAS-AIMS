using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.SystemManagement
{
    public class Transport
    {
        [DisplayName("Created By")]
        public virtual long CreatedBy { get; set; }

        [DisplayName("Date Created")]
        public virtual DateTime DateCreated { get; set; }

        [DisplayName("Date Last Modified")]
        public virtual DateTime DateLastModified { get; set; }

        [DisplayName("Last Modified By")]
        public virtual long LastModifiedBy { get; set; }
    }
}
