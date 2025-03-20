
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class SanPhamAdminController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4();

        // Info
        public ActionResult Info()
        {
            var list = db.Food.ToList();
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
        public ActionResult Add(Food sp)
        {
            if (ModelState.IsValid)
            {
                db.Food.Add(sp);
                db.SaveChanges();
                return RedirectToAction("Info");
            }
            return View(sp);
        }

        // Edit (GET)
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Food sp = db.Food.Find(id);
            if (sp == null) return HttpNotFound();
            return View(sp);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Food sp)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Food.Find(sp.FoodId);
                if (existing == null)
                {
                    return HttpNotFound();
                }
                // Cập nhật các thuộc tính
                existing.FoodName = sp.FoodName;
                existing.CategoryId = sp.CategoryId;
                existing.IngredientId = sp.IngredientId;
                existing.Price = sp.Price;
                existing.Discount = sp.Discount;
                existing.DiscountPrice = sp.DiscountPrice;
                existing.Stock = sp.Stock;
                existing.Description = sp.Description;
                existing.ImageURL = sp.ImageURL;
                existing.Status = sp.Status;
                existing.UpdatedDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Info");
            }
            return View(sp);
        }

        // Delete (GET) - Xác nhận trước khi xóa
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Food sp = db.Food.Find(id);
            if (sp == null) return HttpNotFound();
            return View(sp); // View Confirm Delete
        }

        // Delete (POST) - Xác nhận Xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Food sp = db.Food.Find(id);
            if (sp == null) return HttpNotFound();

            db.Food.Remove(sp);
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
