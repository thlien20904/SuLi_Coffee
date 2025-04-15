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
                if (sizeId.HasValue && sizeId != 0)
                {
                    var size = db.Sizes.FirstOrDefault(s => s.SizeID == sizeId);
                    if (size != null)
                    {
                        sizePrice = size.ExtraPrice;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Kích thước không hợp lệ!" });
                    }
                }
                else
                {
                    sizeId = null; // Đảm bảo sizeId là null nếu không có size
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
                    g.Id == userId && g.FoodId == foodId && g.SizeID == sizeId);

                if (gioHangItem != null)
                {
                    // Nếu sản phẩm đã tồn tại, cập nhật số lượng và tổng giá
                    gioHangItem.SoLuong += soLuong;
                    gioHangItem.TotalPrice += itemTotalPrice;
                    db.SaveChanges();
                }
                else
                {
                    // Nếu sản phẩm chưa tồn tại, thêm mới
                    gioHangItem = new GioHang
                    {
                        Id = userId,
                        FoodId = foodId,
                        SoLuong = soLuong,
                        SizeID = sizeId, // Thêm Size vào giỏ hàng (null nếu không có size)
                        TotalPrice = itemTotalPrice
                    };
                    db.GioHangs.Add(gioHangItem);
                    db.SaveChanges();

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
                FoodId = g.FoodId, // Thêm FoodId để sử dụng trong CapNhatSoLuong
                Food = new Food
                {
                    FoodId = g.Food.FoodId,
                    FoodName = g.Food.FoodName,
                    ImageURL = g.Food.ImageURL,
                    Price = g.Food.Price
                },
                SoLuong = g.SoLuong,
                SizeID = g.SizeID, // Thêm SizeID để sử dụng trong CapNhatSoLuong
                Size = g.Size != null ? new Size { SizeName = g.Size.SizeName, ExtraPrice = g.Size.ExtraPrice } : null,
                GioHang_Topping = g.GioHang_Topping.Select(gt => new GioHang_Topping
                {
                    Topping = new Topping { ToppingName = gt.Topping.ToppingName, ToppingPrice = gt.Topping.ToppingPrice }
                }).ToList(),
                TotalPrice = g.TotalPrice
            }).ToList();

            return View(cartList);
        }

        [HttpPost]
        public ActionResult CapNhatSoLuong(int foodId, int? sizeId, int soLuong)
        {
            try
            {
                if (Session["Id"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                int userId = Convert.ToInt32(Session["Id"]);

                // Nếu sizeId là 0, đặt thành null
                if (sizeId == 0)
                {
                    sizeId = null;
                }

                // Tìm sản phẩm trong giỏ hàng
                var gioHangItem = db.GioHangs
                    .Include(g => g.Food)
                    .Include(g => g.Size)
                    .Include(g => g.GioHang_Topping.Select(gt => gt.Topping))
                    .FirstOrDefault(g => g.Id == userId && g.FoodId == foodId && g.SizeID == sizeId);

                if (gioHangItem == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng!" });
                }

                if (soLuong <= 0)
                {
                    return Json(new { success = false, message = "Số lượng không hợp lệ!" });
                }

                // Cập nhật số lượng
                gioHangItem.SoLuong = soLuong;

                // Tính lại tổng giá
                decimal sizePrice = gioHangItem.Size?.ExtraPrice ?? 0;
                decimal toppingTotalPrice = gioHangItem.GioHang_Topping.Sum(gt => gt.Topping.ToppingPrice);
                decimal itemTotalPrice = (gioHangItem.Food.Price + sizePrice + toppingTotalPrice) * soLuong;
                gioHangItem.TotalPrice = itemTotalPrice;

                db.SaveChanges();

                // Cập nhật tổng số lượng và tổng tiền giỏ hàng
                int tongSoLuong = db.GioHangs.Where(g => g.Id == userId).Sum(g => (int?)g.SoLuong).GetValueOrDefault();
                decimal newTotal = db.GioHangs.Where(g => g.Id == userId).Sum(g => (decimal?)g.TotalPrice).GetValueOrDefault();
                Session["SoLuongGioHang"] = tongSoLuong;

                return Json(new
                {
                    success = true,
                    cartCount = tongSoLuong,
                    newItemTotal = itemTotalPrice,
                    newTotal = newTotal
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult XoaKhoiGio(int foodId, int? sizeId)
        {
            try
            {
                if (Session["Id"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập." });
                }

                int userId = Convert.ToInt32(Session["Id"]);

                // Nếu sizeId là 0, đặt thành null
                if (sizeId == 0)
                {
                    sizeId = null;
                }

                // Tìm sản phẩm trong giỏ hàng dựa trên foodId và sizeId
                var gioHangItem = db.GioHangs
                    .Include(g => g.GioHang_Topping)
                    .FirstOrDefault(g => g.Id == userId && g.FoodId == foodId && g.SizeID == sizeId);

                if (gioHangItem != null)
                {
                    // Xóa các topping liên quan trước
                    db.GioHang_Topping.RemoveRange(gioHangItem.GioHang_Topping);
                    // Xóa sản phẩm khỏi giỏ hàng
                    db.GioHangs.Remove(gioHangItem);
                    db.SaveChanges();

                    // Cập nhật tổng số lượng và tổng tiền giỏ hàng
                    int tongSoLuong = db.GioHangs.Where(g => g.Id == userId).Sum(g => (int?)g.SoLuong).GetValueOrDefault();
                    decimal newTotal = db.GioHangs.Where(g => g.Id == userId).Sum(g => (decimal?)g.TotalPrice).GetValueOrDefault();
                    Session["SoLuongGioHang"] = tongSoLuong;

                    return Json(new { success = true, cartCount = tongSoLuong, newTotal = newTotal });
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