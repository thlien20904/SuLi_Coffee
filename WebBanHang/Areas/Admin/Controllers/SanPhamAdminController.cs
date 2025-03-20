using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class SanPhamAdminController : Controller
    {
        private WebAppDBEntities2 db = new WebAppDBEntities2();

        // Info - Danh sách món ăn
        public ActionResult Info()
        {
            TempData["ErrorMessage"] = null;
            var list = db.Foods.ToList();
            return View(list);
        }

        // Add (GET)
        public ActionResult Add()
        {
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Ingredients = db.Ingredients.ToList();

            // Trả về View với model rỗng
            var model = new FoodAddViewModel
            {
                Food = new Food()
            };
            return View(model);
        }

        // Add (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FoodAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Set ngày tạo
                model.Food.UpdatedDate = DateTime.Now;

                db.Foods.Add(model.Food);
                db.SaveChanges();

                // Lưu vào bảng trung gian
                if (model.SelectedIngredientIds != null)
                {
                    foreach (var ingredientId in model.SelectedIngredientIds)
                    {
                        db.FoodIngredients.Add(new FoodIngredient
                        {
                            FoodId = model.Food.FoodId,
                            IngredientId = ingredientId,
                            Quantity = 1 // Có thể custom số lượng ở đây
                        });
                    }
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Thêm món ăn mới thành công!";
                return RedirectToAction("Info");
            }

            // Load lại dữ liệu nếu có lỗi
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Ingredients = db.Ingredients.ToList();
            return View(model);
        }

        // Edit (GET)
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var food = db.Foods.Find(id);
            if (food == null) return HttpNotFound();

            var model = new FoodEditViewModel
            {
                Food = food,
                SelectedIngredientIds = food.FoodIngredients.Select(fi => fi.IngredientId.Value).ToList()
            };

            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Ingredients = db.Ingredients.ToList();
            return View(model);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FoodEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var food = db.Foods.Find(model.Food.FoodId);
                if (food == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy món ăn.";
                    return RedirectToAction("Info", "SanPhamAdmin");
                }

                // Cập nhật thông tin món ăn
                food.FoodName = model.Food.FoodName;
                food.CategoryId = model.Food.CategoryId;
                food.Price = model.Food.Price;
                food.Discount = model.Food.Discount;
                food.Stock = model.Food.Stock;
                food.Description = model.Food.Description;
                food.ImageURL = model.Food.ImageURL;
                food.UpdatedDate = DateTime.Now;
                food.Status = model.Food.Status;

                // Cập nhật nguyên liệu (xóa cũ thêm mới)
                db.FoodIngredients.RemoveRange(db.FoodIngredients.Where(fi => fi.FoodId == food.FoodId));
                if (model.SelectedIngredientIds != null)
                {
                    foreach (var ingId in model.SelectedIngredientIds)
                    {
                        db.FoodIngredients.Add(new FoodIngredient
                        {
                            FoodId = food.FoodId,
                            IngredientId = ingId,
                            Quantity = 1
                        });
                    }
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật món ăn thành công!";
                return RedirectToAction("Info", "SanPhamAdmin");
            }

            // Load lại dữ liệu dropdown nếu có lỗi
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Ingredients = db.Ingredients.ToList();
            return View(model);
        }



        // Delete (GET)
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var food = db.Foods
                .Include(f => f.Category)
                .Include(f => f.FoodIngredients.Select(fi => fi.Ingredient))
                .FirstOrDefault(f => f.FoodId == id);

            if (food == null) return HttpNotFound();
            return View(food);
        }

        // Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var food = db.Foods
                .Include(f => f.FoodIngredients) // Load các dòng liên quan trong bảng trung gian
                .FirstOrDefault(f => f.FoodId == id);

            if (food == null) return HttpNotFound();

            // Xóa dữ liệu bảng trung gian trước
            if (food.FoodIngredients != null && food.FoodIngredients.Any())
            {
                db.FoodIngredients.RemoveRange(food.FoodIngredients);
            }

            db.Foods.Remove(food);
            db.SaveChanges();
            return RedirectToAction("Info");
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
