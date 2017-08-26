using AIMS.Data.DataObjects.Entities.Project;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataContext.DataContext.ProjectDataContext
{
    public class ProjectDataContext : DbContext
    {
        public ProjectDataContext()
            : base("name = Aims")
        {
        }

        public virtual DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
