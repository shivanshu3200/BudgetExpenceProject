
using BudgetExpenceProject.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BudgetExpenceProject.Controllers
{
    public class BudgetsController : Controller
    {
        private BudgetExpenceTablesEntities db = new BudgetExpenceTablesEntities();

        // LIST budgets of logged-in user
        public async Task<ActionResult> Index()
        {
            int userId = int.Parse(Session["UserId"].ToString());
            var budgets = db.Budgets.Where(b => b.UserId == userId);
            return View(await budgets.ToListAsync());
        }

        // DETAILS (GET)
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);

            var budget = await db.Budgets
                                 .Include(b => b.Login)
                                 .FirstOrDefaultAsync(b => b.BudgetId == id);

            if (budget == null)
                return HttpNotFound();

            return View(budget);
        }

        // CREATE (GET)
        public ActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "BudgetAmount,StartDate,EndDate")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                budget.BudgetId = db.Budgets.Any()
                    ? db.Budgets.Max(x => x.BudgetId) + 1
                    : 1;

                budget.UserId = int.Parse(Session["UserId"].ToString());
                budget.CreateDate = DateTime.Now;

                db.Budgets.Add(budget);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(budget);
        }

        // EDIT (GET)
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);

            Budget budget = await db.Budgets.FindAsync(id);

            if (budget == null)
                return HttpNotFound();

            return View(budget);
        }

        // EDIT (POST)
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "BudgetId,BudgetAmount,StartDate,EndDate,CreateDate")] Budget budget)
        {
            budget.UserId = int.Parse(Session["UserId"].ToString());

            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(budget);
        }

        // DELETE (GET)
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);

            var budget = await db.Budgets
                                 .Include(b => b.Login)
                                 .FirstOrDefaultAsync(b => b.BudgetId == id);

            if (budget == null)
                return HttpNotFound();

            return View(budget);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Budget budget = await db.Budgets.FindAsync(id);

            db.Budgets.Remove(budget);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
