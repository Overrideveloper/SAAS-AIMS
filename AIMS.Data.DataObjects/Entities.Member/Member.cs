using AIMS.Data.DataObjects.Entities.SystemManagement;
using AIMS.Data.Enums.Enums.Gender;
using AIMS.Data.Enums.Enums.Level;
using AIMS.Data.Enums.Enums.State;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AIMS.Data.DataObjects.Entities.Member
{
    public class Member : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        public string MidName { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Matric. number is required")]
        [StringLength(20, MinimumLength = 17)]
        [Index(IsUnique = true)]
        [Remote("IsMatricNoAvailable", "Member", ErrorMessage = "Matric. number is already in use")]
        public string MatricNumber { get; set; }

        [Required(ErrorMessage = "State of origin is required")]
        public State StateOfOrigin { get; set; }

        [Required(ErrorMessage = "Year of Admission is required")]
        public DateTime YearOfAdmission { get; set; }

        [Required(ErrorMessage = "Level of Admission is required")]
        public Level LevelOfAdmission { get; set; }

        public virtual ICollection<Dues.Dues> Dues { get; set; }
    }
}
