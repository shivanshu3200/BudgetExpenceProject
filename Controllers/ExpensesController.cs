
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BudgetExpenceProject.Models;
using System;

namespace BudgetExpenceProject.Controllers
{
    public class ExpensesController : Controller
    {
        private BudgetExpenceTablesEntities db = new BudgetExpenceTablesEntities();

        // Seed default categories if empty
        private void SeedDefaultCategories()
        {
            if (!db.Categories.Any())
            {
                var defaults = new[]
                {
                    new Category { CategoryId = 1, CategoryName = "Food", Type = "Expense" },
                    new Category { CategoryId = 2, CategoryName = "Travel", Type = "Expense" },
                    new Category { CategoryId = 3, CategoryName = "Shopping", Type = "Expense" },
                    new Category { CategoryId = 4, CategoryName = "Groceries", Type = "Expense" },
                    new Category { CategoryId = 5, CategoryName = "Bills", Type = "Expense" },
                    new Category { CategoryId = 6, CategoryName = "Medical", Type = "Expense" },
                    new Category { CategoryId = 7, CategoryName = "Entertainment", Type = "Expense" },
                    new Category { CategoryId = 8, CategoryName = "Other", Type = "Expense" }
                };

                foreach (var c in defaults)
                    db.Categories.Add(c);

                db.SaveChanges();
            }
        }

        // LIST EXPENSES
        public async Task<ActionResult> Index()
        {
            int userId = int.Parse(Session["UserId"].ToString());

            var expenses = db.Expenses
                             .Include(e => e.Category)
                             .Include(e => e.Login)
                             .Where(e => e.UserId == userId);

            return View(await expenses.ToListAsync());
        }

        // DETAILS (FIXED)
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);

            var expense = await db.Expenses
                                  .Include(e => e.Category)
                                  .Include(e => e.Login)
                                  .FirstOrDefaultAsync(e => e.ExpenseId == id);

            if (expense == null)
                return HttpNotFound();

            return View(expense);
        }

        // CREATE (GET)
        public ActionResult Create()
        {
            SeedDefaultCategories();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "CategoryId,Amount,Date,PaymentMethod,Note")] Expense expense)
        {
            SeedDefaultCategories();

            int userId = int.Parse(Session["UserId"].ToString());

            var budget = db.Budgets
                           .Where(b => b.UserId == userId)
                           .OrderByDescending(b => b.EndDate)
                           .FirstOrDefault();

            if (budget == null)
            {
                ViewBag.Error = "Please create a budget first.";
                Load(expense);
                return View(expense);
            }

            if (expense.Date < budget.StartDate || expense.Date > budget.EndDate)
            {
                ViewBag.Error = "Expense date is outside your budget period!";
                Load(expense);
                return View(expense);
            }

            expense.ExpenseId = db.Expenses.Any() ? db.Expenses.Max(x => x.ExpenseId) + 1 : 1;
            expense.UserId = userId;

            db.Expenses.Add(expense);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // EDIT (GET)
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);

            Expense expense = await db.Expenses.FindAsync(id);
            if (expense == null)
                return HttpNotFound();

            SeedDefaultCategories();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", expense.CategoryId);

            return View(expense);
        }

        // EDIT (POST)
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "ExpenseId,CategoryId,Amount,Date,PaymentMethod,Note")] Expense expense)
        {
            int userId = int.Parse(Session["UserId"].ToString());
            expense.UserId = userId;

            if (ModelState.IsValid)
            {
                db.Entry(expense).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            SeedDefaultCategories();
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", expense.CategoryId);

            return View(expense);
        }

        // DELETE (GET)
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);

            var expense = await db.Expenses
                                  .Include(e => e.Category)
                                  .Include(e => e.Login)
                                  .FirstOrDefaultAsync(e => e.ExpenseId == id);

            if (expense == null)
                return HttpNotFound();

            return View(expense);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Expense expense = await db.Expenses.FindAsync(id);

            db.Expenses.Remove(expense);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Load dropdown
        private void Load(Expense expense)
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", expense.CategoryId);
        }
    }
}
