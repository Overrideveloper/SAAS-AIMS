using AIMS.Data.DataObjects.Entities.SystemManagement;
using AIMS.Data.Enums.Enums.MemoType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Data.DataObjects.Entities.Memo
{
    public class Memo : Transport
    {
        [Key]
        public long ID { get; set; }

        [Required(ErrorMessage = "Select a type!")]
        public MemoType Type { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Upload a memo!")]
        public string FileUpload { get; set; }

        public long SessionID { get; set; }

        [ForeignKey("SessionID")]
        public virtual Session.Session Session { get; set; }
    }
}
