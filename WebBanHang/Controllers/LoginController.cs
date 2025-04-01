using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class LoginController : Controller
    {
        private WebAppDBEntities3 db = new WebAppDBEntities3();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }


        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                bool isValid = false;

                if (user.PasswordHash.StartsWith("$2a$") || user.PasswordHash.StartsWith("$2b$"))
                {
                    // Là password hash chuẩn bcrypt
                    isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                }
                else
                {
                    // Password cũ lưu dạng plain text
                    isValid = (password == user.PasswordHash);
                }

                if (isValid)
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
            }

            ViewBag.Message = "Sai thông tin đăng nhập!";
            return View();
        }

    }
}
