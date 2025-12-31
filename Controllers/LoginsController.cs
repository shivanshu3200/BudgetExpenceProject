
using BudgetExpenceProject.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace BudgetExpenceProject.Controllers
{
    public class LoginsController : Controller
    {
        private BudgetExpenceTablesEntities db = new BudgetExpenceTablesEntities();

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string Email, string Password)
        {
            var user = db.Logins.FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Email, false);

                Session["UserId"] = user.UserId;
                Session["FullName"] = user.FullName;

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = "Invalid Email or Password!";
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Create([Bind(Include = "FullName,Email,Password,PhoneNo_")] Login login)
        {
            if (ModelState.IsValid)
            {
                login.UserId = db.Logins.Any() ? db.Logins.Max(x => x.UserId) + 1 : 1;

                db.Logins.Add(login);
                await db.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(login);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            return View(await db.Logins.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(400);

            Login login = await db.Logins.FindAsync(id);
            if (login == null) return HttpNotFound();

            return View(login);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(400);

            Login login = await db.Logins.FindAsync(id);
            if (login == null) return HttpNotFound();

            return View(login);
        }

        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "UserId,FullName,Email,Password,PhoneNo_")] Login login)
        {
            if (ModelState.IsValid)
            {
                db.Entry(login).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(login);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(400);

            Login login = await db.Logins.FindAsync(id);
            if (login == null) return HttpNotFound();

            return View(login);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Login login = await db.Logins.FindAsync(id);

            db.Logins.Remove(login);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
