using AIMS.Data.DataObjects.Entities.SystemManagement;
using AIMS.Data.Enums.Enums.Post;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Exco
{
    public class Exco : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Enter matric number!")]
        public string MatricNo { get; set; }

        [Required(ErrorMessage = "Enter last name!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter first name!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Select post!")]
        public Post Post { get; set; }

        [Required]
        public long SessionID { get; set; }

        [ForeignKey("SessionID")]
        public Session.Session Session { get; set; }
    }
}
