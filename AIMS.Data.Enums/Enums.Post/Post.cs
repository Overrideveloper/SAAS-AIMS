using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.Enums.Enums.Post
{
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter.PascalCaseWordSplittingEnumConverter))]
    public enum Post
    {
        President,
        VicePresident,
        SecretaryGeneral,
        DirectorOfFinance,
        DirectorOfSoftware,
        DirectorOfSocials,
        DirectorOfSports,
        AssistantSecretaryGeneral,
        PublicRelationsOfficerI,
        PublicRelationsOfficerII,
        DirectorOfWelfareI,
        DirectorOfWelfareII,
        Provost,
        Treasurer
    }
}
