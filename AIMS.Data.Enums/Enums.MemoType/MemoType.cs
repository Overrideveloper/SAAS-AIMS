using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.Enums.Enums.MemoType
{
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter.PascalCaseWordSplittingEnumConverter))]
    public enum MemoType
    {
        Request,
        Confirmation,
        PeriodicReport,
        IdeasAndSuggestions,
        InformalStudyResults
    }
}
