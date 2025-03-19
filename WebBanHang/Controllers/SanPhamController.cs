using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        private WebAppDBEntities2 db = new WebAppDBEntities2();

        // Lấy danh sách từ bảng Food
        public ActionResult Info()
        {
            var list = db.Foods.ToList();
            return View(list);
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
