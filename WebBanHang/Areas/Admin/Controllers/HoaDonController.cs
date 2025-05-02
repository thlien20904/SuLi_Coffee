using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;
using ClosedXML.Excel;
using System.IO;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class HoaDonController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4();

        // Action hiển thị danh sách hóa đơn
        public ActionResult HoaDon(DateTime? startDate, DateTime? endDate)
        {
            var orders = db.Orders
                .Include(o => o.PhuongThucThanhToan)
                .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                orders = orders.Where(o => o.OrderDate >= startDate.Value && o.OrderDate <= endDateInclusive);
            }

            var list = orders.ToList();

            // Nếu là yêu cầu AJAX, trả về PartialView chỉ chứa các hàng
            if (Request.IsAjaxRequest())
            {
                return PartialView("_HoaDonRows", list);
            }

            // Nếu không phải AJAX, trả về View đầy đủ
            return View(list);
        }

        // Action xuất danh sách hóa đơn ra Excel
        public ActionResult ExportToExcel(DateTime? startDate, DateTime? endDate)
        {
            var orders = db.Orders
                .Include(o => o.PhuongThucThanhToan)
                .AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                orders = orders.Where(o => o.OrderDate >= startDate.Value && o.OrderDate <= endDateInclusive);
            }

            var orderList = orders.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách hóa đơn");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "STT";
                worksheet.Cell(currentRow, 2).Value = "Mã hóa đơn";
                worksheet.Cell(currentRow, 3).Value = "Mã người dùng";
                worksheet.Cell(currentRow, 4).Value = "Ngày đặt hàng";
                worksheet.Cell(currentRow, 5).Value = "Tổng tiền";
                worksheet.Cell(currentRow, 6).Value = "Phương thức thanh toán";
                worksheet.Cell(currentRow, 7).Value = "Trạng thái";

                // Định dạng header
                for (int col = 1; col <= 7; col++)
                {
                    worksheet.Cell(currentRow, col).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, col).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Cell(currentRow, col).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(currentRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Dữ liệu
                int index = 1;
                foreach (var order in orderList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = index++;
                    worksheet.Cell(currentRow, 2).Value = order.OrderId;
                    worksheet.Cell(currentRow, 3).Value = order.UserId;
                    worksheet.Cell(currentRow, 4).Value = order.OrderDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 5).Value = order.TotalAmount;
                    worksheet.Cell(currentRow, 6).Value = order.PhuongThucThanhToan?.TenPhuongThuc ?? "N/A";
                    worksheet.Cell(currentRow, 7).Value = order.StatusId;
                }

                // Tự động điều chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Lưu vào MemoryStream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileName = "DanhSachHoaDon.xlsx";
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream.ToArray(), contentType, fileName);
                }
            }
        }

        public ActionResult ExportChiTietToExcel(int orderId)
        {
            var chiTiet = db.OrderDetails
                .Include(od => od.Food)
                .Include(od => od.Size)
                .Where(c => c.OrderId == orderId)
                .ToList();

            if (!chiTiet.Any())
            {
                return Content("Không có dữ liệu chi tiết để xuất!");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"ChiTietHoaDon_{orderId}");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "STT";
                worksheet.Cell(currentRow, 2).Value = "Tên sản phẩm";
                worksheet.Cell(currentRow, 3).Value = "Kích thước";
                worksheet.Cell(currentRow, 4).Value = "Topping";
                worksheet.Cell(currentRow, 5).Value = "Số lượng";
                worksheet.Cell(currentRow, 6).Value = "Giá";

                // Định dạng header
                for (int col = 1; col <= 6; col++)
                {
                    worksheet.Cell(currentRow, col).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, col).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Cell(currentRow, col).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(currentRow, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Dữ liệu
                int index = 1;
                foreach (var item in chiTiet)
                {
                    currentRow++;
                    var toppingName = item.ToppingId.HasValue
                        ? db.Toppings.FirstOrDefault(t => t.ToppingID == item.ToppingId)?.ToppingName ?? "Không có"
                        : "Không có";

                    worksheet.Cell(currentRow, 1).Value = index++;
                    worksheet.Cell(currentRow, 2).Value = item.Food != null ? item.Food.FoodName : "Không xác định";
                    worksheet.Cell(currentRow, 3).Value = item.Size != null ? item.Size.SizeName : "Không có";
                    worksheet.Cell(currentRow, 4).Value = toppingName;
                    worksheet.Cell(currentRow, 5).Value = item.Quantity;
                    worksheet.Cell(currentRow, 6).Value = item.Price.ToString("N0") + " đ";
                }

                // Tự động điều chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Lưu vào MemoryStream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileName = $"ChiTietHoaDon_{orderId}.xlsx";
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    return File(stream.ToArray(), contentType, fileName);
                }
            }
        }
        // Action chi tiết hóa đơn
        public ActionResult ChiTietHoaDon(int orderId)
        {
            try
            {
                var chiTiet = db.OrderDetails
                               .Include(od => od.Food)
                               .Include(od => od.Size)
                               .Include(od => od.Topping)
                               .Where(c => c.OrderId == orderId)
                               .ToList();

                if (!chiTiet.Any())
                {
                    return Content("<tr><td colspan='5' style='color: red;'>⚠️ Không có dữ liệu</td></tr>");
                }

                return PartialView("_ChiTietHoaDon", chiTiet);
            }
            catch (Exception ex)
            {
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