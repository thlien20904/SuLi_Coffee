using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class TaiKhoanController : Controller
    {
        private WebAppDBEntities3 db = new WebAppDBEntities3();

        public ActionResult TaiKhoan()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string username = Session["Username"].ToString();
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View(user);
        }
        [HttpPost]
        public JsonResult UpdateUser(int id, string field, string value)
        {
            if (Session["Username"] == null)
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
            }

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng!" });
            }

            string currentUsername = Session["Username"].ToString();
            string currentRole = Session["UserRole"]?.ToString() ?? "";

            if (currentRole != "Admin" && currentUsername != user.Username)
            {
                return Json(new { success = false, message = "Bạn không có quyền chỉnh sửa!" });
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return Json(new { success = false, message = "Dữ liệu không được để trống!" });
            }

            try
            {
                switch (field)
                {
                    case "Username":
                        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9]{3,20}$"))
                            return Json(new { success = false, message = "Username phải từ 3-20 ký tự, chỉ chứa chữ và số!" });
                        if (db.Users.Any(u => u.Username == value && u.Id != id))
                            return Json(new { success = false, message = "Username đã tồn tại!" });
                        string oldUsername = user.Username;
                        user.Username = value.Trim();
                        if (currentUsername == oldUsername)
                        {
                            Session["Username"] = user.Username;
                        }
                        break;

                    case "FullName":
                        if (value.Length < 3)
                            return Json(new { success = false, message = "Tên quá ngắn!" });
                        user.FullName = value.Trim();
                        break;

                    case "Email":
                        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                            return Json(new { success = false, message = "Email không hợp lệ!" });
                        if (db.Users.Any(u => u.Email == value && u.Id != id))
                            return Json(new { success = false, message = "Email đã tồn tại!" });
                        user.Email = value.Trim();
                        break;

                    case "Phone":
                        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^(0[3|5|7|8|9])[0-9]{8}$"))
                            return Json(new { success = false, message = "Số điện thoại không hợp lệ!" });
                        user.Phone = value.Trim();
                        break;

                    case "Address":
                        user.Address = value.Trim();
                        break;

                    default:
                        return Json(new { success = false, message = "Trường không hợp lệ!" });
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật {field}: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật. Vui lòng thử lại!" });
            }
        }
    }
}
