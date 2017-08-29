using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Income
{
    public class IncomeItem
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Amount is required!")]
        public decimal Amount { get; set; }

        [Required]
        public long IncomeCategoryID { get; set; }

        [ForeignKey("IncomeCategoryID")]
        public virtual IncomeCategory IncomeCategory { get; set; }

    }
}
