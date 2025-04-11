using System.Linq;
using System.Web.Mvc;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private WebAppDBEntities4 db = new WebAppDBEntities4();

        public ActionResult Home()
        {
            var bestSellers = db.InvoiceDetails
                .Where(id => id.FoodId != null)
                .GroupBy(id => id.FoodId)
                .Select(g => new
                {
                    FoodId = g.Key.Value,
                    TotalSold = g.Sum(id => id.SoLuong)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(8)
                .ToList()
                .Join(db.Foods,
                      bs => bs.FoodId,
                      food => food.FoodId,
                      (bs, food) => new BanChayModel
                      {
                          FoodId = bs.FoodId,
                          FoodName = food != null ? food.FoodName : "Không có tên",
                          ImageUrl = food != null ? food.ImageURL : Url.Content("~/Images/no-image.png"),
                          Price = food != null ? food.Price : 0,
                          TotalSold = bs.TotalSold
                      })
                .ToList();

            ViewBag.BanChay = bestSellers;

            return View();
        }
    }
}
