using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services.UsernameGenerator
{
    public class UsernameGenerator
	{
        public string GenerateUserName(string email)
        {
            return email.Replace("@", "").Replace(".", "").Replace("-", "");
        }
    }
}
