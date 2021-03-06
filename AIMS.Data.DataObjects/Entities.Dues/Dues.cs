﻿using AIMS.Data.Enums.Enums.Level;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using AIMS.Data.DataObjects.Entities.SystemManagement;

namespace AIMS.Data.DataObjects.Entities.Dues
{
    public class Dues : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Select A Level")]
        public Level Level { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }

        public long MemberID { get; set; }

        [ForeignKey("MemberID")]
        public virtual Member.Member Member { get; set; }
    }
}
