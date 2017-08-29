using AIMS.Data.DataObjects.Entities.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Income
{
    public class IncomeCategory : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        public string Title { get; set; }

        public long SessionID { get; set; }

        [ForeignKey("SessionID")]
        public virtual Session.Session Session { get; set; }
        
        public virtual ICollection<IncomeItem> IncomeItem { get; set; }
    }
}
