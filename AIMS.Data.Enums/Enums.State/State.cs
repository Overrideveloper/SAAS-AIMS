using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.Enums.Enums.State
{
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter.PascalCaseWordSplittingEnumConverter))]
    public enum State
    {
        Abia,
        Adamawa,
        Anambra,
        AkwaIbom,
        Bauchi,
        Bayelsa,
        Benue,
        Borno,
        CrossRiver,
        Delta,
        Ebonyi,
        Enugu,
        Edo,
        Ekiti,
        Gombe,
        Imo,
        Jigawa,
        Kaduna,
        Kano,
        Katsina,
        Kebbi,
        Kogi,
        Kwara,
        Lagos,
        Nasarawa,
        Niger,
        Ogun,
        Ondo,
        Osun,
        Oyo,
        Plateau,
        Rivers,
        Sokoto,
        Taraba,
        Yobe,
        Zamfara,
        Abuja
    }
}
