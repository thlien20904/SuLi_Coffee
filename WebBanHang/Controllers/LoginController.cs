using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class LoginController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
            if (user != null)
            {
                Session["Username"] = user.Username;
                Session["Role"] = user.Role;

                if (user.Role == "Admin")
                {
                    return RedirectToAction("HomeAdmin", "HomeAdmin", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Home", "Home");
                }
            }
            ViewBag.Message = "Sai thông tin đăng nhập!";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Home", "Home");
        }
    }
}
