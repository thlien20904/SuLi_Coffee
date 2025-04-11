using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;

namespace WebBanHang.Controllers
{
    public class GioHangController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4();

        [HttpPost]
        public ActionResult ThemVaoGio(int foodId, int soLuong, int? sizeId, List<int> toppingIds)
        {
            try
            {
                if (Session["Id"] == null)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thêm vào giỏ hàng!" }, JsonRequestBehavior.AllowGet);
                }

                if (!int.TryParse(Session["Id"].ToString(), out int userId))
                {
                    return Json(new { success = false, message = "Lỗi khi lấy thông tin tài khoản!" });
                }

                var food = db.Foods.FirstOrDefault(f => f.FoodId == foodId);
                if (food == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không hợp lệ!" });
                }

                if (soLuong <= 0)
                {
                    return Json(new { success = false, message = "Số lượng không hợp lệ!" });
                }
                decimal sizePrice = 0;
                decimal toppingTotalPrice = 0;
                // Tính giá size (nếu có)
                if (sizeId.HasValue)
                {
                    var size = db.Sizes.FirstOrDefault(s => s.SizeID == sizeId);
                    if (size != null)
                    {
                        sizePrice = size.ExtraPrice;
                    }
                }

                // Tính giá topping
                if (toppingIds != null && toppingIds.Any())
                {
                    toppingTotalPrice = db.Toppings
                                        .Where(t => toppingIds.Contains(t.ToppingID))
                                        .Sum(t => t.ToppingPrice);
                }
                // Tổng giá của sản phẩm bao gồm size và topping
                decimal itemTotalPrice = (food.Price + sizePrice + toppingTotalPrice) * soLuong;
                // Kiểm tra xem đã có sản phẩm cùng loại và cùng Size trong giỏ hàng chưa
                var gioHangItem = db.GioHangs.FirstOrDefault(g =>
                    g.Id == userId && g.FoodId == foodId &&
                    (g.SizeID == sizeId || (!g.SizeID.HasValue || sizeId == 0))
                );
                if (gioHangItem != null)
                {
                    gioHangItem.SoLuong += soLuong;
                    gioHangItem.TotalPrice += itemTotalPrice;
                }
                else
                {
                    gioHangItem = new GioHang
                    {
                        Id = userId,
                        FoodId = foodId,
                        SoLuong = soLuong,
                        SizeID = sizeId, // Thêm Size vào giỏ hàng
                        TotalPrice = itemTotalPrice
                    };
                    db.GioHangs.Add(gioHangItem);
                    db.SaveChanges();
                }

                // Thêm Topping vào bảng GioHang_Topping
                if (toppingIds != null && toppingIds.Any())
                {
                    foreach (var toppingId in toppingIds)
                    {
                        db.GioHang_Topping.Add(new GioHang_Topping
                        {
                            GioHangID = gioHangItem.GioHangID,
                            ToppingID = toppingId
                        });
                    }
                    db.SaveChanges();
                }

                // Cập nhật tổng số lượng sản phẩm trong giỏ hàng
                int tongSoLuong = db.GioHangs.Where(g => g.Id == userId).Sum(g => (int?)g.SoLuong).GetValueOrDefault();
                Session["SoLuongGioHang"] = tongSoLuong;

                return Json(new { success = true, soLuongGioHang = tongSoLuong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        public ActionResult GioHang()
        {
            if (Session["Id"] == null)
            {
                return RedirectToAction("Login", "Login", new { returnUrl = Request.Url.PathAndQuery });
            }

            int userId = Convert.ToInt32(Session["Id"]);

            var gioHangItems = db.GioHangs
                .Where(gh => gh.Id == userId)
                .Include(gh => gh.Food)
                .Include(gh => gh.Size)
                .Include(gh => gh.GioHang_Topping.Select(gt => gt.Topping))
                .ToList();

            int tongSoLuong = gioHangItems.Sum(g => g.SoLuong);
            Session["SoLuongGioHang"] = tongSoLuong;

            var cartList = gioHangItems.Select(g => new GioHang
            {
                GioHangID = g.GioHangID,
                Id = g.Id,
                Food = new Food
                {
                    FoodId = g.Food.FoodId,
                    FoodName = g.Food.FoodName,
                    ImageURL = g.Food.ImageURL,
                    Price = g.Food.Price
                },
                SoLuong = g.SoLuong,
                Size = g.Size != null ? new Size { SizeName = g.Size.SizeName, ExtraPrice = g.Size.ExtraPrice } : null,
                GioHang_Topping = g.GioHang_Topping.Select(gt => new GioHang_Topping
                {
                    Topping = new Topping { ToppingName = gt.Topping.ToppingName, ToppingPrice = gt.Topping.ToppingPrice }
                }).ToList(),
                TotalPrice = g.SoLuong * (g.Food.Price + (g.Size?.ExtraPrice ?? 0) + g.GioHang_Topping.Sum(gt => gt.Topping.ToppingPrice))
            }).ToList();

            return View(cartList);
        }




        [HttpPost]
        public ActionResult XoaKhoiGio(int foodId)
        {
            try
            {
                if (Session["Id"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                int userId = Convert.ToInt32(Session["Id"]);

                // Log thông tin sản phẩm trước khi xóa
                var gioHangItem = db.GioHangs
                    .Include(g => g.GioHang_Topping)
                    .FirstOrDefault(g => g.Id == userId && g.FoodId == foodId);

                if (gioHangItem != null)
                {
                    // Log thông tin sản phẩm
                    Console.WriteLine("Thông tin sản phẩm trước khi xóa:");
                    Console.WriteLine("FoodId: " + gioHangItem.FoodId);
                    Console.WriteLine("SizeId: " + gioHangItem.SizeID);
                    Console.WriteLine("Số lượng: " + gioHangItem.SoLuong);
                    Console.WriteLine("Tên sản phẩm: " + gioHangItem.Food.FoodName); // Giả sử bạn có thuộc tính Food.Ten trong model

                    // Tiến hành xóa sản phẩm khỏi giỏ hàng
                    db.GioHangs.Remove(gioHangItem);
                    db.SaveChanges();

                    // Cập nhật số lượng giỏ hàng
                    int tongSoLuong = db.GioHangs.Where(g => g.Id == userId).Sum(g => (int?)g.SoLuong).GetValueOrDefault();
                    Session["SoLuongGioHang"] = tongSoLuong;

                    return Json(new { success = true, soLuongGioHang = tongSoLuong });
                }

                return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }

        }
    }
}
