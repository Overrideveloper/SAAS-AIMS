using Microsoft.AspNet.Identity.EntityFramework;
using SAAS_AIMS.Enums;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace SAAS_AIMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string AssociationName { get; set; }
        public string CollegeChapter { get; set; }
        public virtual State State { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }

    public class AppUserDataContext : IdentityDbContext<ApplicationUser>
    {
        public AppUserDataContext()
            : base("Aims")
        {
        }

    }
}