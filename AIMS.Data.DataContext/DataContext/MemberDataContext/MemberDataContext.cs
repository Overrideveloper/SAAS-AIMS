using AIMS.Data.DataObjects.Entities.Member;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.MemberDataContext
{
    public class MemberDataContext : DbContext
    {
        public MemberDataContext()
            : base("name = Aims")
        {
        }

        public virtual DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
