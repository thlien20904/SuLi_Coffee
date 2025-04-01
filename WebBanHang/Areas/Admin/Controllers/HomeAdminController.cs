using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;
using System.Collections.Generic;

namespace WebBanHang.Controllers
{
    public class HomeAdminController : Controller
    {
        private WebAppDBEntities3 db = new WebAppDBEntities3();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                filterContext.Result = RedirectToAction("Login", "Login", new { area = "" });
            }
            base.OnActionExecuting(filterContext);
        }

        public ActionResult HomeAdmin()
        {
            // Lấy tổng số sản phẩm & nguyên liệu
            ViewBag.TotalProducts = db.Foods.Count();
            ViewBag.TotalIngredients = db.Ingredients.Count();

            // Tính toán sản phẩm bán chạy từ InvoiceDetail
            var bestSellers = db.InvoiceDetails
                .Where(id => id.FoodId != null) // Bỏ qua giá trị null (nếu có)
                .GroupBy(id => id.FoodId)
                .Select(g => new
                {
                    FoodId = g.Key.Value, // Vì FoodId là Nullable<int>, cần lấy giá trị thật
                    TotalSold = g.Sum(id => id.SoLuong),
                    Food = db.Foods.FirstOrDefault(f => f.FoodId == g.Key.Value)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(8)
                .ToList();

            // Chuyển dữ liệu vào ViewBag dưới dạng `BanChayModel`
            ViewBag.BanChay = bestSellers.Select(b => new BanChayModel
            {
                FoodId = b.FoodId,
                FoodName = b.Food.FoodName,
                ImageUrl = b.Food.ImageURL,
                Price = b.Food.Price,
                TotalSold = b.TotalSold
            }).ToList();

            // Lấy danh sách nguyên liệu sắp hết
            ViewBag.LowStockIngredients = db.Ingredients
                .Where(i => i.SoLuong < 10)
                .ToList();

            return View();
        }


        [HttpGet]
        public ActionResult GlobalSearch(string query, string filterType)
        {
            if (string.IsNullOrEmpty(query))
            {
                TempData["Error"] = "Vui lòng nhập từ khóa tìm kiếm.";
                return RedirectToAction("HomeAdmin");
            }

            switch (filterType)
            {
                case "food":
                    return RedirectToAction("Info", "SanPhamAdmin", new { area = "Admin", search = query });
                case "invoice":
                    return RedirectToAction("HoaDon", "BaoCao", new { area = "Admin", search = query });
                case "staff":
                    return RedirectToAction("NhanVien", "NhanVien", new { area = "Admin", search = query });
                case "ingredient":
                    return RedirectToAction("KhoAdmin", "KhoAdmin", new { area = "Admin", search = query });
                default:
                    return RedirectToAction("HomeAdmin", "HomeAdmin", new { area = "Admin", search = query });
            }
        }
    }
}
