using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class KhoAdminController : Controller
    {
        private WebAppDBEntities2 db = new WebAppDBEntities2();

        public ActionResult KhoAdmin()
        {
            TempData["ErrorMessage"] = null;
            var list = db.Ingredients.ToList();
            return View(list);
        }

        // Thêm nguyên liệu (GET)
        public ActionResult Add()
        {
            return View(new Ingredient());
        }

        // Thêm nguyên liệu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Ingredient ingredient)
        {
            try
            {
                if (!ValidateIngredient(ingredient))
                    return View(ingredient);

                if (db.Ingredients.Any(i => i.IngredientName == ingredient.IngredientName))
                {
                    ModelState.AddModelError("IngredientName", "Nguyên liệu này đã tồn tại.");
                    return View(ingredient);
                }

                ingredient.LastUpdated = DateTime.Now;
                db.Ingredients.Add(ingredient);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm nguyên liệu thành công!";
                return RedirectToAction("KhoAdmin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm nguyên liệu: " + ex.Message);
            }

            return View(ingredient);
        }

        // Chỉnh sửa nguyên liệu (GET)
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ingredient = db.Ingredients.Find(id);
            if (ingredient == null) return HttpNotFound();
            return View(ingredient);
        }

        // Chỉnh sửa nguyên liệu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ingredient ingredient)
        {
            try
            {
                if (!ValidateIngredient(ingredient))
                    return View(ingredient);

                if (db.Ingredients.Any(i => i.IngredientName == ingredient.IngredientName && i.IngredientId != ingredient.IngredientId))
                {
                    ModelState.AddModelError("IngredientName", "Tên nguyên liệu đã tồn tại.");
                    return View(ingredient);
                }

                ingredient.LastUpdated = DateTime.Now;
                db.Entry(ingredient).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật nguyên liệu thành công!";
                return RedirectToAction("KhoAdmin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật nguyên liệu: " + ex.Message);
            }

            return View(ingredient);
        }

        // Xóa nguyên liệu (GET)
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ingredient = db.Ingredients.Find(id);
            if (ingredient == null) return HttpNotFound();
            return View(ingredient);
        }

        // Xóa nguyên liệu (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var ingredient = db.Ingredients.Find(id);
                if (ingredient == null) return HttpNotFound();

                db.Ingredients.Remove(ingredient);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Xóa nguyên liệu thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa nguyên liệu: " + ex.Message;
            }

            return RedirectToAction("KhoAdmin");
        }

        // ✅ Hàm kiểm tra hợp lệ dữ liệu nguyên liệu
        private bool ValidateIngredient(Ingredient ingredient)
        {
            bool isValid = true;

            // Kiểm tra tên nguyên liệu không được để trống
            if (string.IsNullOrWhiteSpace(ingredient.IngredientName))
            {
                ModelState.AddModelError("IngredientName", "Tên nguyên liệu không được để trống!");
                isValid = false;
            }

            // Kiểm tra số lượng phải lớn hơn 0
            if (ingredient.SoLuong < 1)
            {
                ModelState.AddModelError("SoLuong", "Số lượng phải lớn hơn 0!");
                isValid = false;
            }

            // Kiểm tra phân loại không chứa ký tự đặc biệt
            if (!string.IsNullOrEmpty(ingredient.PhanLoai) && !Regex.IsMatch(ingredient.PhanLoai, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                ModelState.AddModelError("PhanLoai", "Phân loại không được chứa ký tự đặc biệt!");
                isValid = false;
            }

            // Kiểm tra ImageURL (chỉ kiểm tra nếu có nhập)
            if (!string.IsNullOrEmpty(ingredient.ImageURL) && !Uri.IsWellFormedUriString(ingredient.ImageURL, UriKind.Absolute))
            {
                ModelState.AddModelError("ImageURL", "URL ảnh không hợp lệ!");
                isValid = false;
            }

            return isValid;
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
