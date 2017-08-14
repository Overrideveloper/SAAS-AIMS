using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.Enums.Enums.Semester
{
    public enum Semester
    {
        [Description("First Semester")]
        [Display( Name = "First Semester")]
        First,

        [Description("Second Semester")]
        [Display( Name = "Second Semester")]
        Second
    }
}
