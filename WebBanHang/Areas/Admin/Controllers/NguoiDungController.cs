using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4();

        // Hiển thị danh sách Users
        public ActionResult NguoiDung()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}