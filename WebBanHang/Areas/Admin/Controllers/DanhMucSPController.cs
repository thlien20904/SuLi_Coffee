using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class DanhMucSPController : Controller
    {
        private WebAppDBEntities3 db = new WebAppDBEntities3();

        // Hiển thị danh sách danh mục
        public ActionResult DanhMucSP()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        // Thêm danh mục - GET
        public ActionResult Add()
        {
            return View();
        }

        // Thêm danh mục - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                return RedirectToAction("DanhMucSP");
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm danh mục.";
            return View(category);
        }

        // Chỉnh sửa danh mục - GET
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();

            return View(category);
        }

        // Chỉnh sửa danh mục - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("DanhMucSP");
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật danh mục.";
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Danh mục không tồn tại.";
                return RedirectToAction("DanhMucSP");
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            return RedirectToAction("DanhMucSP");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
