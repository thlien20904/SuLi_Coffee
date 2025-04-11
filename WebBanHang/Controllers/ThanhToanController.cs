using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHang.Models;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace WebBanHang.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly WebAppDBEntities4 db = new WebAppDBEntities4();

        // Hiển thị trang thanh toán
        public ActionResult ThanhToan()
        {
            if (Session["Id"] == null || !int.TryParse(Session["Id"]?.ToString(), out int userId))
            {
                return RedirectToAction("Login", "Login");
            }

            // Lấy thông tin giỏ hàng của người dùng
            var gioHang = db.GioHangs
                            .Include(g => g.Food)
                            .Include(g => g.Size)
                            .Where(g => g.Id == userId)
                            .ToList();

            if (gioHang.Count == 0)
            {
                return RedirectToAction("GioHang", "GioHang"); // Chuyển về giỏ hàng nếu rỗng
            }

            // Lấy thông tin người dùng
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Tính tổng tiền đơn hàng
            decimal totalAmount = gioHang.Sum(g => g.TotalPrice);

            // Lấy danh sách phương thức thanh toán
            var paymentMethods = db.PhuongThucThanhToans.ToList();
            ViewBag.PaymentMethods = paymentMethods;

            ViewBag.User = user;
            ViewBag.CartItems = gioHang;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.OrderDate = DateTime.Now;

            // Debug giá trị của ViewBag
            System.Diagnostics.Debug.WriteLine($"OrderDate: {ViewBag.OrderDate}");

            return View(user); // Truyền Model sang View
        }

        [HttpPost]
        public JsonResult MuaNgay(int foodId, int soLuong, int sizeId, List<int> toppingIds)
        {
            try
            {
                if (Session["Id"] == null || !int.TryParse(Session["Id"]?.ToString(), out int userId))
                {
                    return Json(new { success = false, message = "Bạn chưa đăng nhập!" }, JsonRequestBehavior.AllowGet);
                }

                // Log để debug
                System.Diagnostics.Debug.WriteLine($"UserId from Session: {userId}");

                // Kiểm tra xem sản phẩm có tồn tại không
                var food = db.Foods.FirstOrDefault(f => f.FoodId == foodId);
                if (food == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm!" }, JsonRequestBehavior.AllowGet);
                }

                // Kiểm tra xem size có tồn tại không
                var size = db.Sizes.FirstOrDefault(s => s.SizeID == sizeId);
                if (size == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy kích thước sản phẩm!" }, JsonRequestBehavior.AllowGet);
                }

                // Xóa toàn bộ giỏ hàng hiện tại
                var existingCart = db.GioHangs.Where(g => g.Id == userId).ToList();
                db.GioHangs.RemoveRange(existingCart);
                db.SaveChanges();

                // Tính tổng giá của sản phẩm
                decimal totalPrice = food.Price + size.ExtraPrice;

                // Thêm giá của các topping (nếu có)
                if (toppingIds != null && toppingIds.Count > 0)
                {
                    var toppings = db.Toppings.Where(tp => toppingIds.Contains(tp.ToppingID)).ToList();
                    totalPrice += toppings.Sum(t => t.ToppingPrice);
                }

                // Nhân với số lượng
                totalPrice *= soLuong;

                // Tạo mới giỏ hàng chỉ với sản phẩm đang mua
                var newCartItem = new GioHang
                {
                    Id = userId,
                    FoodId = foodId,
                    SizeID = sizeId,
                    SoLuong = soLuong,
                    TotalPrice = totalPrice
                };

                db.GioHangs.Add(newCartItem);
                db.SaveChanges();

                // Cập nhật lại số lượng trong session
                Session["SoLuong"] = soLuong;

                // Thêm các topping vào chi tiết giỏ hàng (nếu có)
                if (toppingIds != null && toppingIds.Count > 0)
                {
                    foreach (var toppingId in toppingIds)
                    {
                        var cartTopping = new GioHang_Topping
                        {
                            GioHangID = newCartItem.GioHangID,
                            ToppingID = toppingId
                        };
                        db.GioHang_Topping.Add(cartTopping);
                    }
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi mua ngay: {ex.Message}");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }        // Xử lý cập nhật thông tin người dùng (AJAX)
        [HttpPost]
        public JsonResult UpdateUser(int id, string field, string value)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateUser called with id: {id}, field: {field}, value: {value}");

            if (Session["Id"] == null || !int.TryParse(Session["Id"]?.ToString(), out int userId))
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập!" }, JsonRequestBehavior.AllowGet);
            }

            var user = db.Users.FirstOrDefault(u => u.Id == id); // Sửa Users thành User
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng!" }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra quyền: chỉ người dùng đó hoặc Admin mới được sửa
            if (userId != id)
            {
                string currentRole = Session["Role"]?.ToString() ?? ""; // Sửa UserRole thành Role
                if (currentRole != "Admin")
                {
                    return Json(new { success = false, message = "Bạn không có quyền chỉnh sửa thông tin này!" }, JsonRequestBehavior.AllowGet);
                }
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return Json(new { success = false, message = $"Trường {field} không được để trống!" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                switch (field)
                {
                    case "FullName":
                        if (value.Length < 3)
                            return Json(new { success = false, message = "Tên phải từ 3 ký tự trở lên!" }, JsonRequestBehavior.AllowGet);
                        user.FullName = value.Trim();
                        break;

                    case "Email":
                        if (!Regex.IsMatch(value, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                            return Json(new { success = false, message = "Email không hợp lệ!" }, JsonRequestBehavior.AllowGet);
                        if (db.Users.Any(u => u.Email == value && u.Id != id)) // Sửa Users thành User
                            return Json(new { success = false, message = "Email đã tồn tại!" }, JsonRequestBehavior.AllowGet);
                        user.Email = value.Trim();
                        break;

                    case "Phone":
                        if (!Regex.IsMatch(value, @"^(0[3|5|7|8|9])[0-9]{8}$"))
                            return Json(new { success = false, message = "Số điện thoại không hợp lệ! Phải bắt đầu bằng 03, 05, 07, 08, 09 và có 10 chữ số." }, JsonRequestBehavior.AllowGet);
                        user.Phone = value.Trim();
                        break;

                    case "Address":
                        user.Address = value.Trim();
                        break;

                    default:
                        return Json(new { success = false, message = "Trường không hợp lệ!" }, JsonRequestBehavior.AllowGet);
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật {field}: {ex.Message}");
                return Json(new { success = false, message = $"Có lỗi xảy ra khi cập nhật {field}: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ThanhToan(string address, int paymentMethodId)
        {
            if (Session["Id"] == null || !int.TryParse(Session["Id"]?.ToString(), out int userId))
            {
                return RedirectToAction("Login", "Login");
            }

            var gioHang = db.GioHangs
                            .Include(g => g.Food)
                            .Include(g => g.Size)
                            .Where(g => g.Id == userId)
                            .ToList();

            if (gioHang.Count == 0)
            {
                return RedirectToAction("GioHang", "GioHang");
            }

            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng!";
                return RedirectToAction("ThanhToan");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                TempData["Error"] = "Địa chỉ không được để trống!";
                return RedirectToAction("ThanhToan");
            }

            // Cập nhật địa chỉ mới
            user.Address = address;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            // Tính tổng tiền đơn hàng
            decimal totalAmount = gioHang.Sum(g => g.TotalPrice);

            // Lấy StatusId của trạng thái "Đặt hàng thành công"
            var status = db.OrderStatus.FirstOrDefault(s => s.StatusName == "Đặt hàng thành công");
            if (status == null)
            {
                TempData["Error"] = "Không tìm thấy trạng thái 'Đặt hàng thành công' trong hệ thống!";
                return RedirectToAction("ThanhToan");
            }

            // Tạo đơn hàng mới
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                PaymentMethodId = paymentMethodId,
                StatusId = status.StatusId // Gán StatusId
            };
            db.Orders.Add(order);
            db.SaveChanges(); // Save để lấy OrderId

            // Thêm chi tiết đơn hàng
            foreach (var item in gioHang)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    FoodId = item.FoodId,
                    SizeId = item.SizeID,
                    Quantity = item.SoLuong,
                    Price = item.TotalPrice
                };
                db.OrderDetails.Add(orderDetail);
            }
            db.SaveChanges();

            // Xóa giỏ hàng
            db.GioHangs.RemoveRange(gioHang);
            db.SaveChanges();

            // Reset session giỏ hàng
            Session["SoLuong"] = 0;

            return RedirectToAction("ThanhCong");
        }

        public ActionResult ThanhCong()
        {
            ViewBag.Message = "Bạn đã đặt hàng thành công lúc " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
            return View();
        }
    }
}