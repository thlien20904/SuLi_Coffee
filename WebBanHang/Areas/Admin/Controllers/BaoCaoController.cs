using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    public class BaoCaoController : Controller
    {
        private WebAppDBEntities3 db = new WebAppDBEntities3();

        // 🏆 Hiển thị trang tổng hợp báo cáo
        public ActionResult BaoCao()
        {
            return View();
        }

        // 📌 Hiển thị danh sách Hóa Đơn
        public ActionResult HoaDon(DateTime? startDate, DateTime? endDate)
        {
            var hoaDons = db.Invoices.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.DateCheckIn >= startDate.Value
                    && (h.DateCheckOut == null || h.DateCheckOut <= endDate.Value)); // Xử lý DateCheckOut NULL
            }

            var list = hoaDons.ToList();
            System.Diagnostics.Debug.WriteLine("Số hóa đơn lấy được: " + list.Count);

            // Debug chi tiết dữ liệu lấy được
            foreach (var hd in list)
            {
                System.Diagnostics.Debug.WriteLine($"InvoiceId: {hd.InvoiceId}, TableId: {hd.TableId}, DateCheckIn: {hd.DateCheckIn}, DateCheckOut: {hd.DateCheckOut}, TrangThai: {hd.TrangThai}");
            }

            // ✅ Nếu là request AJAX, trả về PartialView để cập nhật bảng dữ liệu
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_HoaDonTable", list);
            }

            // ✅ Nếu là request bình thường, trả về cả giao diện
            return View(list);
        }
        public ActionResult ChiTietHoaDon(int invoiceId)
        {
            var chiTiet = db.InvoiceDetails
                           .Where(c => c.InvoiceId == invoiceId)
                           .ToList();

            if (!chiTiet.Any())
            {
                return Content("<tr><td colspan='3' style='color: red;'>⚠️ Không có dữ liệu</td></tr>");
            }

            return PartialView("_ChiTietHoaDon", chiTiet);
        }



        // 💰 Hiển thị Doanh Thu
        public ActionResult DoanhThu()
        {
            decimal tongDoanhThu = db.InvoiceDetails.Sum(d => (decimal?)d.Price) ?? 0;
            ViewBag.TongDoanhThu = tongDoanhThu;
            return View();
        }
        public ActionResult BanChay()
        {
            var bestSellers = db.InvoiceDetails
                .GroupBy(d => d.FoodId)
                .Select(g => new
                {
                    FoodId = g.Key,
                    TotalSold = g.Sum(d => d.SoLuong)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(8) // Giới hạn 8 sản phẩm bán chạy
                .ToList();

            var danhSachBanChay = bestSellers
                .Join(db.Foods,
                      d => d.FoodId,
                      f => f.FoodId,
                      (d, f) => new WebBanHang.Models.BanChayModel
                      {
                          FoodId = f.FoodId,
                          FoodName = f.FoodName,
                          ImageUrl = f.ImageURL,
                          Price = f.Price,
                          TotalSold = d.TotalSold
                      })
                .ToList();

            if (danhSachBanChay.Any())
            {
                return View(danhSachBanChay);
            }
            else
            {
                ViewBag.ErrorMessage = "Không có sản phẩm bán chạy.";
                return View(new List<WebBanHang.Models.BanChayModel>()); // Trả về danh sách rỗng nhưng không null
            }
        }


        public ActionResult ExportToExcel()
        {
            var matHangBanChay = db.InvoiceDetails
                .GroupBy(d => d.FoodId)
                .Select(g => new
                {
                    FoodId = g.Key,
                    TotalSold = g.Sum(d => d.SoLuong)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(10)
                .ToList();

            // Join với bảng Foods để lấy thông tin sản phẩm
            var products = matHangBanChay
                .Join(db.Foods, sold => sold.FoodId, food => food.FoodId, (sold, food) => new
                {
                    food.FoodId,
                    food.FoodName,
                    food.Price,
                    TotalSold = sold.TotalSold
                })
                .ToList();

            using (var workbook = new XLWorkbook())  // ✅ ClosedXML
            {
                var worksheet = workbook.Worksheets.Add("SanPhamBanChay");

                // Tiêu đề cột
                worksheet.Cell("A1").Value = "ID";
                worksheet.Cell("B1").Value = "Tên sản phẩm";
                worksheet.Cell("C1").Value = "Giá";
                worksheet.Cell("D1").Value = "Đã bán";

                int row = 2;
                foreach (var item in products)
                {
                    worksheet.Cell(row, 1).Value = item.FoodId;
                    worksheet.Cell(row, 2).Value = item.FoodName;
                    worksheet.Cell(row, 3).Value = item.Price;
                    worksheet.Cell(row, 4).Value = item.TotalSold;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SanPhamBanChay.xlsx");
                }
            }
        }


    }
}
