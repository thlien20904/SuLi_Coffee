
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class SanPhamAdminController : Controller
    {
        private WebAppDBEntities2 db = new WebAppDBEntities2();

        // Info
        public ActionResult Info()
        {
            var list = db.SanPhams.ToList();
            return View(list);
        }

        // Add (GET)
        public ActionResult Add()
        {
            return View();
        }

        // Add (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(SanPham sp)
        {
            if (ModelState.IsValid)
            {
                db.SanPhams.Add(sp);
                db.SaveChanges();
                return RedirectToAction("Info");
            }
            return View(sp);
        }

        // Edit (GET)
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            SanPham sp = db.SanPhams.Find(id);
            if (sp == null) return HttpNotFound();
            return View(sp);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham sp)
        {
            if (ModelState.IsValid)
            {
                var existing = db.SanPhams.Find(sp.SanPhamID);
                if (existing == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật từng trường (an toàn hơn)
                existing.TenSanPham = sp.TenSanPham;
                existing.Gia = sp.Gia;
                existing.GiaGiam = sp.GiaGiam;
                existing.MoTa = sp.MoTa;
                existing.HinhAnh = sp.HinhAnh;
                existing.Loai = sp.Loai;
                existing.GiamGia = sp.GiamGia;

                db.SaveChanges();
                return RedirectToAction("Info");
            }
            return View(sp);
        }

        // Delete (GET) - Xác nhận trước khi xóa
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            SanPham sp = db.SanPhams.Find(id);
            if (sp == null) return HttpNotFound();
            return View(sp); // View Confirm Delete
        }

        // Delete (POST) - Xác nhận Xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sp = db.SanPhams.Find(id);
            if (sp == null) return HttpNotFound();

            db.SanPhams.Remove(sp);
            db.SaveChanges();
            return RedirectToAction("Info");
        }

        // Giải phóng tài nguyên DbContext
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
