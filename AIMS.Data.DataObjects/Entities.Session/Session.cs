using AIMS.Data.DataObjects.Entities.SystemManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Session
{
    public class Session : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Session title is required")]
        [DisplayName("Session Title")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayName("End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime EndDate { get; set; }

        public virtual ICollection<Meeting.Meeting> Meeting { get; set; }
    }
}
