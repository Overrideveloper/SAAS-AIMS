using AIMS.Data.DataObjects.Entities.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Expense
{
    public class ExpenseItem : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public long ExpenseCategoryID { get; set; }

        [ForeignKey("ExpenseCategoryID")]
        public virtual ExpenseCategory ExpenseCategory { get; set; }
    }
}
