
using System.Linq;
using System.Web.Mvc;
using BudgetExpenceProject.Models;

namespace BudgetExpenceProject.Controllers
{
    public class HomeController : Controller
    {
        BudgetExpenceTablesEntities db = new BudgetExpenceTablesEntities();

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            int userId = int.Parse(Session["UserId"].ToString());

            var totalBudget = db.Budgets
                .Where(x => x.UserId == userId)
                .Sum(x => (decimal?)x.BudgetAmount) ?? 0;

            var totalExpense = db.Expenses
                .Where(x => x.UserId == userId)
                .Sum(x => (decimal?)x.Amount) ?? 0;

            ViewBag.Budget = totalBudget;
            ViewBag.Expense = totalExpense;

            if (totalExpense > totalBudget)
            {
                ViewBag.Warning = "Warning: Your expenses are higher than your budget!";
            }

            return View();
        }
    }
}
