using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class SanPhamAdminController : Controller
    {
        private WebAppDBEntities2 db = new WebAppDBEntities2();

        // Danh sách món ăn
        public ActionResult Info()
        {
            TempData["ErrorMessage"] = null;

            var foodList = db.Foods.ToList(); // Lấy danh sách món ăn từ database

            Debug.WriteLine("Số lượng món ăn trong DB: " + foodList.Count); // Kiểm tra xem có dữ liệu hay không

            if (foodList == null || !foodList.Any())
            {
                TempData["ErrorMessage"] = "Không có sản phẩm nào.";
            }

            return View(foodList); // Truyền danh sách vào View
        }


        // Thêm món ăn (GET)
        public ActionResult Add()
        {
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.Ingredients = db.Ingredients.ToList();
            return View(new FoodAddViewModel { Food = new Food() });
        }

        // Thêm món ăn (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FoodAddViewModel model)
        {
            if (!ValidateFood(model.Food))
            {
                ViewBag.Categories = db.Categories.ToList();
                ViewBag.Ingredients = db.Ingredients.ToList();
                return View(model);
            }

            model.Food.UpdatedDate = DateTime.Now;
            db.Foods.Add(model.Food);
            db.SaveChanges();

            // Lưu vào bảng trung gian nếu có nguyên liệu
            if (model.SelectedIngredientIds != null)
            {
                foreach (var ingredientId in model.SelectedIngredientIds)
                {
                    db.FoodIngredients.Add(new FoodIngredient
                    {
                        FoodId = model.Food.FoodId,
                        IngredientId = ingredientId,
                        Quantity = 1
                    });
                }
                db.SaveChanges();
            }

            TempData["SuccessMessage"] = "Thêm món ăn thành công!";
            return RedirectToAction("Info");
        }

        // Chỉnh sửa món ăn (GET)
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

        // Chỉnh sửa món ăn (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FoodEditViewModel model)
        {
            if (!ValidateFood(model.Food))
            {
                ViewBag.Categories = db.Categories.ToList();
                ViewBag.Ingredients = db.Ingredients.ToList();
                return View(model);
            }

            var food = db.Foods.Find(model.Food.FoodId);
            if (food == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy món ăn.";
                return RedirectToAction("Info");
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

            // Cập nhật nguyên liệu
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
            return RedirectToAction("Info");
        }

        // ✅ Hàm kiểm tra hợp lệ dữ liệu món ăn
        private bool ValidateFood(Food food)
        {
            bool isValid = true;

            // Kiểm tra tên món ăn không được để trống
            if (string.IsNullOrWhiteSpace(food.FoodName))
            {
                ModelState.AddModelError("FoodName", "Tên món ăn không được để trống!");
                isValid = false;
            }

            // Kiểm tra giá phải lớn hơn 0
            if (food.Price <= 0)
            {
                ModelState.AddModelError("Price", "Giá món ăn phải lớn hơn 0!");
                isValid = false;
            }

            // Kiểm tra số lượng tồn kho không âm
            if (food.Stock < 0)
            {
                ModelState.AddModelError("Stock", "Số lượng tồn kho không được nhỏ hơn 0!");
                isValid = false;
            }

            // Kiểm tra URL ảnh (nếu có)
            if (!string.IsNullOrEmpty(food.ImageURL) && !Uri.IsWellFormedUriString(food.ImageURL, UriKind.Absolute))
            {
                ModelState.AddModelError("ImageURL", "URL ảnh không hợp lệ!");
                isValid = false;
            }

            // Kiểm tra món ăn có trùng tên không (tránh trùng lặp)
            if (db.Foods.Any(f => f.FoodName == food.FoodName && f.FoodId != food.FoodId))
            {
                ModelState.AddModelError("FoodName", "Tên món ăn đã tồn tại!");
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

