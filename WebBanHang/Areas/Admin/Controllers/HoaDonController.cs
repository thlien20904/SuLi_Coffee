using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class HoaDonController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4();

        public ActionResult HoaDon(DateTime? startDate, DateTime? endDate)
        {
            var orders = db.Orders
                .Include(o => o.PhuongThucThanhToan)
                .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1); // Bao gồm cả ngày cuối
                orders = orders.Where(o => o.OrderDate >= startDate.Value && o.OrderDate <= endDateInclusive);
            }

            var list = orders.ToList();

            System.Diagnostics.Debug.WriteLine("Số đơn hàng lấy được: " + list.Count);

            foreach (var order in list)
            {
                System.Diagnostics.Debug.WriteLine($"OrderId: {order.OrderId}, UserId: {order.UserId}, OrderDate: {order.OrderDate}, TotalAmount: {order.TotalAmount}, PaymentMethod: {order.PhuongThucThanhToan?.TenPhuongThuc}, Status: {order.StatusId}");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_HoaDonTable", list);
            }

            return View(list);
        }

        public ActionResult ChiTietHoaDon(int orderId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ChiTietHoaDon called with OrderId: {orderId}");

                var chiTiet = db.OrderDetails
                               .Include(od => od.Food)
                               .Include(od => od.Size)
                               .Include(od => od.Topping)
                               .Where(c => c.OrderId == orderId)
                               .ToList();

                System.Diagnostics.Debug.WriteLine($"Số chi tiết hóa đơn lấy được: {chiTiet.Count}");

                if (!chiTiet.Any())
                {
                    return Content("<tr><td colspan='5' style='color: red;'>⚠️ Không có dữ liệu</td></tr>");
                }

                return PartialView("_ChiTietHoaDon", chiTiet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");
                return Content("<tr><td colspan='5' style='color: red;'>⚠️ Lỗi server</td></tr>");
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