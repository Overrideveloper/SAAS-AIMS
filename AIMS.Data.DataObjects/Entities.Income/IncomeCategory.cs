using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Income
{
    public class IncomeCategory
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        public string Title { get; set; }

        public virtual ICollection<IncomeItem> IncomeItem { get; set; }
    }
}
