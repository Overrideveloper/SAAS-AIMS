using AIMS.Data.DataObjects.Entities.Meeting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.MeetingDataContext
{
    public class MeetingDataContext : DbContext
    {
        public MeetingDataContext() :
            base("Aims")
        {
        }

        public DbSet<Meeting> Meetings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<AIMS.Data.DataObjects.Entities.Session.Session> Sessions { get; set; }
    }
}
