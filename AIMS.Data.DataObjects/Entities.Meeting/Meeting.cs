using AIMS.Data.DataObjects.Entities.SystemManagement;
using AIMS.Data.Enums.Enums.Semester;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Meeting
{
    public class Meeting : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public string Venue { get; set; }

        [Required(ErrorMessage = "Semester is required")]
        public Semester Semester { get; set; }

        [Display(Name = "Upload minutes of meeting")]
        public string FileUpload { get; set; }

        [Required]
        public long SessionID { get; set; }

        [ForeignKey("SessionID")]
        public virtual Session.Session Session { get; set; }
    }
}
