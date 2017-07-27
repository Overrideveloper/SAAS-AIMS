using AIMS.Data.DataObjects.Entities.UserAccount;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.AppUserContext
{
    public class AppUserContext : IdentityDbContext<AppUser>
    {
        public AppUserContext()
            : base("Aims")
        {
        }
    }
}
