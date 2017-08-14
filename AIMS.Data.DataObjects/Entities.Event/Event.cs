using AIMS.Data.DataObjects.Entities.SystemManagement;
using AIMS.Data.Enums.Enums.Semester;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Event
{
    public class Event : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime Date { get; set; }

        [Required]
        public Semester Semester { get; set; }

        [Required]
        public string Venue { get; set; }

        [Required]
        public long SessionID { get; set; }

        [ForeignKey("SessionID")]
        public virtual Session.Session Session { get; set; }

    }
}
