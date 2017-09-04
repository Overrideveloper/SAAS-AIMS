using AIMS.Data.DataContext.DataContext.EventDataContext;
using AIMS.Data.DataContext.DataContext.ExpenseDataContext;
using AIMS.Data.DataContext.DataContext.IncomeDataContext;
using AIMS.Data.DataContext.DataContext.MeetingDataContext;
using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataContext.DataContext.ProjectDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.ViewModels.ViewModel.IncExp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class SessionDetailsController : BaseController
    {
        private readonly EventDataContext _eventDataContext;
        private readonly MeetingDataContext _meetingDataContext;
        private readonly ProjectDataContext _projectDataContext;
        private readonly IncomeDataContext _incomeDataContext;
        private readonly ExpenseDataContext _expenseDataContext;

        #region constructor
        public SessionDetailsController()
        {
            _eventDataContext = new EventDataContext();
            _meetingDataContext = new MeetingDataContext();
            _projectDataContext = new ProjectDataContext();
            _incomeDataContext = new IncomeDataContext();
            _expenseDataContext = new ExpenseDataContext();
        }
        #endregion

        public JsonResult IncExpe(long sessionid)
        {
            IncExp viewModel = new IncExp();

            viewModel.Income = _incomeDataContext.IncomeItem.Where(s => s.IncomeCategory.SessionID == sessionid).Sum(s => (Decimal?)s.Amount) ?? 0;
            viewModel.Expenses = _expenseDataContext.ExpenseItem.Where(s => s.ExpenseCategory.SessionID == sessionid).Sum(s => (Decimal?)s.Amount) ?? 0;
            return Json(new { view = viewModel}, JsonRequestBehavior.AllowGet);
        }

        #region statistics
        //
        // GET: /SessionDetails/
        [HttpGet]
        [Authorize]
        public ActionResult Statistics(long sessionid)
        {
            Session["sessionid"] = sessionid;
            ViewBag.Event = _eventDataContext.Event.Where(s => s.SessionID == sessionid).ToArray().Length;
            ViewBag.Meeting = _meetingDataContext.Meetings.Where(s => s.SessionID == sessionid).ToArray().Length;
            ViewBag.Project = _projectDataContext.Projects.Where(s => s.SessionID == sessionid).ToArray().Length;
            ViewBag.Income = _incomeDataContext.IncomeItem.Where(s => s.IncomeCategory.SessionID == sessionid).Sum(s => (Decimal?)s.Amount) ?? 0;
            ViewBag.Expense = _expenseDataContext.ExpenseItem.Where(s => s.ExpenseCategory.SessionID == sessionid).Sum(s => (Decimal?)s.Amount) ?? 0;
            ViewBag.Balance = ViewBag.Income - ViewBag.Expense;
            return View("Statistics");
        }
        #endregion
    }
}