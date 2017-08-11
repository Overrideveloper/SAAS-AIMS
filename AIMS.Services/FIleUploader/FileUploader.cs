using AIMS.Data.Enums.Enums.UploadType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AIMS.Services.FIleUploader
{
    public class FileUploader
    {
        public string UploadFile(HttpPostedFileBase fileUpload, UploadType uploadType)
        {
            var fileName = DateTime.Now.ToFileTime().ToString();

            if (fileUpload != null)
            {
                var fileInfo = new FileInfo(fileUpload.FileName);
                if ((fileInfo.Extension.ToLower() == ".jpg") || (fileInfo.Extension.ToLower() == ".jpeg") || (fileInfo.Extension.ToLower() == ".gif") 
                    || (fileInfo.Extension.ToLower() == ".png") || (fileInfo.Extension.ToLower() == ".pdf") || (fileInfo.Extension.ToLower() == ".docx") 
                    || (fileInfo.Extension.ToLower() == ".txt") || (fileInfo.Extension.ToLower() == ".doc") || (fileInfo.Extension.ToLower() == ".rtf"))
                {
                    try
                    {
                        var fileExtension = fileInfo.Extension;

                        fileName = DateTime.Now.ToFileTime() + fileExtension;

                        //Create upload folder if not created
                        var uploadFolderPath = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + uploadType);

                        //Create directory if not existing
                        if (!Directory.Exists(uploadFolderPath))
                            Directory.CreateDirectory(uploadFolderPath);

                        //Save file
                        var filePath = uploadFolderPath + "/" + fileName;
                        fileUpload.SaveAs(filePath);
                    }
                    catch (Exception ex)
                    {

                        ex.ToString();
                    }
                }
            }
            return fileName;
        }

        public bool ThumbnailCallback()
        {
            return false;
        }
    }
}
