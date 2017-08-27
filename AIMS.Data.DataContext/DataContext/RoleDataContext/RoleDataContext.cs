using AIMS.Data.DataObjects.Entities.Role;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.RoleDataContext
{
    public class RoleDataContext : DbContext
    {
        public RoleDataContext() :
            base("name = Aims")
        {
        }
        public virtual DbSet<Role> Roles { get; set; }

    }
}
