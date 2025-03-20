using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4();

        // Trang sản phẩm mặc định
        public ActionResult SanPham(string searchString, int? categoryId)
        {
            var list = db.Food.AsQueryable();

            // Lọc theo tên món ăn
            if (!string.IsNullOrEmpty(searchString))
            {
                list = list.Where(f => f.FoodName.Contains(searchString));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                list = list.Where(f => f.CategoryId == categoryId);
            }

            return View(list.ToList());
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
