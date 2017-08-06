using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.Enums.Enums.Level
{
    public enum Level
    {
        [Display(Name = "100 Level")]
        One,
        [Display(Name = "200 Level")]
        Two,
        [Display(Name = "300 Level")]
        Three,
        [Display(Name = "400 Level")]
        Four
    }
}
