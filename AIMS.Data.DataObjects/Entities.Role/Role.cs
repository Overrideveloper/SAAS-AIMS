﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Role
{
    public class Role
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        public bool CanManageSessions { get; set; }

        public bool CanManageMembers { get; set; }

        public bool CanManageEvents { get; set; }

        public bool CanManageExecutives { get; set; }

        public bool CanManageExpenses { get; set; }

        public bool CanManageIncome { get; set; }

        public bool CanManageMeetings { get; set; }
    }
}
