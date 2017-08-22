using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataObjects.Entities.Member;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Data.Enums.Enums.State;
using AIMS.Data.Enums.Enums.UploadType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AIMS.Data.Enums.Enums.Gender;
using AIMS.Data.Enums.Enums.Level;

namespace SAAS_AIMS.Controllers
{
    public class MemberController : BaseController
    {
        
        private readonly MemberDataContext _memberDataContext;
        public static long userID;

        #region constructor
        public MemberController()
        {
            _memberDataContext = new MemberDataContext();
            userID = 0;
        }
        #endregion

        #region convert csv to datatable
        private static DataTable ConvertCSV(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("Surname", typeof(System.String)));
            dt.Columns.Add(new DataColumn("MidName", typeof(System.String)));
            dt.Columns.Add(new DataColumn("FirstName", typeof(System.String)));
            dt.Columns.Add(new DataColumn("MatricNumber", typeof(System.String)));
            dt.Columns.Add(new DataColumn("StateOfOrigin", typeof(Enums.State)));
            dt.Columns.Add(new DataColumn("YearOfAdmission", typeof(System.DateTime)));
            dt.Columns.Add(new DataColumn("LevelOfAdmission", typeof(Level)));
            dt.Columns.Add(new DataColumn("Gender", typeof(Gender)));
            dt.Columns.Add(new DataColumn("CreatedBy", typeof(System.Int64)));
            dt.Columns.Add(new DataColumn("DateCreated", typeof(System.DateTime)));
            dt.Columns.Add(new DataColumn("LastModifiedBy", typeof(System.Int64)));
            dt.Columns.Add(new DataColumn("DateLastModified", typeof(System.DateTime)));
            
            DataRow dr;
            string s;
            int j = 0;

            while (!sr.EndOfStream)
            {
                while ((s = sr.ReadLine()) != null)
                {
                    if (j > 0)
                    {
                        string[] str = s.Split(',');

                        dr = dt.NewRow();

                        dr["Surname"] = str[1].ToString();
                        dr["MidName"] = str[2].ToString();
                        dr["FirstName"] = str[3].ToString();
                        dr["MatricNumber"] = str[4].ToString();
                        dr["StateOfOrigin"] = str[5].ToString();
                        dr["YearOfAdmission"] = str[6].ToString();
                        dr["LevelOfAdmission"] = str[7].ToString();
                        dr["Gender"] = str[8].ToString();
                        dr["CreatedBy"] = userID.ToString();
                        dr["DateCreated"] = DateTime.Now.ToString();
                        dr["LastModifiedBy"] = userID.ToString();
                        dr["DateLastModified"] = DateTime.Now.ToString();

                        dt.Rows.Add(dr);
                    }
                    j++;
                }
            }
            sr.Dispose();
            sr.Close();
            return dt;
        }
        #endregion

        #region copy csv upload to database
        private static String CopyToDB(DataTable dt)
        {
            string feedback = string.Empty;

            string connect = ConfigurationManager.ConnectionStrings["Aims"].ConnectionString;

            string mysqlcmd = "INSERT_MEMBER_DATA";

            MySqlConnection conn = new MySqlConnection(connect);
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(mysqlcmd, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.UpdatedRowSource = UpdateRowSource.None;

                cmd.Parameters.Add("?Surname", MySqlDbType.String).SourceColumn = "Surname";
                cmd.Parameters.Add("?MidName", MySqlDbType.String).SourceColumn = "MidName";
                cmd.Parameters.Add("?FirstName", MySqlDbType.String).SourceColumn = "FirstName";
                cmd.Parameters.Add("?MatricNumber", MySqlDbType.String).SourceColumn = "MatricNumber";
                cmd.Parameters.Add("?StateOfOrigin", MySqlDbType.Int32).SourceColumn = "StateOfOrigin";
                cmd.Parameters.Add("?YearOfAdmission", MySqlDbType.DateTime).SourceColumn = "YearOfAdmission";
                cmd.Parameters.Add("?LevelOfAdmission", MySqlDbType.Int32).SourceColumn = "LevelOfAdmission";
                cmd.Parameters.Add("?Gender", MySqlDbType.Int32).SourceColumn = "Gender";
                cmd.Parameters.Add("?CreatedBy", MySqlDbType.Int64).SourceColumn = "CreatedBy";
                cmd.Parameters.Add("?DateCreated", MySqlDbType.DateTime).SourceColumn = "DateCreated";
                cmd.Parameters.Add("?LastModifiedBy", MySqlDbType.Int64).SourceColumn = "LastModifiedBy";
                cmd.Parameters.Add("?DateLastModified", MySqlDbType.DateTime).SourceColumn = "DateLastModified";

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.InsertCommand = cmd;
                da.UpdateBatchSize = 100;
                int records = da.Update(dt);
                conn.Close();

                feedback = "Succesfully uploaded members' data!";
            }
            catch (Exception ex)
            {
                feedback = "Failed to upload members data: " + ex.Message + ". Contact System Administrator!";
            }
            
            return feedback;
        }
        #endregion

        #region association member index
        //
        // GET: /Member/
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var member = from m in _memberDataContext.Members
                             select m;
            return View(member.OrderBy(order => order.Surname));
        }
        #endregion

        #region matric number remote validation
        [Authorize]
        public JsonResult IsMatricNoAvailable(string MatricNumber) {
            return Json(!_memberDataContext.Members.Any(member => member.MatricNumber == MatricNumber), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region create association member
        // GET: /Member/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var member = new Member();
            return PartialView("Create", member);
        }

        // POST: /Member/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member member)
        {
            if (member.MatricNumber == null)
            {
                ModelState.AddModelError("empty", "Matric number is required");
            }
            if (ModelState.IsValid)
            {
                if (_memberDataContext.Members.Any(record => record.MatricNumber == member.MatricNumber))
                {
                    ModelState.AddModelError("Matric", "Matric. number is already is use");
                }
                else
                {
                    var membervar = new Member
                    {
                        FirstName = member.FirstName,
                        Gender = member.Gender,
                        LevelOfAdmission = member.LevelOfAdmission,
                        MatricNumber = member.MatricNumber,
                        MidName = member.MidName,
                        StateOfOrigin = member.StateOfOrigin,
                        Surname = member.Surname,
                        YearOfAdmission = member.YearOfAdmission,

                        CreatedBy = Convert.ToInt64(Session["UserID"]),
                        DateCreated = DateTime.Now,
                        DateLastModified = DateTime.Now,
                        LastModifiedBy = Convert.ToInt64(Session["UserID"])
                    };

                    _memberDataContext.Members.Add(membervar);
                    _memberDataContext.SaveChanges();
                    TempData["Success"] = "Association member successfully created! ";
                    TempData["NotificationType"] = NotificationType.Create.ToString();
                    return Json(new { success = true });
                }
            }
            return PartialView("Create", member);
        }
        #endregion

        #region upload member data via csv
        //
        // GET: /Member/Upload
        [HttpGet]
        [Authorize]
        public ActionResult Upload(bool upload)
        {
            return View();
        }

        //
        // POST: /Member/Upload
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            DataTable dt = new DataTable();

            if (file != null)
            {
                var info = new FileInfo(file.FileName);
                if ((info.Extension.ToLower() == ".csv") || (info.Extension.ToLower() == ".xlsx"))
                {
                    try
                    {
                        var extension = info.Extension;
                        string fileName = DateTime.Now.ToFileTime() + extension;

                        //Create upload folder if not created
                        var folderPath = HttpContext.Server.MapPath("~/UploadedFiles/" + UploadType.MemberData);

                        //Create directory if not existing
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        //Save file
                        var filePath = folderPath + "/" + fileName;
                        file.SaveAs(filePath);

                        dt = ConvertCSV(filePath);

                        TempData["NotificationType"] = NotificationType.Upload.ToString();
                        TempData["Type"] = "Success";
                        TempData["Response"] = CopyToDB(dt);
                    }
                    catch (Exception ex)
                    {
                        TempData["NotificationType"] = NotificationType.Upload.ToString();
                        TempData["Type"] = "Error";
                        TempData["Response"] = "Error uploading member details: " + ex.Message + ". Contact System Administrator!";
                    }
                }
                else
                {
                    TempData["NotificationType"] = NotificationType.Upload.ToString();
                    TempData["Variant"] = "Error";
                    TempData["Error"] = "Error! Only .CSV and .XLSX files are can be uploaded!";
                }
            }
            else
            {
                TempData["NotificationType"] = NotificationType.Upload.ToString();
                TempData["Variant"] = "Error";
                TempData["Error"] = "Error! Select a file for upload!";
            }
            dt.Dispose();
            return RedirectToAction("Index");
        }
        #endregion

        #region edit association member
        //
        // GET: /Member/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            Session["id"] = id;
            var member = await _memberDataContext.Members.FindAsync(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            Session["matric"] = member.MatricNumber;
            return PartialView("Edit", member);
        }

        //
        // POST: /Member/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Member member)
        {
            if (ModelState.IsValid)
            {
                    member.DateLastModified = DateTime.Now;
                    member.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                    _memberDataContext.Entry(member).State = EntityState.Modified;
                    _memberDataContext.SaveChanges();

                    TempData["Success"] = "Association member successfully modified! ";
                    TempData["NotificationType"] = NotificationType.Create.ToString();
                    return Json(new { success = true });
            }
            return PartialView("Edit", member);
        }

        #endregion

        #region delete association member
        //
        // DELETE: /Member/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var member = await _memberDataContext.Members.FindAsync(id);
            _memberDataContext.Members.Remove(member);
            await _memberDataContext.SaveChangesAsync();

            TempData["Success"] = "Association member successfully deleted!";
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index");
        }
        #endregion
    }
}