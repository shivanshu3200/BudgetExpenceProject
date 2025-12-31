
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BudgetExpenceProject.Models;

namespace BudgetExpenceProject.Controllers
{
    public class CategoriesController : Controller
    {
        private BudgetExpenceTablesEntities db = new BudgetExpenceTablesEntities();

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

        public async Task<ActionResult> Index()
        {
            SeedDefaultCategories();

            int userId = int.Parse(Session["UserId"].ToString());

            // category-wise total expenses
            var expenseData = db.Expenses
                                .Where(e => e.UserId == userId)
                                .GroupBy(e => e.Category.CategoryName)
                                .Select(g => new
                                {
                                    Category = g.Key,
                                    Total = g.Sum(x => x.Amount)
                                }).ToList();

            ViewBag.Labels = expenseData.Select(x => x.Category).ToArray();
            ViewBag.Data = expenseData.Select(x => x.Total).ToArray();

            return View(await db.Categories.ToListAsync());
        }
    }
}
