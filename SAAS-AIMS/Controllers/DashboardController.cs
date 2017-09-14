using AIMS.Data.DataContext.DataContext.DuesDataContext;
using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Member;
using AIMS.Data.Enums.Enums.Gender;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AIMS.Data.ViewModels.ViewModel.Member;
using AIMS.Data.DataContext.DataContext.IncomeDataContext;
using AIMS.Data.DataContext.DataContext.ExpenseDataContext;
using AIMS.Data.ViewModels.ViewModel.Finance;

namespace SAAS_AIMS.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly MemberDataContext _memberdatacontext;
        private readonly SessionDataContext _sessiondatacontext;
        private readonly IncomeDataContext _incomedatacontext;
        private readonly ExpenseDataContext _expensedatacontext;
       
        #region constructor
        public DashboardController()
        {
            _memberdatacontext = new MemberDataContext();
            _sessiondatacontext = new SessionDataContext();
            _incomedatacontext = new IncomeDataContext();
            _expensedatacontext = new ExpenseDataContext();
        }
        #endregion

        #region bar chart
        public ContentResult FinanceSummary()
        {
            List<FinanceSummaryViewModel> FinanceSummary = new List<FinanceSummaryViewModel>();

            var sessions = _sessiondatacontext.Sessions.ToList();

            foreach (AIMS.Data.DataObjects.Entities.Session.Session session in sessions)
            {
                FinanceSummaryViewModel financeSummary = new FinanceSummaryViewModel();
                financeSummary.y = session.Title;

                var incomeitem = _incomedatacontext.IncomeItem.Where(s => s.IncomeCategory.SessionID == session.ID);
                financeSummary.a = incomeitem.Sum(s => (Decimal?)s.Amount) ?? 0;

                var expenseitem = _expensedatacontext.ExpenseItem.Where(s => s.ExpenseCategory.SessionID == session.ID);
                financeSummary.b = expenseitem.Sum(s => (Decimal?)s.Amount) ?? 0;

                financeSummary.c = financeSummary.a - financeSummary.b;

                FinanceSummary.Add(financeSummary);
            }
            return Content(JsonConvert.SerializeObject(FinanceSummary), "application/json");
        }
        #endregion

        #region index method
        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            ViewBag.Members = _memberdatacontext.Members.ToArray().Length;
            ViewBag.Male = _memberdatacontext.Members.Where(member => member.Gender == Gender.Male).ToArray().Length;
            ViewBag.Female = _memberdatacontext.Members.Where(member => member.Gender == Gender.Female).ToArray().Length;
            ViewBag.Session = _sessiondatacontext.Sessions.ToArray().Length;
            ViewBag.Income = _incomedatacontext.IncomeItem.Sum(s => (Decimal?)s.Amount) ?? 0;
            ViewBag.Expense = _expensedatacontext.ExpenseItem.Sum(s => (Decimal?)s.Amount) ?? 0;
            return View();
        }
        #endregion
    }
}