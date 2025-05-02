using System;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class DonHangOnlineController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4(); // Sửa thành WebAppDBEntities

        public ActionResult Index()
        {
            TempData["ErrorMessage"] = null;

            // Lấy tất cả đơn hàng
            var allOrders = db.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList()
                .Select(o =>
                {
                    var user = db.Users.FirstOrDefault(u => u.Id == o.UserId);
                    var status = db.OrderStatus.FirstOrDefault(s => s.StatusId == o.StatusId);
                    return new RecentOrderModel
                    {
                        OrderId = o.OrderId,
                        FullName = user != null ? user.FullName : "Unknown",
                        Status = status != null ? status.StatusName : "Unknown",
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount
                    };
                })
                .ToList();

            ViewBag.AllOrders = allOrders; // Đổi tên để tránh nhầm lẫn với RecentOrders

            // Lấy danh sách trạng thái để hiển thị trong dropdown
            ViewBag.OrderStatuses = db.OrderStatus.ToList();

            return View();
        }

        [HttpPost]
        public JsonResult UpdateOrderStatus(int orderId, int statusId)
        {
            try
            {
                var order = db.Orders.Find(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                var status = db.OrderStatus.FirstOrDefault(s => s.StatusId == statusId); // Giả định OrderStatu
                if (status == null)
                {
                    return Json(new { success = false, message = "Trạng thái không hợp lệ!" });
                }

                order.StatusId = statusId;
                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật trạng thái thành công!", newStatus = status.StatusName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

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